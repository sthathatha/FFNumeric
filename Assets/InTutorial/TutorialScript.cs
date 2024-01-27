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
        ui.SetMessage("クー", "よーし魔王を倒すぞ！");

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Laugh, false);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Eraps, TalkUI.PictureType.Sad, false, true);
        ui.SetMessage("エラ", "早い早いまだプロローグ始まって２秒ですよ");

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Eraps, TalkUI.PictureType.Sad, false);
        ui.SetMessage("クー", "こんなへちょいゲームにストーリーとか無くてもいいんじゃない");

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, false);
        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, false, true);
        ui.SetMessage("エグザ", "へちょいって何だよ");

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, false);
        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, false);
        ui.SetMessage("クー", "というわけでこの世界のルールを確認するよ");

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Sp1, true, true);
        ui.SetMessage("クー", "これだ！バンッ");
        image1.SetActive(true);

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Sp1, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Worra, TalkUI.PictureType.Laugh, false, true);
        ui.SetMessage("ウーラ", "見たことある～");

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, false, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Worra, TalkUI.PictureType.Normal, false);
        ui.SetMessage("エラ", "確かにへちょいかも…");

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, false);
        ui.SetMessage("クー", "一応ちょっとだけ見た目と違うルールにしてて、戦闘をしかけたら最初に攻撃してダメージを与えるよ");
        image1.SetActive(false);
        image2.SetActive(true);

        yield return ui.WaitForClick();

        ui.SetMessage("クー", "そこから相手が反撃して交互に…で、０にしたほうが勝ち");
        image2.SetActive(false);
        image3.SetActive(true);

        yield return ui.WaitForClick();

        ui.SetMessage("クー", "勝負がつくまで戦って１ターン終了とします");
        image3.SetActive(false);
        image4.SetActive(true);

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, false, true);
        ui.SetMessage("エグザ", "システム上こっちが先手取るからかなり有利だな");
        image4.SetActive(false);

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, false);
        ui.SetMessage("クー", "計算では　x^2 < x + 1　を満たすようなx倍までの相手には勝てると思う");

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Drows, TalkUI.PictureType.Sad, false, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Exa, TalkUI.PictureType.Sad, false);
        ui.SetMessage("ドロシー", "わかんねーよ");

        yield return ui.WaitForClick();

        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Koob, TalkUI.PictureType.Laugh, false, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Drows, TalkUI.PictureType.Sad, false);
        ui.SetMessage("クー", "さあそれじゃ出発！");

        yield return ui.WaitForClick();

        ui.RemoveCharacterPic(TalkUI.PicturePos.Left);
        yield return new WaitForSeconds(0.1f);
        ui.RemoveCharacterPic(TalkUI.PicturePos.Center);
        yield return new WaitForSeconds(0.5f);

        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Worra, TalkUI.PictureType.Sad, false, true);
        ui.SetMessage("ウーラ", "大丈夫かしら");

        yield return ui.WaitForClick();

        ui.RemoveCharacterPic(TalkUI.PicturePos.Center);
        ui.SetMessage("", "");
        yield return new WaitForSeconds(0.1f);
        ui.RemoveCharacterPic(TalkUI.PicturePos.Right);
        yield return new WaitForSeconds(1f);

        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.You, TalkUI.PictureType.Normal, false);
        ui.SetMessage("悠", "……");
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
