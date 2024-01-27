using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

abstract public class GameSceneBaseScript : MonoBehaviour
{
    /// <summary>�J�����I�u�W�F�N�g</summary>
    public Camera mainCamera;

    /// <summary>�t�B�[���h�Ǘ�</summary>
    protected Dictionary<int, Dictionary<int, GameObject>> fieldCellList;

    /// <summary>�Q�[�����</summary>
    protected Constant.GameState state;

    /// <summary>�L�^�p�ő�HP</summary>
    protected double saveMaxHp;
    /// <summary>�^�[����</summary>
    protected int gameTurn;

    /// <summary>�v���C���[��ێ�</summary>
    protected GameObject playerCell;

    /// <summary>�v���C���[�I�u�W�F�N�g���ێ�</summary>
    protected GameObject playerObj = null;

    /// <summary>�X�L���Q�[�W�P��̃T�C�Y</summary>
    protected int skillGaugeLength = 0;
    /// <summary>�X�L���X�g�b�N�ő�</summary>
    protected int skillStockMax = 0;
    /// <summary>�X�L���Q�[�W���ݒl</summary>
    protected int skillGaugeNow = 0;
    /// <summary>�X�L���g�p�\��</summary>
    protected int skillCount = 0;
    /// <summary>�X�L���g�p�ҋ@���̎˒�</summary>
    protected int skillRange = 0;

    /// <summary>�X�L��UI</summary>
    protected SkillUIScript skillUIScript = null;

    /// <summary>�J�[�\���v���n�u���\�[�X</summary>
    private ResourceRequest cursorHandle;
    /// <summary>�G�v���n�u���\�[�X</summary>
    private ResourceRequest enemyHandle;
    /// <summary>�v���C���[�v���n�u���\�[�X</summary>
    private ResourceRequest playerHandle;
    /// <summary>�v���C���[Animator���\�[�X</summary>
    private ResourceRequest playerAnimHandle;

    /// <summary>�U���G�t�F�N�g�v���n�u���\�[�X</summary>
    private ResourceRequest attackEffectHandle;
    /// <summary>�K�E�Z�G�t�F�N�g�v���n�u���\�[�X</summary>
    private ResourceRequest skillEffectHandle;
    /// <summary>�X�L���������G�t�F�N�g�v���n�u���\�[�X</summary>
    private ResourceRequest skillGradEffectHandle;

    /// <summary>�Q�[���I�[�o�[�\��</summary>
    public GameObject gameoverFade;

    /// <summary>�Q�[�������j���[</summary>
    public GameObject gameMenuUI;
    public Toggle chkOptionBattleSkip;
    public Slider slOptionBGMVolume;
    public Slider slOptionSEVolume;

    // Start is called before the first frame update
    protected virtual IEnumerator Start()
    {
        state = Constant.GameState.Loading;
        gameoverFade.SetActive(false);

        cursorHandle = Resources.LoadAsync<GameObject>("InGameScene/hexCursorPrefab");
        enemyHandle = Resources.LoadAsync<GameObject>("InGameScene/enemyInGamePrefab");
        playerHandle = Resources.LoadAsync<GameObject>("InGameScene/playerInGamePrefab");
        skillGradEffectHandle = Resources.LoadAsync<GameObject>("InGameScene/skillGradation");
        var playerInitHp = 1.0;
        var selectChara = Global.GetTemporaryData().selectPlayer;
        var playerAnimResource = Constant.GetPlayerNameE(selectChara);
        Constant.CharacterParams characterParams;
        switch (selectChara)
        {
            case Constant.PlayerID.Drows:
                characterParams = Constant.DROWS_PARAM;
                break;
            case Constant.PlayerID.Eraps:
                characterParams = Constant.ERAPS_PARAM;
                break;
            case Constant.PlayerID.Exa:
                characterParams = Constant.EXA_PARAM;
                break;
            case Constant.PlayerID.Worra:
                attackEffectHandle = Resources.LoadAsync<GameObject>("InGameScene/effectNeedle");
                characterParams = Constant.WORRA_PARAM;
                break;
            case Constant.PlayerID.Koob:
                attackEffectHandle = Resources.LoadAsync<GameObject>("InGameScene/effectMagicS");
                skillEffectHandle = Resources.LoadAsync<GameObject>("InGameScene/effectMagicL");
                characterParams = Constant.KOOB_PARAM;
                break;
            case Constant.PlayerID.You:
                characterParams = Constant.YOU_PARAM;
                break;
            default:
                characterParams = new Constant.CharacterParams();
                break;
        }
        playerInitHp = characterParams.InitHP;
        skillCount = 0;
        skillStockMax = characterParams.SkillStockMax;
        skillGaugeLength = characterParams.SkillGaugeLength;
        skillGaugeNow = 0;
        skillUIScript = GameObject.FindGameObjectWithTag("SkillUITag").GetComponent<SkillUIScript>();
        skillUIScript.InitObject();
        UpdateSkillUI();

        playerAnimHandle = Resources.LoadAsync<RuntimeAnimatorController>("InGameScene/Anim/" + playerAnimResource);

        fieldCellList = new Dictionary<int, Dictionary<int, GameObject>>();

        yield return cursorHandle;
        yield return enemyHandle;
        yield return playerHandle;
        yield return playerAnimHandle;
        yield return attackEffectHandle;
        yield return skillEffectHandle;
        yield return skillGradEffectHandle;

        CreateFieldCell(0, 0, Constant.CharacterType.Player);
        playerCell = fieldCellList[0][0];
        playerCell.GetComponent<FieldCellScript>().InitCharacterHp(playerInitHp);
        playerObj = playerCell.transform.Find("playerInGamePrefab(Clone)").gameObject;

        gameTurn = 0;

        InitGameSystem();
        playerObj.GetComponent<CharacterScript>().SetCharacterRight(true);

        yield return null;

        yield return TurnStartBase();
    }

    protected abstract void InitGameSystem();

    public Constant.GameState GetGameState() { return state; }

    /// <summary>
    /// �G�N���b�N�\����
    /// </summary>
    /// <returns></returns>
    public int GetClickRange()
    {
        if (state == Constant.GameState.Idle)
        {
            return GetPlayerObject().GetComponent<PlayerScript>().GetAttackRange();
        }
        else if (state == Constant.GameState.SkillWait)
        {
            return skillRange;
        }

        return 0;
    }

    protected virtual void OnDestroy()
    {
        DestroyFieldAll();

        //Resources.UnloadAsset(cursorHandle.asset);
        //Resources.UnloadAsset(enemyHandle.asset);
        //Resources.UnloadAsset(playerHandle.asset);
        Resources.UnloadAsset(playerAnimHandle.asset);

        if (attackEffectHandle?.isDone == true)
        {
            Resources.UnloadAsset(attackEffectHandle.asset);
        }
        if (skillEffectHandle?.isDone == true)
        {
            Resources.UnloadAsset(skillEffectHandle.asset);
        }
        //Resources.UnloadAsset(skillGradEffectHandle.asset);
        Resources.UnloadUnusedAssets();
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    /// <summary>
    /// �t�B�[���h�P�}�X�쐬
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    /// <param name="type">�z�u�L�����N�^�[</param>
    /// <param name="animResource">���\�[�X</param>
    protected void CreateFieldCell(int x, int y, Constant.CharacterType type, RuntimeAnimatorController animResource = null)
    {
        if (!fieldCellList.ContainsKey(y))
        {
            fieldCellList.Add(y, new Dictionary<int, GameObject>());
        }

        GameObject cellObj;
        FieldCellScript cellScr;
        if (fieldCellList[y].ContainsKey(x))
        {
            // ���łɑ��݂��Ă�
            cellObj = fieldCellList[y][x];
            cellScr = cellObj.GetComponent<FieldCellScript>();

            // �L���������łɋ���
            if (cellScr.GetCharacterType() != Constant.CharacterType.Empty)
            {
                return;
            }

            // ����ێ�����
            if (cellScr.IsEmptyTime())
            {
                return;
            }
        }
        else
        {
            // ���݂��Ȃ��ꍇ�쐬
            cellObj = new GameObject();
            cellObj.AddComponent<FieldCellScript>();
            cellScr = cellObj.GetComponent<FieldCellScript>();
            // �J�[�\���쐬
            cellScr.CreateCursor(cursorHandle.asset as GameObject);

            // �ʒu�ݒ�
            cellObj.GetComponent<FieldCellScript>().SetLocation(x, y);
            fieldCellList[y].Add(x, cellObj);
        }


        // �L�����N�^�[�ݒ�
        switch (type)
        {
            case Constant.CharacterType.Player:
                cellScr.CreateCharacter(type, playerHandle.asset as GameObject, playerAnimHandle.asset as RuntimeAnimatorController);
                break;
            case Constant.CharacterType.Enemy:
                cellScr.CreateCharacter(type, enemyHandle.asset as GameObject);
                break;
            case Constant.CharacterType.Boss:
                cellScr.CreateCharacter(type, enemyHandle.asset as GameObject, animResource);
                break;
        }
    }

    /// <summary>
    /// �Z���S�폜
    /// </summary>
    protected void DestroyFieldAll()
    {
        if (fieldCellList != null)
        {
            foreach (var row in fieldCellList)
            {
                foreach (var cellObj in row.Value)
                {
                    if (!cellObj.Value.IsDestroyed())
                    {
                        Destroy(cellObj.Value);
                    }
                }
                row.Value.Clear();
            }
            fieldCellList.Clear();
        }

        playerCell = null;
        playerObj = null;
    }

    /// <summary>
    /// �t�B�[���h�Z���X�N���v�g�擾
    /// </summary>
    /// <param name="_loc"></param>
    /// <returns></returns>
    protected FieldCellScript GetFieldCellScript(Vector2Int _loc)
    {
        if (!fieldCellList.ContainsKey(_loc.y))
        {
            return null;
        }

        if (!fieldCellList[_loc.y].ContainsKey(_loc.x))
        {
            return null;
        }

        return fieldCellList[_loc.y][_loc.x].GetComponent<FieldCellScript>();
    }

    /// <summary>
    /// �L�����N�^�[�X�N���v�g�擾
    /// </summary>
    /// <param name="_loc"></param>
    /// <returns></returns>
    protected CharacterScript GetCharacterScript(Vector2Int _loc)
    {
        var cellScr = GetFieldCellScript(_loc);
        if (cellScr == null) { return null; }

        return cellScr.GetCharacterScript();
    }

    /// <summary>
    /// �L�����N�^�[�����݂��邩�m�F
    /// </summary>
    /// <param name="loc">���W</param>
    /// <returns></returns>
    public bool IsCharacterBeing(Vector2Int loc)
    {
        var scr = GetFieldCellScript(loc);
        if (scr == null) { return false; }

        if (scr.GetCharacterType() == Constant.CharacterType.Empty)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// �v���C���[�̈ʒu
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetPlayerLocation()
    {
        return playerCell.GetComponent<FieldCellScript>().GetLocation();
    }

    /// <summary>
    /// �v���C���[GameObject�擾
    /// </summary>
    /// <returns></returns>
    public GameObject GetPlayerObject()
    {
        return playerObj;
    }

    /// <summary>
    /// �G�N���b�N���̏���
    /// </summary>
    /// <param name="enm"></param>
    public void EnemyClickEvent(GameObject enm)
    {
        StartCoroutine(ExecuteBattle(enm));
    }

    /// <summary>
    /// �G�N���b�N���̃R���[�`��
    /// </summary>
    /// <param name="enm"></param>
    protected IEnumerator ExecuteBattle(GameObject enm)
    {
        if (state == Constant.GameState.SkillWait)
        {
            yield return ExecuteSkillWait(enm);
            state = Constant.GameState.Idle;
            yield break;
        }
        else if (state != Constant.GameState.Idle)
        {
            yield break;
        }

        state = Constant.GameState.Battle;

        var enmScript = enm.GetComponent<CharacterScript>();
        var enmCellScript = enmScript.GetCellScript();
        var plrScript = playerObj.GetComponent<CharacterScript>();

        // ������ς���
        var distVec = enmCellScript.GetLocation() - GetPlayerLocation();
        var distance = Util.CalcLocationDistance(enmCellScript.GetLocation(), GetPlayerLocation());
        var isRight = distVec.x * 2 > distVec.y;
        plrScript.SetCharacterRight(isRight);
        if (enmCellScript.GetCharacterType() == Constant.CharacterType.Boss)
        {
            enmScript.SetCharacterRight(!isRight);
        }

        // ���E
        List<Vector2Int> lrList = null;

        // �ђʍU����
        var penetrateList = new List<Vector2Int>();
        if (plrScript.GetAttackCount() > 0)
        {
            if (distance == 1)
            {
                penetrateList.Add(enmCellScript.GetLocation() + distVec);
            }
            else
            {
                penetrateList.AddRange(Util.GetAroundLocations(enmCellScript.GetLocation()));
            }
        }

        var attackCount = 0;

        while (true)
        {
            // �v���C���[�U��
            var pAtk = plrScript.GetAttackNum();
            enmScript.Damage(pAtk);
            attackCount++;

            if (!Global.GetSaveData().IsBattleSkip() || attackCount == 1)
            {
                switch (Global.GetTemporaryData().selectPlayer)
                {
                    case Constant.PlayerID.Drows:
                        SoundManager.GetInstance().PlaySE(SoundManager.SeID.ATTACK_DROWS);
                        break;
                    case Constant.PlayerID.Eraps:
                        SoundManager.GetInstance().PlaySE(SoundManager.SeID.ATTACK_ERAPS);
                        break;
                    case Constant.PlayerID.Exa:
                        SoundManager.GetInstance().PlaySE(SoundManager.SeID.ATTACK_EXA);
                        break;
                    case Constant.PlayerID.Worra:
                        SoundManager.GetInstance().PlaySE(SoundManager.SeID.ATTACK_WORRA);
                        break;
                    case Constant.PlayerID.Koob:
                        SoundManager.GetInstance().PlaySE(SoundManager.SeID.ATTACK_KOOB);
                        break;
                    case Constant.PlayerID.You:
                        SoundManager.GetInstance().PlaySE(SoundManager.SeID.ATTACK_YOU);
                        break;
                }
                plrScript.AnimateAttack();
                PlayAttackEffect(enmCellScript.GetLocation(), isRight);
            }

            if (Global.GetTemporaryData().selectPlayer == Constant.PlayerID.Exa)
            {
                //�G�O�U ���E���U��
                lrList = Util.GetLRWingLocations(distVec);
                foreach (var lr in lrList)
                {
                    var target = GetPlayerLocation() + lr;
                    var lrScr = GetCharacterScript(target);
                    if (!lrScr) { continue; }
                    lrScr.Damage(pAtk * Constant.SKILL_EXA_SIDE_ATTACK_RATE);
                }
            }

            var rapidTargetScr = enmScript;
            var playEffect = false;
            for (var i = 1; i < plrScript.GetAttackCount(); i++)
            {
                if (!Global.GetSaveData().IsBattleSkip())
                {
                    yield return new WaitForSeconds(0.1f);
                }
                //2��ȏ�U�� �i�G�O�U�͈͔̔͂��f����Ȃ����ɒ��Ӂj

                if (rapidTargetScr.IsAlive())
                {
                    // �c���Ă����炻�̂܂�
                    rapidTargetScr.Damage(pAtk);
                    if (!Global.GetSaveData().IsBattleSkip())
                    {
                        PlayAttackEffect(rapidTargetScr.GetCellScript().GetLocation(), isRight);
                    }
                }
                else if (distance == 1)
                {
                    // ����ł�����ђʍU��
                    // �����̏ꍇ����ɐi��
                    var penetrateLoc = rapidTargetScr.GetCellScript().GetLocation() + distVec;
                    var penetScr = GetCharacterScript(penetrateLoc);
                    if (!penetScr) { break; }
                    if (!penetScr.IsAlive()) { break; };

                    penetScr.Damage(pAtk);
                    PlayAttackEffect(penetrateLoc, isRight);

                    rapidTargetScr = penetScr;
                    penetrateList.Add(penetrateLoc);
                }
                else
                {
                    // ���͂̂�
                    var penetCount = 0;
                    foreach (var penetrateLoc in penetrateList)
                    {
                        var penetScr = GetCharacterScript(penetrateLoc);
                        if (!penetScr) { continue; }
                        if (!penetScr.IsAlive()) { continue; };

                        penetCount++;
                        penetScr.Damage(pAtk);
                        if (!Global.GetSaveData().IsBattleSkip() || !playEffect)
                        {
                            PlayAttackEffect(penetrateLoc, isRight);
                        }
                    }
                    playEffect = true;

                    if (penetCount == 0)
                    {
                        break;
                    }
                }
            }

            if (!Global.GetSaveData().IsBattleSkip())
            {
                yield return new WaitForSeconds(0.4f);
            }

            if (Global.GetTemporaryData().selectPlayer == Constant.PlayerID.Exa)
            {
                //�G�O�U ���E����
                foreach (var lr in lrList)
                {
                    var target = GetPlayerLocation() + lr;
                    var lrScr = GetCharacterScript(target);
                    if (!lrScr) { continue; }
                    if (!lrScr.IsAlive())
                    {
                        //�����A���S�A�j���[�V����
                        plrScript.AddHp(lrScr.GetMaxHp());
                        lrScr.GetCellScript().DeathCharacterObj();
                        yield return DestroyEnemy();
                    }
                }
            }

            if (!enmScript.IsAlive())
            {
                // ����
                plrScript.AddHp(enmScript.GetMaxHp());

                // �G�̎��S�A�j���[�V����
                enmCellScript.DeathCharacterObj();
                yield return DestroyEnemy();

                if (Global.GetSaveData().IsBattleSkip())
                {
                    yield return new WaitForSeconds(0.4f);
                }

                // �E�[���̊ђʂ����G
                foreach (var penetrateVec in penetrateList)
                {
                    var penetScr = GetCharacterScript(penetrateVec);
                    if (!penetScr) { continue; }
                    if (penetScr.IsAlive()) { continue; }
                    plrScript.AddHp(penetScr.GetMaxHp());
                    penetScr.GetCellScript().DeathCharacterObj();
                    yield return DestroyEnemy();
                }

                yield return new WaitForSeconds(0.2f);

                // �v���C���[Object���ړ�
                yield return MovePlayerLocation(enmCellScript.GetLocation());

                if (attackCount != 1 || Global.GetTemporaryData().selectPlayer != Constant.PlayerID.Drows)
                {
                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    // �ꌂ�œ|�����ꍇ�h���V�[ �A���U��
                    var attacked = false;
                    for (int rushCount = 1; rushCount < Constant.SKILL_DROWS_ATTACK_LIMIT; ++rushCount)
                    {
                        yield return DrowsAttackEnd();

                        var rushNext = GetPlayerLocation() + distVec;
                        var rushEnemyScr = GetCharacterScript(rushNext);
                        if (!rushEnemyScr)
                        {
                            //�������Ȃ��ƒ�~
                            break;
                        }

                        attacked = true;
                        rushEnemyScr.Damage(plrScript.GetAttackNum());
                        if (rushEnemyScr.IsAlive())
                        {
                            //�|���ĂȂ��ƒ�~
                            break;
                        }

                        // �A�����j�Ő������A�b�v
                        var upRate = 1.0 + 0.03 * rushCount;
                        plrScript.AddHp(rushEnemyScr.GetMaxHp() * upRate);

                        // �ꌂ�œ|������葱�s
                        var rushCellScr = rushEnemyScr.GetCellScript();
                        rushCellScr.DeathCharacterObj();
                        yield return DestroyEnemy();
                        yield return MovePlayerLocation(rushCellScr.GetLocation());
                    }

                    if (attacked)
                    {
                        SoundManager.GetInstance().PlaySE(SoundManager.SeID.ATTACK_DROWS);
                        plrScript.AnimateAttack();
                        yield return new WaitForSeconds(0.4f);
                    }
                }

                break;
            }

            // �G�U��
            var eAtk = enmScript.GetAttackNum();
            // �����ɂ���Č���
            if (distance > 1)
            {
                eAtk = eAtk / (distance * 2 - 1);
            }

            // SE
            if (!Global.GetSaveData().IsBattleSkip())
            {
                if (plrScript.GetSp() > 0)
                {
                    SoundManager.GetInstance().PlaySE(SoundManager.SeID.DAMAGE_SHIELD);
                }
                else
                {
                    SoundManager.GetInstance().PlaySE(SoundManager.SeID.DAMAGE_NORMAL);
                }
            }
            plrScript.Damage(eAtk);

            if (!Global.GetSaveData().IsBattleSkip())
            {
                enmScript.AnimateAttack();
                yield return new WaitForSeconds(0.4f);
            }

            if (!plrScript.IsAlive())
            {
                if (Global.GetSaveData().IsBattleSkip())
                {
                    yield return new WaitForSeconds(0.4f);
                }

                // GameOver
                state = Constant.GameState.GameOverFade;

                // �t�F�[�h����
                SoundManager.GetInstance().PlaySE(SoundManager.SeID.GAMEOVER);
                gameoverFade.SetActive(true);
                var fadeGroup = gameoverFade.GetComponent<CanvasGroup>();
                while (true)
                {
                    var a = fadeGroup.alpha;
                    a += Time.deltaTime / 2f;
                    if (a > 1f)
                    {
                        fadeGroup.alpha = 1f;
                        break;
                    }

                    fadeGroup.alpha = a;
                    yield return null;
                }

                state = Constant.GameState.GameOver;

                yield break;
            }
        }

        yield return TurnEndBase();
        yield return TurnStartBase();
    }

    /// <summary>
    /// �X�L���ҋ@���ɓG�N���b�N�Ŏ��s
    /// </summary>
    /// <param name="enm">�GObject</param>
    /// <returns></returns>
    protected IEnumerator ExecuteSkillWait(GameObject enm)
    {
        skillCount--;
        UpdateSkillUI();
        state = Constant.GameState.Battle;
        var pScr = GetPlayerObject().GetComponent<PlayerScript>();
        var enmScript = enm.GetComponent<CharacterScript>();
        var enmCellScript = enmScript.GetCellScript();

        // ������ς���
        var eLoc = enmCellScript.GetLocation();
        var distVec = eLoc - GetPlayerLocation();
        var isRight = distVec.x * 2 > distVec.y;
        pScr.SetCharacterRight(isRight);

        PlaySkillExecEffect();
        yield return new WaitForSeconds(1f);

        switch (Global.GetTemporaryData().selectPlayer)
        {
            case Constant.PlayerID.Exa:
                SoundManager.GetInstance().PlaySE(SoundManager.SeID.SKILL_EXA);
                // �G�O�U�@�P��U������HP�z��
                var lrList = Util.GetLRWingLocations(distVec);
                var pAtk = pScr.GetAttackNum();
                enmScript.Damage(pAtk);
                pScr.AnimateAttack();
                PlayAttackEffect(enmCellScript.GetLocation(), isRight);

                foreach (var lr in lrList)
                {
                    var target = GetPlayerLocation() + lr;
                    var lrScr = GetCharacterScript(target);
                    if (!lrScr) { continue; }
                    lrScr.Damage(pAtk);
                    pScr.AddHp(pAtk);
                }
                pScr.AddHp(pAtk);
                yield return new WaitForSeconds(0.4f);

                foreach (var lr in lrList)
                {
                    var target = GetPlayerLocation() + lr;
                    var lrScr = GetCharacterScript(target);
                    if (!lrScr) { continue; }
                    if (!lrScr.IsAlive())
                    {
                        pScr.AddHp(lrScr.GetMaxHp());
                        lrScr.GetCellScript().DeathCharacterObj();
                        yield return DestroyEnemy();
                    }
                }
                if (!enmScript.IsAlive())
                {
                    pScr.AddHp(enmScript.GetMaxHp());
                    enmCellScript.DeathCharacterObj();
                    yield return DestroyEnemy();

                    yield return MovePlayerLocation(eLoc);
                    yield return new WaitForSeconds(0.2f);

                    ImmediatePlayerMove();
                }

                UpdateCellColor();
                break;
            case Constant.PlayerID.You:
                SoundManager.GetInstance().PlaySE(SoundManager.SeID.SKILL_YOU);
                // �I�@����
                var addHp = enmScript.GetMaxHp();
                enmScript.Damage(addHp);
                pScr.AnimateAttack();
                PlayAttackEffect(enmCellScript.GetLocation(), isRight);
                yield return new WaitForSeconds(0.4f);

                // ����
                pScr.AddHp(addHp);

                // �G�̎��S�A�j���[�V����
                enmCellScript.DeathCharacterObj();
                yield return DestroyEnemy();
                yield return new WaitForSeconds(0.2f);

                // �v���C���[Object���ړ�
                yield return MovePlayerLocation(eLoc);
                yield return new WaitForSeconds(0.2f);

                ImmediatePlayerMove();
                UpdateCellColor();
                break;
        }
    }

    /// <summary>
    /// �Q�[���I�[�o�[��N���b�N�Ń^�C�g���Ɉړ�
    /// </summary>
    public void GameOverClick()
    {
        if (state == Constant.GameState.GameOver)
        {
            ManagerScript.GetInstance().ChangeScene("TitleScene");

            SaveRecord();
        }
    }

    /// <summary>
    /// �v���C�L�^�ɕۑ�
    /// </summary>
    protected virtual void SaveRecord()
    {
    }

    /// <summary>
    /// �v���C���[�ړ�
    /// </summary>
    /// <param name="_newLocation"></param>
    /// <returns></returns>
    protected IEnumerator MovePlayerLocation(Vector2Int _newLocation)
    {
        var plrCellScript = playerCell.GetComponent<FieldCellScript>();
        plrCellScript.RemoveCharacterObj();

        playerCell = fieldCellList[_newLocation.y][_newLocation.x];
        var enmCellScript = playerCell.GetComponent<FieldCellScript>();
        enmCellScript.SetCharacterObj(playerObj, Constant.CharacterType.Player);

        var camera = GameObject.Find("Main Camera").GetComponent<InGameCameraScript>();
        camera.MoveTo(Util.GetBasePosition(_newLocation.x, _newLocation.y), Constant.CAMERA_MOVE_TIME);

        yield return null;
    }

    /// <summary>
    /// �U���G�t�F�N�g�Đ�
    /// </summary>
    /// <param name="loc"></param>
    /// <param name="isRight"></param>
    protected void PlayAttackEffect(Vector2Int loc, bool isRight)
    {
        var playPosition = Util.GetBasePosition(loc.x, loc.y);
        GameObject eff;

        switch (Global.GetTemporaryData().selectPlayer)
        {
            case Constant.PlayerID.Koob:
                eff = Instantiate(attackEffectHandle.asset as GameObject);
                eff.transform.position = playPosition;
                break;
            case Constant.PlayerID.Worra:
                eff = Instantiate(attackEffectHandle.asset as GameObject);
                eff.transform.Find("needle").GetComponent<OneShotEffect>().SetDestroyObject(eff);
                eff.transform.position = playPosition;
                if (!isRight)
                {
                    eff.transform.rotation = Quaternion.Euler(0, 0, 180);
                }
                break;
        }
    }

    /// <summary>
    /// �X�L���������̃O���f�[�V�����G�t�F�N�g
    /// </summary>
    protected void PlaySkillExecEffect()
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SKILL_EFFECT);

        var eff = Instantiate(skillGradEffectHandle.asset as GameObject);
        eff.transform.SetParent(playerObj.transform, false);
    }

    /// <summary>
    /// �J�[�\���F�X�V
    /// </summary>
    /// <param name="_skillRange">�X�L���˒��w�� -1�̏ꍇ�U���˒����O���[�i�f�t�H���g�j</param>
    protected void UpdateCellColor(int _skillRange = -1)
    {
        var plrScript = playerObj.GetComponent<CharacterScript>();

        foreach (var row in fieldCellList)
            foreach (var objItem in row.Value)
            {
                var scr = objItem.Value.GetComponent<FieldCellScript>();
                var dist = Util.CalcLocationDistance(GetPlayerLocation(), scr.GetLocation());

                int attackRange = _skillRange > 0 ? _skillRange : plrScript.GetAttackRange();
                Color cellColor;

                if (dist == 0)
                {
                    cellColor = Color.black;
                }
                else if (dist <= attackRange)
                {
                    cellColor = _skillRange > 0 ? Color.red : Color.gray;
                }
                else
                {
                    cellColor = Color.black;
                }

                scr.SetCellColor(cellColor);
                scr.UpdateHPColor(plrScript);
            }
    }

    /// <summary>
    /// �G��|�������̏���
    /// </summary>
    protected IEnumerator DestroyEnemy()
    {
        // MAX�łȂ��ꍇ or AUTO�̏ꍇ
        if (skillCount < skillStockMax || skillStockMax <= 0)
        {
            skillGaugeNow++;
            if (skillGaugeNow >= skillGaugeLength)
            {
                skillGaugeNow = 0;
                skillCount++;

                // AUTO�̏ꍇ������
                if (skillStockMax <= 0)
                {
                    yield return ExecuteSkill();
                }
            }
        }

        UpdateSkillUI();
    }

    /// <summary>
    /// �X�L��UI�X�V
    /// </summary>
    protected void UpdateSkillUI()
    {
        skillUIScript.SetSkillState(skillGaugeNow, skillGaugeLength, skillCount, skillStockMax);
    }

    /// <summary>
    /// �X�L��UI�N���b�N
    /// </summary>
    public void SkillUIClickEvent()
    {
        if (state == Constant.GameState.SkillWait)
        {
            state = Constant.GameState.Idle;
            UpdateCellColor();
            return;
        }

        if (state != Constant.GameState.Idle)
        {
            return;
        }

        StartCoroutine(ExecuteSkill());
    }

    /// <summary>
    /// �X�L�����s
    /// </summary>
    protected IEnumerator ExecuteSkill()
    {
        if (skillCount <= 0) { yield break; }

        var pScr = playerObj.GetComponent<CharacterScript>();
        var pLoc = GetPlayerLocation();

        // �L�������̏���
        switch (Global.GetTemporaryData().selectPlayer)
        {
            case Constant.PlayerID.Drows:
                skillCount--;
                UpdateSkillUI();
                // �h���V�[�@���^�[���U���͑���
                pScr.SetSkillValid(Constant.SKILL_DROWS_KEEP_TURN);
                PlaySkillExecEffect();
                yield return new WaitForSeconds(0.2f);
                break;
            case Constant.PlayerID.Eraps:
                skillCount--;
                UpdateSkillUI();
                // �G���@�V�[���h����
                pScr.IncreaseSpRate(Constant.SKILL_ERAPS_SP_UPDATE_RATE);
                PlaySkillExecEffect();
                yield return new WaitForSeconds(0.2f);
                break;
            case Constant.PlayerID.Exa:
                skillRange = 1;
                UpdateCellColor(skillRange);
                state = Constant.GameState.SkillWait;
                break;
            case Constant.PlayerID.Worra:
                skillCount--;
                UpdateSkillUI();
                // �E�[���@�˒��S�܂ő����@�ȍ~�͍U���񐔑���
                if (pScr.GetAttackRange() < Constant.SKILL_WORRA_MAX_RANGE)
                {
                    pScr.IncreaseAttackRange();
                }
                else
                {
                    pScr.IncreaseAttackCount();
                }
                PlaySkillExecEffect();
                yield return new WaitForSeconds(0.2f);
                break;
            case Constant.PlayerID.Koob:
                SoundManager.GetInstance().PlaySE(SoundManager.SeID.SKILL_KOOB);

                skillCount--;
                UpdateSkillUI();
                var addHp = 0.0;
                // �G�t�F�N�g�Đ�
                pScr.AnimateAttack();
                var eff = Instantiate(skillEffectHandle.asset as GameObject);
                eff.transform.position = Util.GetBasePosition(pLoc.x, pLoc.y);
                yield return new WaitForSeconds(0.2f);

                // �ړ�����
                var farLength = 0;
                var movableList = new List<Vector2Int>();
                // �|���Ȃ������z
                var aliveList = new List<Vector2Int>();

                // �N�[�@�͈͓��ɒ萔�{�_���[�W�A�����s�A�����s��
                for (int x = -Constant.SKILL_KOOB_RAGNAROK_RANGE; x <= Constant.SKILL_KOOB_RAGNAROK_RANGE; ++x)
                    for (int y = -Constant.SKILL_KOOB_RAGNAROK_RANGE; y <= Constant.SKILL_KOOB_RAGNAROK_RANGE; ++y)
                    {
                        // �ʒu�Ƌ�������
                        if (x == 0 && y == 0) continue;
                        var loc = pLoc + new Vector2Int(x, y);
                        var eScr = GetCharacterScript(loc);
                        if (!eScr) continue;
                        var length = Util.CalcLocationDistance(pLoc, loc);
                        if (length > Constant.SKILL_KOOB_RAGNAROK_RANGE) continue;

                        var dmg = pScr.GetAttackNum() * Constant.SKILL_KOOB_RAGNAROK_RATE;
                        eScr.Damage(dmg);
                        if (!eScr.IsAlive())
                        {
                            if (farLength < length)
                            {
                                movableList.Clear();
                                movableList.Add(loc);
                                farLength = length;
                            }
                            else if (farLength == length)
                            {
                                movableList.Add(loc);
                            }

                            addHp += eScr.GetMaxHp();
                            eScr.GetCellScript().DeathCharacterObj();
                            yield return DestroyEnemy();
                        }
                        else
                        {
                            // �|���ĂȂ������疃ჂƐ����s��
                            eScr.AddAttackInvalidCount(Constant.SKILL_KOOB_RAGNAROK_PARALYZE_TURN);
                            eScr.AddPowUpInvalidTurn(Constant.SKILL_KOOB_RAGNAROK_POW_INVALID_TURN);
                            aliveList.Add(loc);
                        }
                    }
                yield return new WaitForSeconds(1.2f);

                // �|�����ꍇ
                if (farLength > 0)
                {
                    pScr.AddHp(addHp);

                    // �����c�肪�����画��
                    if (aliveList.Count > 0)
                    {
                        var tmpDist = int.MaxValue;
                        var tmpList = new List<Vector2Int>();
                        movableList.ForEach(movableLoc =>
                        {
                            // �����c���1�ԋ߂������𔻒�
                            var aliveDist = aliveList.Min(aliveLoc =>
                            {
                                return Util.CalcLocationDistance(movableLoc, aliveLoc);
                            });

                            // "�����c���1�ԋ߂�����"��1�ԋ߂��ꏊ�����Ɏc��
                            if (aliveDist < tmpDist)
                            {
                                tmpList.Clear();
                                tmpList.Add(movableLoc);
                                tmpDist = aliveDist;
                            } else if(aliveDist == tmpDist)
                            {
                                tmpList.Add(movableLoc);
                            }
                        });

                        movableList.Clear();
                        movableList.AddRange(tmpList);
                    }

                    // ��₩�烉���_���őI��
                    var moveLoc = movableList[UnityEngine.Random.Range(0, movableList.Count)];

                    yield return MovePlayerLocation(moveLoc);
                    ImmediatePlayerMove();
                    UpdateCellColor();
                }
                break;
            case Constant.PlayerID.You:
                skillRange = Constant.SKILL_YOU_KILL_RANGE;
                UpdateCellColor(skillRange);
                state = Constant.GameState.SkillWait;
                break;
        }
    }

    /// <summary>
    /// �^�[���J�n
    /// </summary>
    private IEnumerator TurnStartBase()
    {
        gameTurn++;
        saveMaxHp = GetPlayerObject().GetComponent<CharacterScript>().GetHp();
        foreach (var row in fieldCellList)
            foreach (var objItem in row.Value)
            {
                var scr = objItem.Value.GetComponent<FieldCellScript>();
                scr.TurnStart();
            }

        yield return TurnStart();

        UpdateCellColor();

        state = Constant.GameState.Idle;
    }

    /// <summary>
    /// �^�[���I��
    /// </summary>
    private IEnumerator TurnEndBase()
    {
        foreach (var row in fieldCellList)
            foreach (var objItem in row.Value)
            {
                var scr = objItem.Value.GetComponent<FieldCellScript>();
                scr.TurnEnd();
            }

        yield return TurnEnd();
    }

    /// <summary>
    /// �h���p�^�[���J�n
    /// </summary>
    abstract protected IEnumerator TurnStart();

    /// <summary>
    /// �h���p�^�[���I��
    /// </summary>
    abstract protected IEnumerator TurnEnd();

    /// <summary>
    /// �h���V�[�̘A���U���@�P��
    /// </summary>
    /// <returns></returns>
    abstract protected IEnumerator DrowsAttackEnd();

    /// <summary>
    /// �^�[���o�߂����v���C���[���������ꍇ�̏���
    /// </summary>
    /// <returns></returns>
    virtual protected void ImmediatePlayerMove()
    {
    }

    #region ���j���[����
    /// <summary>
    /// ���j���[�J��
    /// </summary>
    public void OpenGameMenu()
    {
        if (state != Constant.GameState.Idle)
        {
            return;
        }

        var opt = Global.GetSaveData().option;
        chkOptionBattleSkip.isOn = (opt.skipEffect == 1);
        slOptionBGMVolume.value = opt.bgmVolume;
        slOptionSEVolume.value = opt.seVolume;

        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_OK);
        gameMenuUI.SetActive(true);
    }

    /// <summary>
    /// ���j���[����
    /// </summary>
    public void CloseGameMenu()
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_CANCEL);
        Global.GetSaveData().Save();
        gameMenuUI.SetActive(false);
    }

    /// <summary>
    /// �^�C�g���ɖ߂�
    /// </summary>
    public void BackToTitle()
    {
        ManagerScript.GetInstance().ChangeScene("TitleScene");
    }

    /// <summary>
    /// �퓬�X�L�b�v
    /// </summary>
    /// <param name="skip"></param>
    public void OptionBattleSkip(bool skip)
    {
        Global.GetSaveData().option.skipEffect = chkOptionBattleSkip.isOn ? 1 : 0;
    }

    /// <summary>
    /// BGM����
    /// </summary>
    /// <param name="vol"></param>
    public void OptionBgmVol(float vol)
    {
        Global.GetSaveData().option.bgmVolume = (int)slOptionBGMVolume.value;
        SoundManager.GetInstance().UpdateVol();
    }

    /// <summary>
    /// SE����
    /// </summary>
    /// <param name="vol"></param>
    public void OptionSEVol(float vol)
    {
        Global.GetSaveData().option.seVolume = (int)slOptionSEVolume.value;
    }
    #endregion
}
