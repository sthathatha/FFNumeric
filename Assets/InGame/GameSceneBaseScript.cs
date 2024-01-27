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
    /// <summary>カメラオブジェクト</summary>
    public Camera mainCamera;

    /// <summary>フィールド管理</summary>
    protected Dictionary<int, Dictionary<int, GameObject>> fieldCellList;

    /// <summary>ゲーム状態</summary>
    protected Constant.GameState state;

    /// <summary>記録用最大HP</summary>
    protected double saveMaxHp;
    /// <summary>ターン数</summary>
    protected int gameTurn;

    /// <summary>プレイヤーを保持</summary>
    protected GameObject playerCell;

    /// <summary>プレイヤーオブジェクトも保持</summary>
    protected GameObject playerObj = null;

    /// <summary>スキルゲージ１回のサイズ</summary>
    protected int skillGaugeLength = 0;
    /// <summary>スキルストック最大</summary>
    protected int skillStockMax = 0;
    /// <summary>スキルゲージ現在値</summary>
    protected int skillGaugeNow = 0;
    /// <summary>スキル使用可能数</summary>
    protected int skillCount = 0;
    /// <summary>スキル使用待機中の射程</summary>
    protected int skillRange = 0;

    /// <summary>スキルUI</summary>
    protected SkillUIScript skillUIScript = null;

    /// <summary>カーソルプレハブリソース</summary>
    private ResourceRequest cursorHandle;
    /// <summary>敵プレハブリソース</summary>
    private ResourceRequest enemyHandle;
    /// <summary>プレイヤープレハブリソース</summary>
    private ResourceRequest playerHandle;
    /// <summary>プレイヤーAnimatorリソース</summary>
    private ResourceRequest playerAnimHandle;

    /// <summary>攻撃エフェクトプレハブリソース</summary>
    private ResourceRequest attackEffectHandle;
    /// <summary>必殺技エフェクトプレハブリソース</summary>
    private ResourceRequest skillEffectHandle;
    /// <summary>スキル発動時エフェクトプレハブリソース</summary>
    private ResourceRequest skillGradEffectHandle;

    /// <summary>ゲームオーバー表示</summary>
    public GameObject gameoverFade;

    /// <summary>ゲーム内メニュー</summary>
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
    /// 敵クリック可能距離
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
    /// フィールド１マス作成
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    /// <param name="type">配置キャラクター</param>
    /// <param name="animResource">リソース</param>
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
            // すでに存在してる
            cellObj = fieldCellList[y][x];
            cellScr = cellObj.GetComponent<FieldCellScript>();

            // キャラがすでに居る
            if (cellScr.GetCharacterType() != Constant.CharacterType.Empty)
            {
                return;
            }

            // 空を維持する
            if (cellScr.IsEmptyTime())
            {
                return;
            }
        }
        else
        {
            // 存在しない場合作成
            cellObj = new GameObject();
            cellObj.AddComponent<FieldCellScript>();
            cellScr = cellObj.GetComponent<FieldCellScript>();
            // カーソル作成
            cellScr.CreateCursor(cursorHandle.asset as GameObject);

            // 位置設定
            cellObj.GetComponent<FieldCellScript>().SetLocation(x, y);
            fieldCellList[y].Add(x, cellObj);
        }


        // キャラクター設定
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
    /// セル全削除
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
    /// フィールドセルスクリプト取得
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
    /// キャラクタースクリプト取得
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
    /// キャラクターが存在するか確認
    /// </summary>
    /// <param name="loc">座標</param>
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
    /// プレイヤーの位置
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetPlayerLocation()
    {
        return playerCell.GetComponent<FieldCellScript>().GetLocation();
    }

    /// <summary>
    /// プレイヤーGameObject取得
    /// </summary>
    /// <returns></returns>
    public GameObject GetPlayerObject()
    {
        return playerObj;
    }

    /// <summary>
    /// 敵クリック時の処理
    /// </summary>
    /// <param name="enm"></param>
    public void EnemyClickEvent(GameObject enm)
    {
        StartCoroutine(ExecuteBattle(enm));
    }

    /// <summary>
    /// 敵クリック時のコルーチン
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

        // 向きを変える
        var distVec = enmCellScript.GetLocation() - GetPlayerLocation();
        var distance = Util.CalcLocationDistance(enmCellScript.GetLocation(), GetPlayerLocation());
        var isRight = distVec.x * 2 > distVec.y;
        plrScript.SetCharacterRight(isRight);
        if (enmCellScript.GetCharacterType() == Constant.CharacterType.Boss)
        {
            enmScript.SetCharacterRight(!isRight);
        }

        // 左右
        List<Vector2Int> lrList = null;

        // 貫通攻撃先
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
            // プレイヤー攻撃
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
                //エグザ 左右を攻撃
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
                //2回以上攻撃 （エグザの範囲は反映されない事に注意）

                if (rapidTargetScr.IsAlive())
                {
                    // 残っていたらそのまま
                    rapidTargetScr.Damage(pAtk);
                    if (!Global.GetSaveData().IsBattleSkip())
                    {
                        PlayAttackEffect(rapidTargetScr.GetCellScript().GetLocation(), isRight);
                    }
                }
                else if (distance == 1)
                {
                    // 死んでいたら貫通攻撃
                    // 直線の場合さらに進む
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
                    // 周囲のみ
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
                //エグザ 左右判定
                foreach (var lr in lrList)
                {
                    var target = GetPlayerLocation() + lr;
                    var lrScr = GetCharacterScript(target);
                    if (!lrScr) { continue; }
                    if (!lrScr.IsAlive())
                    {
                        //成長、死亡アニメーション
                        plrScript.AddHp(lrScr.GetMaxHp());
                        lrScr.GetCellScript().DeathCharacterObj();
                        yield return DestroyEnemy();
                    }
                }
            }

            if (!enmScript.IsAlive())
            {
                // 成長
                plrScript.AddHp(enmScript.GetMaxHp());

                // 敵の死亡アニメーション
                enmCellScript.DeathCharacterObj();
                yield return DestroyEnemy();

                if (Global.GetSaveData().IsBattleSkip())
                {
                    yield return new WaitForSeconds(0.4f);
                }

                // ウーラの貫通した敵
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

                // プレイヤーObjectを移動
                yield return MovePlayerLocation(enmCellScript.GetLocation());

                if (attackCount != 1 || Global.GetTemporaryData().selectPlayer != Constant.PlayerID.Drows)
                {
                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    // 一撃で倒した場合ドロシー 連続攻撃
                    var attacked = false;
                    for (int rushCount = 1; rushCount < Constant.SKILL_DROWS_ATTACK_LIMIT; ++rushCount)
                    {
                        yield return DrowsAttackEnd();

                        var rushNext = GetPlayerLocation() + distVec;
                        var rushEnemyScr = GetCharacterScript(rushNext);
                        if (!rushEnemyScr)
                        {
                            //奥が居ないと停止
                            break;
                        }

                        attacked = true;
                        rushEnemyScr.Damage(plrScript.GetAttackNum());
                        if (rushEnemyScr.IsAlive())
                        {
                            //倒せてないと停止
                            break;
                        }

                        // 連続撃破で成長率アップ
                        var upRate = 1.0 + 0.03 * rushCount;
                        plrScript.AddHp(rushEnemyScr.GetMaxHp() * upRate);

                        // 一撃で倒せる限り続行
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

            // 敵攻撃
            var eAtk = enmScript.GetAttackNum();
            // 距離によって減衰
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

                // フェード処理
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
    /// スキル待機中に敵クリックで実行
    /// </summary>
    /// <param name="enm">敵Object</param>
    /// <returns></returns>
    protected IEnumerator ExecuteSkillWait(GameObject enm)
    {
        skillCount--;
        UpdateSkillUI();
        state = Constant.GameState.Battle;
        var pScr = GetPlayerObject().GetComponent<PlayerScript>();
        var enmScript = enm.GetComponent<CharacterScript>();
        var enmCellScript = enmScript.GetCellScript();

        // 向きを変える
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
                // エグザ　１回攻撃してHP吸収
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
                // 悠　即死
                var addHp = enmScript.GetMaxHp();
                enmScript.Damage(addHp);
                pScr.AnimateAttack();
                PlayAttackEffect(enmCellScript.GetLocation(), isRight);
                yield return new WaitForSeconds(0.4f);

                // 成長
                pScr.AddHp(addHp);

                // 敵の死亡アニメーション
                enmCellScript.DeathCharacterObj();
                yield return DestroyEnemy();
                yield return new WaitForSeconds(0.2f);

                // プレイヤーObjectを移動
                yield return MovePlayerLocation(eLoc);
                yield return new WaitForSeconds(0.2f);

                ImmediatePlayerMove();
                UpdateCellColor();
                break;
        }
    }

    /// <summary>
    /// ゲームオーバー後クリックでタイトルに移動
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
    /// プレイ記録に保存
    /// </summary>
    protected virtual void SaveRecord()
    {
    }

    /// <summary>
    /// プレイヤー移動
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
    /// 攻撃エフェクト再生
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
    /// スキル発動時のグラデーションエフェクト
    /// </summary>
    protected void PlaySkillExecEffect()
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SKILL_EFFECT);

        var eff = Instantiate(skillGradEffectHandle.asset as GameObject);
        eff.transform.SetParent(playerObj.transform, false);
    }

    /// <summary>
    /// カーソル色更新
    /// </summary>
    /// <param name="_skillRange">スキル射程指定 -1の場合攻撃射程をグレー（デフォルト）</param>
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
    /// 敵を倒した時の処理
    /// </summary>
    protected IEnumerator DestroyEnemy()
    {
        // MAXでない場合 or AUTOの場合
        if (skillCount < skillStockMax || skillStockMax <= 0)
        {
            skillGaugeNow++;
            if (skillGaugeNow >= skillGaugeLength)
            {
                skillGaugeNow = 0;
                skillCount++;

                // AUTOの場合即発動
                if (skillStockMax <= 0)
                {
                    yield return ExecuteSkill();
                }
            }
        }

        UpdateSkillUI();
    }

    /// <summary>
    /// スキルUI更新
    /// </summary>
    protected void UpdateSkillUI()
    {
        skillUIScript.SetSkillState(skillGaugeNow, skillGaugeLength, skillCount, skillStockMax);
    }

    /// <summary>
    /// スキルUIクリック
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
    /// スキル実行
    /// </summary>
    protected IEnumerator ExecuteSkill()
    {
        if (skillCount <= 0) { yield break; }

        var pScr = playerObj.GetComponent<CharacterScript>();
        var pLoc = GetPlayerLocation();

        // キャラ毎の処理
        switch (Global.GetTemporaryData().selectPlayer)
        {
            case Constant.PlayerID.Drows:
                skillCount--;
                UpdateSkillUI();
                // ドロシー　一定ターン攻撃力増加
                pScr.SetSkillValid(Constant.SKILL_DROWS_KEEP_TURN);
                PlaySkillExecEffect();
                yield return new WaitForSeconds(0.2f);
                break;
            case Constant.PlayerID.Eraps:
                skillCount--;
                UpdateSkillUI();
                // エラ　シールド強化
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
                // ウーラ　射程４まで増加　以降は攻撃回数増加
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
                // エフェクト再生
                pScr.AnimateAttack();
                var eff = Instantiate(skillEffectHandle.asset as GameObject);
                eff.transform.position = Util.GetBasePosition(pLoc.x, pLoc.y);
                yield return new WaitForSeconds(0.2f);

                // 移動先候補
                var farLength = 0;
                var movableList = new List<Vector2Int>();
                // 倒せなかった奴
                var aliveList = new List<Vector2Int>();

                // クー　範囲内に定数倍ダメージ、反撃不可、成長不可
                for (int x = -Constant.SKILL_KOOB_RAGNAROK_RANGE; x <= Constant.SKILL_KOOB_RAGNAROK_RANGE; ++x)
                    for (int y = -Constant.SKILL_KOOB_RAGNAROK_RANGE; y <= Constant.SKILL_KOOB_RAGNAROK_RANGE; ++y)
                    {
                        // 位置と距離判定
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
                            // 倒してなかったら麻痺と成長不可
                            eScr.AddAttackInvalidCount(Constant.SKILL_KOOB_RAGNAROK_PARALYZE_TURN);
                            eScr.AddPowUpInvalidTurn(Constant.SKILL_KOOB_RAGNAROK_POW_INVALID_TURN);
                            aliveList.Add(loc);
                        }
                    }
                yield return new WaitForSeconds(1.2f);

                // 倒した場合
                if (farLength > 0)
                {
                    pScr.AddHp(addHp);

                    // 生き残りが居たら判定
                    if (aliveList.Count > 0)
                    {
                        var tmpDist = int.MaxValue;
                        var tmpList = new List<Vector2Int>();
                        movableList.ForEach(movableLoc =>
                        {
                            // 生き残りに1番近い距離を判定
                            var aliveDist = aliveList.Min(aliveLoc =>
                            {
                                return Util.CalcLocationDistance(movableLoc, aliveLoc);
                            });

                            // "生き残りに1番近い距離"が1番近い場所を候補に残す
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

                    // 候補からランダムで選ぶ
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
    /// ターン開始
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
    /// ターン終了
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
    /// 派生用ターン開始
    /// </summary>
    abstract protected IEnumerator TurnStart();

    /// <summary>
    /// 派生用ターン終了
    /// </summary>
    abstract protected IEnumerator TurnEnd();

    /// <summary>
    /// ドロシーの連続攻撃　１回毎
    /// </summary>
    /// <returns></returns>
    abstract protected IEnumerator DrowsAttackEnd();

    /// <summary>
    /// ターン経過せずプレイヤーが動いた場合の処理
    /// </summary>
    /// <returns></returns>
    virtual protected void ImmediatePlayerMove()
    {
    }

    #region メニュー処理
    /// <summary>
    /// メニュー開く
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
    /// メニュー閉じる
    /// </summary>
    public void CloseGameMenu()
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_CANCEL);
        Global.GetSaveData().Save();
        gameMenuUI.SetActive(false);
    }

    /// <summary>
    /// タイトルに戻る
    /// </summary>
    public void BackToTitle()
    {
        ManagerScript.GetInstance().ChangeScene("TitleScene");
    }

    /// <summary>
    /// 戦闘スキップ
    /// </summary>
    /// <param name="skip"></param>
    public void OptionBattleSkip(bool skip)
    {
        Global.GetSaveData().option.skipEffect = chkOptionBattleSkip.isOn ? 1 : 0;
    }

    /// <summary>
    /// BGM音量
    /// </summary>
    /// <param name="vol"></param>
    public void OptionBgmVol(float vol)
    {
        Global.GetSaveData().option.bgmVolume = (int)slOptionBGMVolume.value;
        SoundManager.GetInstance().UpdateVol();
    }

    /// <summary>
    /// SE音量
    /// </summary>
    /// <param name="vol"></param>
    public void OptionSEVol(float vol)
    {
        Global.GetSaveData().option.seVolume = (int)slOptionSEVolume.value;
    }
    #endregion
}
