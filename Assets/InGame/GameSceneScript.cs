using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class GameSceneScript : GameSceneBaseScript
{
    private Vector2Int bossLocation;
    private bool enableTutorial;
    private ResourceRequest bossResource = null;

    protected override IEnumerator Start()
    {
        switch (Global.GetTemporaryData().selectStage)
        {
            case 1:
                mainCamera.backgroundColor = Constant.COLOR_BG_DROWS;
                break;
            case 2:
                mainCamera.backgroundColor = Constant.COLOR_BG_ERAPS;
                break;
            case 3:
                mainCamera.backgroundColor = Constant.COLOR_BG_EXA;
                break;
            case 4:
                mainCamera.backgroundColor = Constant.COLOR_BG_WORRA;
                break;
            case 5:
                mainCamera.backgroundColor = Constant.COLOR_BG_KOOB;
                break;
            case 6:
                mainCamera.backgroundColor = Constant.COLOR_BG_YOU;
                break;
        }

        if (Global.GetTemporaryData().selectStage == Constant.LAST_STAGE)
        {
            bossResource = Resources.LoadAsync<RuntimeAnimatorController>("InGameScene/anim/you");
            yield return bossResource;
        }

        yield return base.Start();
    }

    protected override void InitGameSystem()
    {
        enableTutorial = true;
        switch (Global.GetTemporaryData().selectStage)
        {
            case 1:
                SoundManager.GetInstance().PlayBGM(SoundManager.BgmID.GAME_DROWS);
                break;
            case 2:
                SoundManager.GetInstance().PlayBGM(SoundManager.BgmID.GAME_ERAPS);
                break;
            case 3:
                SoundManager.GetInstance().PlayBGM(SoundManager.BgmID.GAME_EXA);
                break;
            case 4:
                SoundManager.GetInstance().PlayBGM(SoundManager.BgmID.GAME_WORRA);
                break;
            case 5:
                SoundManager.GetInstance().PlayBGM(SoundManager.BgmID.GAME_KOOB);
                break;
            case 6:
                SoundManager.GetInstance().PlayBGM(SoundManager.BgmID.GAME_YOU);
                break;
        }

        CreateStoryField();
        UpdateCellColor();
    }

    /// <summary>
    /// �폜��
    /// </summary>
    protected override void OnDestroy()
    {
        if (bossResource?.isDone == true)
        {
            Resources.UnloadAsset(bossResource.asset);
        }
        base.OnDestroy();
    }

    /// <summary>
    /// �͈̓t�B�[���h�S������
    /// </summary>
    private void CreateStoryField()
    {
        // �ʃN���X�ō쐬
        var list = StoryStageData.GetFieldList();
        foreach (var data in list)
        {
            CreateEnemyField(data.loc, data.initHp, data.type);
        }
    }

    /// <summary>
    /// �w��ʒu��Enemy����
    /// </summary>
    /// <param name="_loc"></param>
    /// <param name="_hp"></param>
    /// <param name="_type"></param>
    private void CreateEnemyField(Vector2Int _loc, double _hp, Constant.CharacterType _type = Constant.CharacterType.Enemy)
    {
        if (IsCharacterBeing(_loc)) { return; }

        if (_type == Constant.CharacterType.Boss && bossResource?.isDone == true)
        {
            CreateFieldCell(_loc.x, _loc.y, _type, bossResource.asset as RuntimeAnimatorController);
        }
        else
        {
            CreateFieldCell(_loc.x, _loc.y, _type);
        }

        var cellScr = GetFieldCellScript(_loc);
        cellScr.InitCharacterHp(_hp);
        cellScr.UpdateHPColor(GetPlayerObject().GetComponent<PlayerScript>());

        if (_type == Constant.CharacterType.Boss)
        {
            bossLocation = _loc;
        }
    }

    /// <summary>
    /// �^�[���J�n����
    /// </summary>
    override protected IEnumerator TurnStart()
    {
        // �J�n���`���[�g���A����b�e��
        switch (Global.GetTemporaryData().selectStage)
        {
            case 1:
                yield return DrowsStartTutorial();
                break;
            case 2:
                yield return ErapsStartTutorial();
                break;
            case 3:
                yield return ExaStartTutorial();
                break;
            case 4:
                yield return WorraStartTutorial();
                break;
            case 5:
                yield return KoobStartTutorial();
                break;
            case 6:
                yield return YouStartTutorial();
                break;
        }
    }

    /// <summary>
    /// �^�[���I������
    /// </summary>
    override protected IEnumerator TurnEnd()
    {
        // �{�X�����Ȃ�or�{�X�ʒu�Ƀv���C���[�������Ă���N���A
        if (IsCharacterBeing(bossLocation) && GetPlayerLocation() != bossLocation)
        {
            yield break;
        }

        // �N���A�t���O�Ɖ�b
        switch (Global.GetTemporaryData().selectStage)
        {
            case 1:
                yield return DrowsClearTutorial();
                Global.GetSaveData().stage1Clear = 1;
                break;
            case 2:
                yield return ErapsClearTutorial();
                Global.GetSaveData().stage2Clear = 1;
                break;
            case 3:
                yield return ExaClearTutorial();
                Global.GetSaveData().stage3Clear = 1;
                break;
            case 4:
                yield return WorraClearTutorial();
                Global.GetSaveData().stage4Clear = 1;
                break;
            case 5:
                yield return KoobClearTutorial();
                Global.GetSaveData().stage5Clear = 1;
                break;
            case 6:
                yield return YouClearTutorial();
                Global.GetSaveData().stage6Clear = 1;
                break;
        }
        Global.GetSaveData().Save();

        yield return new WaitForSeconds(1f);
        ManagerScript.GetInstance().ChangeScene("TitleScene");
    }

    protected override IEnumerator DrowsAttackEnd()
    {
        yield break;
    }

    #region ��b�@�h���V�[�p
    /// <summary>
    /// �h���V�[�p�`���[�g���A����b
    /// </summary>
    private IEnumerator DrowsStartTutorial()
    {
        if (!enableTutorial) yield break;

        var ui = TalkUI.GetInstance();
        var pPos = GetPlayerLocation();
        var error = false;
        if (gameTurn == 1)
        {
            ui.Open();

            ui.SetMessage("", "�͈͓��̑Ώۂ��^�b�v���čU�����܂�");
            yield return ui.WaitForClick();
            ui.Close();
        }
        else if (gameTurn == 2)
        {
            ui.Open();

            ui.SetMessage("", "�{�X�ȊO�̓G�̓^�[�����ɐ������Ă����܂����A�h���V�[�͂���ɑ΍R���鋭�͂Ȍ��������Ă��܂�");
            yield return ui.WaitForClick();
            ui.SetMessage("", "�����ĉE�ɐi��ł݂܂��傤");
            yield return ui.WaitForClick();
            ui.Close();
        }
        else if (gameTurn == 3)
        {
            if (pPos != new Vector2Int(4, 0))
            {
                error = true;
            }
            else
            {
                ui.Open();

                ui.SetMessage("", "�h���V�[�̓����X�^�[���ꌂ�œ|�������A�����I�ɓ����^�[�����ł܂������i�ݑ����܂��B");
                yield return ui.WaitForClick();
                ui.SetMessage("", "�Ȃ��X�g�[���[���[�h�ł͓G�����������A�܂��ɓG�����Ȃ��Ȃ�Ɠ����Ȃ��Ȃ��Ă��܂����ߒ��ӂ��Ă��������B");
                yield return ui.WaitForClick();
                ui.SetMessage("", "�l�񂾏ꍇ�͉E��̃��j���[����^�C�g����ʂɖ߂邱�Ƃ��ł��܂��B");
                yield return ui.WaitForClick();
                ui.SetMessage("", "����ł́A���͉E���ɐi��ł݂܂��傤");
                yield return ui.WaitForClick();
                ui.Close();
            }
        }
        else if (gameTurn == 4)
        {
            if (pPos != new Vector2Int(4, -2))
            {
                error = true;
            }
            else
            {
                ui.Open();

                ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true, true);
                ui.SetMessage("�h���V�[", "�����H\n����������");
                yield return ui.WaitForClick();

                ui.RemoveCharacterPic(TalkUI.PicturePos.Left);
                ui.SetMessage("", "���ɂ��ǉ��U���ł́A�|���Ȃ������ꍇ�ł��퓬�ƂȂ炸�Ƀ^�[�����I������̂ň��S�ł��B");
                yield return ui.WaitForClick();

                ui.SetMessage("", "�{�X�̍����X���C����|���ƃX�e�[�W�N���A�ƂȂ�܂��B");
                yield return ui.WaitForClick();
                ui.Close();
            }
        }else if(gameTurn == 5)
        {
            ui.Open();

            ui.SetMessage("", "�G��|���ƉE���̃Q�[�W�����܂��Ă����A��t�ɂȂ�ƃ^�b�v���ăX�L���𔭓����邱�Ƃ��ł��܂��B");
            yield return ui.WaitForClick();

            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true, true);
            ui.SetMessage("�h���V�[", "�S�R���܂�ˁ[�����");
            yield return ui.WaitForClick();

            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true);
            ui.SetMessage("", "�X�g�[���[���[�h�ł͗��܂肫��Ȃ����߁A�����G���h���X���[�h��p�ƂȂ�܂��B");
            yield return ui.WaitForClick();

            ui.SetMessage("", "�X�L���̌��ʂ̓L�����N�^�[���X�g��ʂŊm�F���邱�Ƃ��ł��܂��B");
            yield return ui.WaitForClick();
            ui.Close();
        }

        if (error)
        {
            enableTutorial = false;
            ui.Open();
            ui.SetMessage("", "������ƁI�\��ƈႤ���Ƃ��Ȃ��ł���������");
            yield return ui.WaitForClick();

            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true, true);
            ui.SetMessage("�h���V�[", "�Ȃ񂾂�ʂɂ��������");
            yield return ui.WaitForClick();

            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true);
            ui.SetMessage("", "���[���I�`���[�g���A���������Ǝv���Ă��ł����I");
            yield return ui.WaitForClick();
            ui.SetMessage("", "�����m���I����ɂ��Ȃ����I");
            yield return ui.WaitForClick();

            ui.SetMessage("�h���V�[", "�c�c");
            yield return ui.WaitForClick();

            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Drows, TalkUI.PictureType.Sad, true, true);
            ui.SetMessage("�h���V�[", "�N������");
            yield return ui.WaitForClick();
            ui.Close();
        }
    }

    /// <summary>
    /// �h���V�[�N���A��b
    /// </summary>
    /// <returns></returns>
    private IEnumerator DrowsClearTutorial()
    {
        var ui = TalkUI.GetInstance();

        ui.Open();

        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, false, true);
        ui.SetMessage("�h���V�[", "�܁A����Ȃ���\n���ǂ��s���΂����񂾂�����");
        yield return ui.WaitForClick();

        ui.SetMessage("�h���V�[", "�K���ɕ����Ă���Ȃ񂩂��邩��");
        yield return ui.WaitForClick();

        ui.Close();
    }
    #endregion

    #region ��b�@�G���p
    /// <summary>
    /// �G���p�`���[�g���A����b
    /// </summary>
    private IEnumerator ErapsStartTutorial()
    {
        var ui = TalkUI.GetInstance();
        if (gameTurn == 1)
        {
            ui.Open();

            ui.SetMessage("", "�G���̓^�[���J�n���ƓG�̌��j���ɃV�[���h�𐶐����܂��B");
            yield return ui.WaitForClick();
            ui.SetMessage("", "�����ʂ͍ŏ��͂킸���ł����A���������̃X�L���ŏ㏸���Ă����܂��B");
            yield return ui.WaitForClick();
            ui.Close();
        }
    }

    /// <summary>
    /// �G���N���A��b
    /// </summary>
    /// <returns></returns>
    private IEnumerator ErapsClearTutorial()
    {
        var ui = TalkUI.GetInstance();

        ui.Open();

        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Eraps, TalkUI.PictureType.Sad, false, true);
        ui.SetMessage("�G��", "����́A�v�����ȏ�ɑ�ς������c\n�����݂Ȃ���ƍ������Ȃ���");
        yield return ui.WaitForClick();

        ui.Close();
    }
    #endregion

    #region ��b�@�G�O�U�p
    /// <summary>
    /// �G�O�U�p�`���[�g���A����b
    /// </summary>
    private IEnumerator ExaStartTutorial()
    {
        var ui = TalkUI.GetInstance();
        if (gameTurn == 1)
        {
            ui.Open();

            ui.SetMessage("", "�G�O�U�͍U�����A�Ώۂ̍��E�ɂ������x�̃_���[�W��^���܂��B");
            yield return ui.WaitForClick();
            ui.SetMessage("", "���E�̓G��|�����ꍇ���������܂����A���̏ꏊ�ɂ͈ړ����܂���B");
            yield return ui.WaitForClick();
            ui.Close();
        }
    }

    /// <summary>
    /// �G�O�U�N���A��b
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExaClearTutorial()
    {
        var ui = TalkUI.GetInstance();

        ui.Open();

        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, false, true);
        ui.SetMessage("�G�O�U", "�����������̃{�X����\n���āA������");
        yield return ui.WaitForClick();

        ui.Close();
    }
    #endregion

    #region ��b�@�E�[���p
    /// <summary>
    /// �E�[���p�`���[�g���A����b
    /// </summary>
    private IEnumerator WorraStartTutorial()
    {
        var ui = TalkUI.GetInstance();
        if (gameTurn == 1)
        {
            ui.Open();

            ui.SetMessage("", "�E�[���͂Q�A���ōU�����A�Q�{�̃_���[�W��^���܂��B");
            yield return ui.WaitForClick();
            ui.SetMessage("", "�r���œG��|�����ꍇ�A�����O�̒l�ł���ɉ��̓G�Ƀ_���[�W��^���܂��B");
            yield return ui.WaitForClick();
            ui.SetMessage("", "���̓G��|���Ă��������܂����A���̏ꏊ�ɂ͈ړ����܂���B");
            yield return ui.WaitForClick();
            ui.Close();
        }
    }

    /// <summary>
    /// �E�[���N���A��b
    /// </summary>
    /// <returns></returns>
    private IEnumerator WorraClearTutorial()
    {
        var ui = TalkUI.GetInstance();

        ui.Open();

        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Worra, TalkUI.PictureType.Sad, false, true);
        ui.SetMessage("�E�[��", "�͂��A���ȏ��ɗ�����������\n�����I��点�܂��傤");
        yield return ui.WaitForClick();

        ui.Close();
    }
    #endregion

    #region ��b�@�N�[�p
    /// <summary>
    /// �N�[�p�`���[�g���A����b
    /// </summary>
    private IEnumerator KoobStartTutorial()
    {
        var ui = TalkUI.GetInstance();
        if (gameTurn == 1)
        {
            ui.Open();

            ui.SetMessage("", "�N�[�͖��@�ɂ�苗��2�}�X�܂ł̓G���U���ł��܂��B");
            yield return ui.WaitForClick();

            ui.SetMessage("", "���ꂽ�G�Ɛ키���́A�����ł�����_���[�W���啝�Ɍy������܂��B");
            yield return ui.WaitForClick();

            ui.Close();
        }else if (gameTurn == 2)
        {
            ui.Open();

            ui.SetMessage("", "�N�[�͓G��|�������ɒʏ���傫���������܂��B");
            yield return ui.WaitForClick();

            ui.Close();
        }
    }

    /// <summary>
    /// �N�[�N���A��b
    /// </summary>
    /// <returns></returns>
    private IEnumerator KoobClearTutorial()
    {
        var ui = TalkUI.GetInstance();

        ui.Open();

        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, false, true);
        ui.SetMessage("�N�[", "�ςȃ��[���̐��E�����ǂ�����������������");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Laugh, false, true);
        ui.SetMessage("�N�[", "���������̏��ɂ������I");
        yield return ui.WaitForClick();

        ui.Close();
    }
    #endregion

    #region ��b�@�I�p
    /// <summary>
    /// �I�p�`���[�g���A����b
    /// </summary>
    private IEnumerator YouStartTutorial()
    {
        var ui = TalkUI.GetInstance();
        if (gameTurn == 1)
        {
            ui.Open();

            ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true, true);
            ui.SetMessage("�h���V�[", "���̂��[�I");
            yield return ui.WaitForClick();
            ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true);
            ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, true, true);
            ui.SetMessage("�G��", "���ꂶ��Ȃ���ł���");
            yield return ui.WaitForClick();
            ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, true);
            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Worra, TalkUI.PictureType.Normal, true, true);
            ui.SetMessage("�E�[��", "�݂�ȁA���ꌩ��");
            yield return ui.WaitForClick();
            ui.Close();
            yield return new WaitForSeconds(1f);

            ui.Open();
            ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Sad, true, true);
            ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.You, TalkUI.PictureType.Normal, false);
            ui.SetMessage("�N�[", "�I�����A�����Ă�́H");
            yield return ui.WaitForClick();
            ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Sad, true);
            ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.You, TalkUI.PictureType.Normal, false, true);
            ui.SetMessage("�I", "�Ȃ񂩖�����������琒�q���ꂿ����āA�V���������ɂȂ����݂���");
            yield return ui.WaitForClick();
            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Sad, true, true);
            ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.You, TalkUI.PictureType.Normal, false);
            ui.SetMessage("�G�O�U", "�Ȃ����݂���\n����ˁ[��o�J�^��");
            yield return ui.WaitForClick();
            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Sad, true);
            ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.You, TalkUI.PictureType.Sp1, false, true);
            ui.SetMessage("�I", "�ӂ͂͂͂́I�悭�����Ȑl�Ԃǂ�");
            yield return ui.WaitForClick();
            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Sad, true, true);
            ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.You, TalkUI.PictureType.Sp1, false);
            ui.SetMessage("�G�O�U", "���邹��");
            yield return ui.WaitForClick();
            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Sad, true);
            ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Drows, TalkUI.PictureType.Sad, true, true);
            ui.SetMessage("�h���V�[", "�Ԃ񉣂��Ă������ƘA��ċA�邼");
            yield return ui.WaitForClick();
            ui.Close();
        }
    }

    /// <summary>
    /// �I�N���A��b
    /// </summary>
    /// <returns></returns>
    private IEnumerator YouClearTutorial()
    {
        var ui = TalkUI.GetInstance();

        ui.Open();

        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.You, TalkUI.PictureType.Sad, false, true);
        ui.SetMessage("�I", "����������");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.You, TalkUI.PictureType.Normal, false);
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, true, true);
        ui.SetMessage("�G��", "�ق�A��܂���");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true, true);
        ui.SetMessage("�N�[", "���̐��E�͂������v��������");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Worra, TalkUI.PictureType.Normal, true, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true);
        ui.SetMessage("�E�[��", "����������ˁA6�X�e�[�W���炢�������������悤�ȋC�������");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, true, true);
        ui.SetMessage("�G�O�U", "�X�e�[�W���ĉ�����");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Laugh, false, true);
        ui.SetMessage("�N�[", "�������Ƃ�邾���̃Q�[���ŃX�e�[�W�����ς�����Ă����傤���Ȃ�����");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, false, true);
        ui.SetMessage("�N�[", "���Ƃ̓G���h���X���[�h�ł��V�ׂ΂����񂶂�Ȃ�����");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, false);
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true, true);
        ui.SetMessage("�h���V�[", "�����V�΂��邽�߂̈琬�v�f�݂����Ȃ͖̂����̂�");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, false, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true);
        ui.SetMessage("�N�[", "����ȃQ�[���ɐl���̋M�d�Ȏ��Ԏg�����A���ɍs�����ق��������Ǝv��");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, false);
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Sad, true, true);
        ui.SetMessage("�G�O�U", "���O����c");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, true, true);
        ui.SetMessage("�G�O�U", "�܂�������");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.You, TalkUI.PictureType.Normal, false, true);
        ui.SetMessage("�I", "�܂������ւ��傢�R�L���Q�[�������������Ă݂悤��");
        yield return ui.WaitForClick();

        ui.Close();
    }
    #endregion
}
