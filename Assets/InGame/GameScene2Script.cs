using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class GameScene2Script : GameSceneBaseScript
{
    public GameObject clearFade;
    public TMP_Text txtClearTurn;
    public GameObject txtNewRecord;
    public Image imgClearCharacter;

    private ResourceRequest clearCharacterResource;

    protected override IEnumerator Start()
    {
        switch (Global.GetTemporaryData().selectPlayer)
        {
            case Constant.PlayerID.Drows:
                mainCamera.backgroundColor = Constant.COLOR_BG_DROWS;
                break;
            case Constant.PlayerID.Eraps:
                mainCamera.backgroundColor = Constant.COLOR_BG_ERAPS;
                break;
            case Constant.PlayerID.Exa:
                mainCamera.backgroundColor = Constant.COLOR_BG_EXA;
                break;
            case Constant.PlayerID.Worra:
                mainCamera.backgroundColor = Constant.COLOR_BG_WORRA;
                break;
            case Constant.PlayerID.Koob:
                mainCamera.backgroundColor = Constant.COLOR_BG_KOOB;
                break;
            case Constant.PlayerID.You:
                mainCamera.backgroundColor = Constant.COLOR_BG_YOU;
                break;
        }

        yield return base.Start();
    }

    protected override void InitGameSystem()
    {
        var resourceName = Constant.GetPlayerNameE(Global.GetTemporaryData().selectPlayer);
        switch (Global.GetTemporaryData().selectPlayer)
        {
            case Constant.PlayerID.Drows:
                SoundManager.GetInstance().PlayBGM(SoundManager.BgmID.GAME_DROWS);
                break;
            case Constant.PlayerID.Eraps:
                SoundManager.GetInstance().PlayBGM(SoundManager.BgmID.GAME_ERAPS);
                break;
            case Constant.PlayerID.Exa:
                SoundManager.GetInstance().PlayBGM(SoundManager.BgmID.GAME_EXA);
                break;
            case Constant.PlayerID.Worra:
                SoundManager.GetInstance().PlayBGM(SoundManager.BgmID.GAME_WORRA);
                break;
            case Constant.PlayerID.Koob:
                SoundManager.GetInstance().PlayBGM(SoundManager.BgmID.GAME_KOOB);
                break;
            case Constant.PlayerID.You:
                SoundManager.GetInstance().PlayBGM(SoundManager.BgmID.GAME_YOU);
                break;
        }
        clearCharacterResource = Resources.LoadAsync<Sprite>("CharacterPic/" + resourceName + "/000_p");

        CreateAllField();
        UpdateCellColor();
    }

    /// <summary>
    /// �폜��
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (clearCharacterResource?.isDone == true)
        {
            Resources.UnloadAsset(clearCharacterResource.asset);
        }
    }

    /// <summary>
    /// �͈̓t�B�[���h�S������
    /// </summary>
    private void CreateAllField()
    {
        // HP�����Ɏg�p���邽�ߍŏ��Ɏ��͂̂U�ӏ�����
        var plrLoc = GetPlayerLocation();
        var aroundLocs = Util.GetAroundLocations(plrLoc);
        foreach (var around in aroundLocs)
        {
            CreateEnemyField(around);
        }

        // �c��
        for (int y = -Constant.ENDLESS_FIELD_Y; y <= Constant.ENDLESS_FIELD_Y; ++y)
            for (int x = -Constant.ENDLESS_FIELD_X + y / 2; x <= Constant.ENDLESS_FIELD_X + y / 2; ++x)
            {
                CreateEnemyField(plrLoc + new Vector2Int(x, y));
            }
    }

    /// <summary>
    /// X���̎��E��������
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <returns></returns>
    private bool IsInVisibleField(int _x, int _y)
    {
        var plrLoc = GetPlayerLocation();
        var dist = new Vector2Int(_x, _y) - plrLoc;

        if (dist.x < -Constant.ENDLESS_FIELD_X + dist.y / 2 ||
            dist.x > Constant.ENDLESS_FIELD_X + dist.y / 2)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// ���E�̊O�̃t�B�[���h������
    /// </summary>
    private void DeleteFarField()
    {
        var plrLoc = GetPlayerLocation();

        var deleteY = new List<int>();
        var deleteCell = new List<Vector2Int>();

        foreach (var row in fieldCellList)
        {
            if (row.Key < plrLoc.y - Constant.ENDLESS_FIELD_Y ||
                row.Key > plrLoc.y + Constant.ENDLESS_FIELD_Y)
            {
                foreach (var cellObj in row.Value)
                {
                    Destroy(fieldCellList[row.Key][cellObj.Key]);
                }
                deleteY.Add(row.Key);
                continue;
            }

            foreach (var cellObj in row.Value)
            {
                if (!IsInVisibleField(cellObj.Key, row.Key))
                {
                    Destroy(fieldCellList[row.Key][cellObj.Key]);
                    deleteCell.Add(new Vector2Int(cellObj.Key, row.Key));
                }
            }
        }

        foreach (var y in deleteY)
        {
            fieldCellList.Remove(y);
        }

        foreach (var cell in deleteCell)
        {
            fieldCellList[cell.y].Remove(cell.x);
        }
    }

    /// <summary>
    /// �w��ʒu��Enemy����
    /// </summary>
    /// <param name="_loc"></param>
    private void CreateEnemyField(Vector2Int _loc)
    {
        if (IsCharacterBeing(_loc)) { return; }

        CreateFieldCell(_loc.x, _loc.y, Constant.CharacterType.Enemy);

        var cellScr = GetFieldCellScript(_loc);
        cellScr.InitCharacterHp(CalcNewEnemyHp(_loc));
        cellScr.UpdateHPColor(GetPlayerObject().GetComponent<PlayerScript>());
    }

    /// <summary>
    /// �V�K�����G�̏���HP���v�Z
    /// </summary>
    /// <param name="_enemyLoc">�������W</param>
    /// <returns></returns>
    protected double CalcNewEnemyHp(Vector2Int _enemyLoc)
    {
        // ��{�l �v���C���[�̎��͂U�}�X�̕��ϒl
        double baseValue = 0;
        var aroundCount = 0;
        var plrLoc = GetPlayerLocation();
        var aroundLocs = Util.GetAroundLocations(plrLoc);
        foreach (var around in aroundLocs)
        {
            if (around == _enemyLoc) { continue; }

            if (!IsCharacterBeing(around)) { continue; }

            aroundCount++;
            baseValue += GetCharacterScript(around).GetMaxHp();
        }
        if (aroundCount == 0)
        {
            // ���݂��Ȃ��ꍇ�̓v���C���[HP
            baseValue = GetCharacterScript(plrLoc).GetHp();
        }
        else
        {
            baseValue /= aroundCount;
        }

        bool isHard = Global.GetTemporaryData().difficulty == Constant.Difficulty.Hard;
        // �v���C���[�Ƃ̋������P����閈�ɂQ�{
        var distance = Util.CalcLocationDistance(plrLoc, _enemyLoc);
        // �{�� ^ (����-1)
        var distMultiValue = System.Math.Pow(isHard ? 1.9 : 1.7, distance - 1);

        //�o�����X����
        // �{���ɗ������܂߂�
        var randMin = isHard ? 1f : 0.9f;
        var randMax = isHard ? 1.2f : 1.15f;
        // �����Ƀ^�[�������e��
        if (gameTurn > 5)
        {
            randMax += 2.0f * (gameTurn - 5) / (isHard ? 450.0f : 750.0f);
        }
        distMultiValue *= Random.Range(randMin, randMax);

        return Util.FixHpValue(baseValue * distMultiValue);
    }

    /// <summary>
    /// �^�[���J�n����
    /// </summary>
    override protected IEnumerator TurnStart()
    {
        yield break;
    }

    /// <summary>
    /// �^�[���I������
    /// </summary>
    override protected IEnumerator TurnEnd()
    {
        CreateAllField();
        DeleteFarField();

        // ����ɒB���Ă�����N���A
        var plrScr = GetPlayerObject().GetComponent<CharacterScript>();
        if (plrScr.GetHp() >= Constant.HP_LIMIT)
        {
            state = Constant.GameState.GameClear;

            var talk = TalkUI.GetInstance();

            yield return clearCharacterResource;
            imgClearCharacter.sprite = clearCharacterResource.asset as Sprite;
            txtClearTurn.SetText(gameTurn.ToString());
            // NewRecord�\������
            CheckNewRecord();

            // ����N���A�ŉ�b�o��
            if (Global.GetSaveData().endlessClear == 0)
            {
                yield return ClearTalk();
                Global.GetSaveData().endlessClear = 1;
            }

            // �L�^�Z�[�u
            SaveRecord();

            //���U���g��ʃt�F�[�h
            var fadeGroup = clearFade.GetComponent<CanvasGroup>();
            fadeGroup.alpha = 0f;
            clearFade.SetActive(true);
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

            yield return new WaitForSeconds(1f);

            talk.Open();
            switch (Global.GetTemporaryData().selectPlayer)
            {
                case Constant.PlayerID.Drows:
                    talk.SetMessage("", "����ς�I�����ŋ��������ȁI");
                    break;
                case Constant.PlayerID.Eraps:
                    talk.SetMessage("", "���̗͂ŁA��肫�ꂽ�ł��傤��");
                    break;
                case Constant.PlayerID.Exa:
                    talk.SetMessage("", "���Ȃ�N�ɂ�������C�����Ȃ���");
                    break;
                case Constant.PlayerID.Worra:
                    talk.SetMessage("", "����ȂƂ���ŕ�����킯�ɂ͂����Ȃ���");
                    break;
                case Constant.PlayerID.Koob:
                    talk.SetMessage("", "���̐��E�̎��͂��������킩���Ă�����");
                    break;
                case Constant.PlayerID.You:
                    talk.SetMessage("", "�A���ăQ�[�����邺");
                    break;
            }
            yield return talk.WaitForClick();
            talk.Close();
            yield return new WaitForSeconds(0.5f);
            ManagerScript.GetInstance().ChangeScene("TitleScene");
        }
    }

    /// <summary>
    /// �^�[���I���ȊO�Ńv���C���[���������u��
    /// </summary>
    protected override void ImmediatePlayerMove()
    {
        base.ImmediatePlayerMove();

        CreateAllField();
        DeleteFarField();
    }

    /// <summary>
    /// �h���V�[�A���U���@�I����
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator DrowsAttackEnd()
    {
        CreateAllField();

        yield break;
    }

    /// <summary>
    /// �v���C�L�^�ۑ�
    /// </summary>
    protected override void SaveRecord()
    {
        base.SaveRecord();

        var isNormal = Global.GetTemporaryData().difficulty == Constant.Difficulty.Normal;
        var maxHp = GetPlayerObject().GetComponent<CharacterScript>().GetHp();
        if (maxHp < saveMaxHp)
        {
            maxHp = saveMaxHp;
        }
        var turn = gameTurn;
        Global.SaveData.EndlessRecord record;

        switch (Global.GetTemporaryData().selectPlayer)
        {
            case Constant.PlayerID.Drows: record = Global.GetSaveData().recordDrows; break;
            case Constant.PlayerID.Eraps: record = Global.GetSaveData().recordEraps; break;
            case Constant.PlayerID.Exa: record = Global.GetSaveData().recordExa; break;
            case Constant.PlayerID.Worra: record = Global.GetSaveData().recordWorra; break;
            case Constant.PlayerID.Koob: record = Global.GetSaveData().recordKoob; break;
            default: record = Global.GetSaveData().recordYou; break;
        }

        if (isNormal)
        {
            if (record.normalMaxHp < maxHp)
            {
                record.normalMaxHp = maxHp;
                record.normalTurn = turn;
            } else if (maxHp >= Constant.HP_LIMIT)
            {
                record.normalMaxHp = Constant.HP_LIMIT;
                if (record.normalTurn > turn)
                {
                    record.normalTurn = turn;
                }
            }
        }else
        {
            if (record.hardMaxHp < maxHp)
            {
                record.hardMaxHp = maxHp;
                record.hardTurn = turn;
            }
            else if (maxHp >= Constant.HP_LIMIT)
            {
                record.hardMaxHp = Constant.HP_LIMIT;
                if (record.hardTurn > turn)
                {
                    record.hardTurn = turn;
                }
            }
        }

        switch (Global.GetTemporaryData().selectPlayer)
        {
            case Constant.PlayerID.Drows: Global.GetSaveData().recordDrows = record; break;
            case Constant.PlayerID.Eraps: Global.GetSaveData().recordEraps = record; break;
            case Constant.PlayerID.Exa: Global.GetSaveData().recordExa = record; break;
            case Constant.PlayerID.Worra: Global.GetSaveData().recordWorra = record; break;
            case Constant.PlayerID.Koob: Global.GetSaveData().recordKoob = record; break;
            default: Global.GetSaveData().recordYou = record; break;
        }

        Global.GetSaveData().Save();
    }

    /// <summary>
    /// NewRecord�\������
    /// </summary>
    protected void CheckNewRecord()
    {
        var isNormal = Global.GetTemporaryData().difficulty == Constant.Difficulty.Normal;

        Global.SaveData.EndlessRecord record;
        switch (Global.GetTemporaryData().selectPlayer)
        {
            case Constant.PlayerID.Drows: record = Global.GetSaveData().recordDrows; break;
            case Constant.PlayerID.Eraps: record = Global.GetSaveData().recordEraps; break;
            case Constant.PlayerID.Exa: record = Global.GetSaveData().recordExa; break;
            case Constant.PlayerID.Worra: record = Global.GetSaveData().recordWorra; break;
            case Constant.PlayerID.Koob: record = Global.GetSaveData().recordKoob; break;
            default: record = Global.GetSaveData().recordYou; break;
        }

        var isNewRecord = false;
        if (isNormal)
        {
            if (record.normalMaxHp < Constant.HP_LIMIT)
            {
                isNewRecord = true;
            }
            else if (record.normalTurn > gameTurn)
            {
                isNewRecord = true;
            }
        }
        else
        {
            if (record.hardMaxHp < Constant.HP_LIMIT)
            {
                isNewRecord = true;
            }
            else if (record.hardTurn > gameTurn)
            {
                isNewRecord = true;
            }
        }

        txtNewRecord.SetActive(isNewRecord);
    }
    
    /// <summary>
    /// ���N���A���̉�b
    /// </summary>
    /// <returns></returns>
    protected IEnumerator ClearTalk()
    {
        var talk = TalkUI.GetInstance();

        talk.Open();
        talk.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true, true);
        talk.SetMessage("�N�[", "�܂��ł��c");
        yield return talk.WaitForClick();
        talk.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true);
        talk.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true, true);
        talk.SetMessage("�h���V�[", "�ǂ������H");
        yield return talk.WaitForClick();
        talk.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, false, true);
        talk.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true);
        talk.SetMessage("�N�[", "�܂�������Ȃɑ�������Ƃ͎v��Ȃ��������炳\n�������ɂ���ȏ�̐��l�͊Ǘ��ł��Ȃ����炱���ŃN���A�����Ŋ��ق��Ă��炨��");
        yield return talk.WaitForClick();
        talk.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, false);
        talk.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, true, true);
        talk.SetMessage("�G��", "�G���h���X���[�h�Ȃ̂ɏI����ł���");
        yield return talk.WaitForClick();
        talk.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.You, TalkUI.PictureType.Normal, true, true);
        talk.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, true);
        talk.SetMessage("�I", "���������[");
        yield return talk.WaitForClick();
        talk.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.You, TalkUI.PictureType.Normal, true);
        talk.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Worra, TalkUI.PictureType.Normal, true, true);
        talk.SetMessage("�E�[��", "�����Q�[���Ȃ̂�");
        yield return talk.WaitForClick();
        talk.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, false, true);
        talk.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Worra, TalkUI.PictureType.Normal, true);
        talk.SetMessage("�N�[", "���̖���RPG�̃X�[�p�[�W�����v��100��őł��~�߂�����");
        yield return talk.WaitForClick();
        talk.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, false);
        talk.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, true, true);
        talk.SetMessage("�G�O�U", "����Switch�Ń����C�N�o�����ǂǂ��Ȃ�����");
        yield return talk.WaitForClick();
        talk.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Koob, TalkUI.PictureType.Laugh, false, true);
        talk.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, true);
        talk.SetMessage("�N�[", "����͎����̖ڂŊm���݂Ă݂�I");
        yield return talk.WaitForClick();
        talk.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Koob, TalkUI.PictureType.Laugh, false);
        talk.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Drows, TalkUI.PictureType.Sad, true, true);
        talk.SetMessage("�h���V�[", "��������");
        yield return talk.WaitForClick();
        talk.Close();
    }
}
