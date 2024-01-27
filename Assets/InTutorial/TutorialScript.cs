using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    public GameObject image1;
    public GameObject image2;
    public GameObject image3;
    public GameObject image4;

    // Start is called before the first frame update
    public IEnumerator Start()
    {
        var ui = TalkUI.GetInstance();
        yield return ui.IsReady();

        ui.Open();
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Laugh, false, true);
        ui.SetMessage("�N�[", "��[��������|�����I");

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Laugh, false);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Eraps, TalkUI.PictureType.Sad, false, true);
        ui.SetMessage("�G��", "���������܂��v�����[�O�n�܂��ĂQ�b�ł���");

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Eraps, TalkUI.PictureType.Sad, false);
        ui.SetMessage("�N�[", "����Ȃւ��傢�Q�[���ɃX�g�[���[�Ƃ������Ă������񂶂�Ȃ�");

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, false);
        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, false, true);
        ui.SetMessage("�G�O�U", "�ւ��傢���ĉ�����");

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, false);
        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, false);
        ui.SetMessage("�N�[", "�Ƃ����킯�ł��̐��E�̃��[�����m�F�����");

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Sp1, true, true);
        ui.SetMessage("�N�[", "���ꂾ�I�o���b");
        image1.SetActive(true);

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Sp1, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Worra, TalkUI.PictureType.Laugh, false, true);
        ui.SetMessage("�E�[��", "�������Ƃ���`");

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, false, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Worra, TalkUI.PictureType.Normal, false);
        ui.SetMessage("�G��", "�m���ɂւ��傢�����c");

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, false);
        ui.SetMessage("�N�[", "�ꉞ������Ƃ��������ڂƈႤ���[���ɂ��ĂāA�퓬������������ŏ��ɍU�����ă_���[�W��^�����");
        image1.SetActive(false);
        image2.SetActive(true);

        yield return ui.WaitForClick();

        ui.SetMessage("�N�[", "�������瑊�肪�������Č��݂Ɂc�ŁA�O�ɂ����ق�������");
        image2.SetActive(false);
        image3.SetActive(true);

        yield return ui.WaitForClick();

        ui.SetMessage("�N�[", "���������܂Ő���ĂP�^�[���I���Ƃ��܂�");
        image3.SetActive(false);
        image4.SetActive(true);

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, false, true);
        ui.SetMessage("�G�O�U", "�V�X�e���ケ����������邩�炩�Ȃ�L������");
        image4.SetActive(false);

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, false);
        ui.SetMessage("�N�[", "�v�Z�ł́@x^2 < x + 1�@�𖞂����悤��x�{�܂ł̑���ɂ͏��Ă�Ǝv��");

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Drows, TalkUI.PictureType.Sad, false, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Exa, TalkUI.PictureType.Sad, false);
        ui.SetMessage("�h���V�[", "�킩��ˁ[��");

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Laugh, false, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Drows, TalkUI.PictureType.Sad, false);
        ui.SetMessage("�N�[", "�������ꂶ��o���I");

        yield return ui.WaitForClick();

        ui.RemoveCharacterPic(TalkUI.PicturePos.Left);
        yield return new WaitForSeconds(0.1f);
        ui.RemoveCharacterPic(TalkUI.PicturePos.Center);
        yield return new WaitForSeconds(0.5f);

        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Worra, TalkUI.PictureType.Sad, false, true);
        ui.SetMessage("�E�[��", "���v������");

        yield return ui.WaitForClick();

        ui.RemoveCharacterPic(TalkUI.PicturePos.Center);
        ui.SetMessage("", "");
        yield return new WaitForSeconds(0.1f);
        ui.RemoveCharacterPic(TalkUI.PicturePos.Right);
        yield return new WaitForSeconds(1f);

        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.You, TalkUI.PictureType.Normal, false);
        ui.SetMessage("�I", "�c�c");
        yield return ui.WaitForClick();
        ui.RemoveCharacterPic(TalkUI.PicturePos.Center);
        ui.SetMessage("", "");

        yield return new WaitForSeconds(1f);
        ui.Close();

        Global.GetSaveData().stage0Clear = 1;
        Global.GetSaveData().Save();

        ManagerScript.GetInstance().ChangeScene("TitleScene");
    }
}
