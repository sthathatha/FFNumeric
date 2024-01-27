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
    /// 削除時
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
    /// 範囲フィールド全部生成
    /// </summary>
    private void CreateStoryField()
    {
        // 別クラスで作成
        var list = StoryStageData.GetFieldList();
        foreach (var data in list)
        {
            CreateEnemyField(data.loc, data.initHp, data.type);
        }
    }

    /// <summary>
    /// 指定位置にEnemy生成
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
    /// ターン開始処理
    /// </summary>
    override protected IEnumerator TurnStart()
    {
        // 開始時チュートリアル会話各種
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
    /// ターン終了処理
    /// </summary>
    override protected IEnumerator TurnEnd()
    {
        // ボスが居ないorボス位置にプレイヤーが入ってたらクリア
        if (IsCharacterBeing(bossLocation) && GetPlayerLocation() != bossLocation)
        {
            yield break;
        }

        // クリアフラグと会話
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

    #region 会話　ドロシー用
    /// <summary>
    /// ドロシー用チュートリアル会話
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

            ui.SetMessage("", "範囲内の対象をタップして攻撃します");
            yield return ui.WaitForClick();
            ui.Close();
        }
        else if (gameTurn == 2)
        {
            ui.Open();

            ui.SetMessage("", "ボス以外の敵はターン毎に成長していきますが、ドロシーはこれに対抗する強力な個性を持っています");
            yield return ui.WaitForClick();
            ui.SetMessage("", "続けて右に進んでみましょう");
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

                ui.SetMessage("", "ドロシーはモンスターを一撃で倒した時、自動的に同じターン内でまっすぐ進み続けます。");
                yield return ui.WaitForClick();
                ui.SetMessage("", "なおストーリーモードでは敵が復活せず、まわりに敵が居なくなると動けなくなってしまうため注意してください。");
                yield return ui.WaitForClick();
                ui.SetMessage("", "詰んだ場合は右上のメニューからタイトル画面に戻ることができます。");
                yield return ui.WaitForClick();
                ui.SetMessage("", "それでは、次は右下に進んでみましょう");
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
                ui.SetMessage("ドロシー", "おっ？\nこいつ強いな");
                yield return ui.WaitForClick();

                ui.RemoveCharacterPic(TalkUI.PicturePos.Left);
                ui.SetMessage("", "個性による追加攻撃では、倒せなかった場合でも戦闘とならずにターンが終了するので安全です。");
                yield return ui.WaitForClick();

                ui.SetMessage("", "ボスの黒いスライムを倒すとステージクリアとなります。");
                yield return ui.WaitForClick();
                ui.Close();
            }
        }else if(gameTurn == 5)
        {
            ui.Open();

            ui.SetMessage("", "敵を倒すと右下のゲージが溜まっていき、一杯になるとタップしてスキルを発動することができます。");
            yield return ui.WaitForClick();

            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true, true);
            ui.SetMessage("ドロシー", "全然溜まらねーじゃん");
            yield return ui.WaitForClick();

            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true);
            ui.SetMessage("", "ストーリーモードでは溜まりきらないため、実質エンドレスモード専用となります。");
            yield return ui.WaitForClick();

            ui.SetMessage("", "スキルの効果はキャラクターリスト画面で確認することができます。");
            yield return ui.WaitForClick();
            ui.Close();
        }

        if (error)
        {
            enableTutorial = false;
            ui.Open();
            ui.SetMessage("", "ちょっと！予定と違うことしないでくださいよ");
            yield return ui.WaitForClick();

            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true, true);
            ui.SetMessage("ドロシー", "なんだよ別にいいじゃん");
            yield return ui.WaitForClick();

            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true);
            ui.SetMessage("", "あーっ！チュートリアルを何だと思ってるんですか！");
            yield return ui.WaitForClick();
            ui.SetMessage("", "もう知らん！勝手にしなさい！");
            yield return ui.WaitForClick();

            ui.SetMessage("ドロシー", "……");
            yield return ui.WaitForClick();

            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Drows, TalkUI.PictureType.Sad, true, true);
            ui.SetMessage("ドロシー", "誰だ今の");
            yield return ui.WaitForClick();
            ui.Close();
        }
    }

    /// <summary>
    /// ドロシークリア会話
    /// </summary>
    /// <returns></returns>
    private IEnumerator DrowsClearTutorial()
    {
        var ui = TalkUI.GetInstance();

        ui.Open();

        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, false, true);
        ui.SetMessage("ドロシー", "ま、こんなもんか\n次どこ行けばいいんだっけな");
        yield return ui.WaitForClick();

        ui.SetMessage("ドロシー", "適当に歩いてきゃなんかあるかな");
        yield return ui.WaitForClick();

        ui.Close();
    }
    #endregion

    #region 会話　エラ用
    /// <summary>
    /// エラ用チュートリアル会話
    /// </summary>
    private IEnumerator ErapsStartTutorial()
    {
        var ui = TalkUI.GetInstance();
        if (gameTurn == 1)
        {
            ui.Open();

            ui.SetMessage("", "エラはターン開始時と敵の撃破時にシールドを生成します。");
            yield return ui.WaitForClick();
            ui.SetMessage("", "生成量は最初はわずかですが、自動発動のスキルで上昇していきます。");
            yield return ui.WaitForClick();
            ui.Close();
        }
    }

    /// <summary>
    /// エラクリア会話
    /// </summary>
    /// <returns></returns>
    private IEnumerator ErapsClearTutorial()
    {
        var ui = TalkUI.GetInstance();

        ui.Open();

        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Eraps, TalkUI.PictureType.Sad, false, true);
        ui.SetMessage("エラ", "これは、思った以上に大変そうだ…\n早くみなさんと合流しないと");
        yield return ui.WaitForClick();

        ui.Close();
    }
    #endregion

    #region 会話　エグザ用
    /// <summary>
    /// エグザ用チュートリアル会話
    /// </summary>
    private IEnumerator ExaStartTutorial()
    {
        var ui = TalkUI.GetInstance();
        if (gameTurn == 1)
        {
            ui.Open();

            ui.SetMessage("", "エグザは攻撃時、対象の左右にも中程度のダメージを与えます。");
            yield return ui.WaitForClick();
            ui.SetMessage("", "左右の敵を倒した場合も成長しますが、その場所には移動しません。");
            yield return ui.WaitForClick();
            ui.Close();
        }
    }

    /// <summary>
    /// エグザクリア会話
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExaClearTutorial()
    {
        var ui = TalkUI.GetInstance();

        ui.Open();

        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, false, true);
        ui.SetMessage("エグザ", "こいつがここのボスだな\nさて、次だ次");
        yield return ui.WaitForClick();

        ui.Close();
    }
    #endregion

    #region 会話　ウーラ用
    /// <summary>
    /// ウーラ用チュートリアル会話
    /// </summary>
    private IEnumerator WorraStartTutorial()
    {
        var ui = TalkUI.GetInstance();
        if (gameTurn == 1)
        {
            ui.Open();

            ui.SetMessage("", "ウーラは２連続で攻撃し、２倍のダメージを与えます。");
            yield return ui.WaitForClick();
            ui.SetMessage("", "途中で敵を倒した場合、成長前の値でさらに奥の敵にダメージを与えます。");
            yield return ui.WaitForClick();
            ui.SetMessage("", "奥の敵を倒しても成長しますが、その場所には移動しません。");
            yield return ui.WaitForClick();
            ui.Close();
        }
    }

    /// <summary>
    /// ウーラクリア会話
    /// </summary>
    /// <returns></returns>
    private IEnumerator WorraClearTutorial()
    {
        var ui = TalkUI.GetInstance();

        ui.Open();

        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Worra, TalkUI.PictureType.Sad, false, true);
        ui.SetMessage("ウーラ", "はあ、厄介な所に来ちゃったわね\n早く終わらせましょう");
        yield return ui.WaitForClick();

        ui.Close();
    }
    #endregion

    #region 会話　クー用
    /// <summary>
    /// クー用チュートリアル会話
    /// </summary>
    private IEnumerator KoobStartTutorial()
    {
        var ui = TalkUI.GetInstance();
        if (gameTurn == 1)
        {
            ui.Open();

            ui.SetMessage("", "クーは魔法により距離2マスまでの敵を攻撃できます。");
            yield return ui.WaitForClick();

            ui.SetMessage("", "離れた敵と戦う時は、反撃でうけるダメージが大幅に軽減されます。");
            yield return ui.WaitForClick();

            ui.Close();
        }else if (gameTurn == 2)
        {
            ui.Open();

            ui.SetMessage("", "クーは敵を倒した時に通常より大きく成長します。");
            yield return ui.WaitForClick();

            ui.Close();
        }
    }

    /// <summary>
    /// クークリア会話
    /// </summary>
    /// <returns></returns>
    private IEnumerator KoobClearTutorial()
    {
        var ui = TalkUI.GetInstance();

        ui.Open();

        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, false, true);
        ui.SetMessage("クー", "変なルールの世界だけどだいたい理解したぞ");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Laugh, false, true);
        ui.SetMessage("クー", "さあ魔王の所にいこう！");
        yield return ui.WaitForClick();

        ui.Close();
    }
    #endregion

    #region 会話　悠用
    /// <summary>
    /// 悠用チュートリアル会話
    /// </summary>
    private IEnumerator YouStartTutorial()
    {
        var ui = TalkUI.GetInstance();
        if (gameTurn == 1)
        {
            ui.Open();

            ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true, true);
            ui.SetMessage("ドロシー", "たのもー！");
            yield return ui.WaitForClick();
            ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true);
            ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, true, true);
            ui.SetMessage("エラ", "道場じゃないんですよ");
            yield return ui.WaitForClick();
            ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, true);
            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Worra, TalkUI.PictureType.Normal, true, true);
            ui.SetMessage("ウーラ", "みんな、あれ見て");
            yield return ui.WaitForClick();
            ui.Close();
            yield return new WaitForSeconds(1f);

            ui.Open();
            ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Sad, true, true);
            ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.You, TalkUI.PictureType.Normal, false);
            ui.SetMessage("クー", "悠ちゃん、何してんの？");
            yield return ui.WaitForClick();
            ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Sad, true);
            ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.You, TalkUI.PictureType.Normal, false, true);
            ui.SetMessage("悠", "なんか魔王やっつけたら崇拝されちゃって、新しい魔王になったみたい");
            yield return ui.WaitForClick();
            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Sad, true, true);
            ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.You, TalkUI.PictureType.Normal, false);
            ui.SetMessage("エグザ", "なったみたい\nじゃねーよバカタレ");
            yield return ui.WaitForClick();
            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Sad, true);
            ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.You, TalkUI.PictureType.Sp1, false, true);
            ui.SetMessage("悠", "ふはははは！よく来たな人間ども");
            yield return ui.WaitForClick();
            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Sad, true, true);
            ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.You, TalkUI.PictureType.Sp1, false);
            ui.SetMessage("エグザ", "うるせえ");
            yield return ui.WaitForClick();
            ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Sad, true);
            ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Drows, TalkUI.PictureType.Sad, true, true);
            ui.SetMessage("ドロシー", "ぶん殴ってさっさと連れて帰るぞ");
            yield return ui.WaitForClick();
            ui.Close();
        }
    }

    /// <summary>
    /// 悠クリア会話
    /// </summary>
    /// <returns></returns>
    private IEnumerator YouClearTutorial()
    {
        var ui = TalkUI.GetInstance();

        ui.Open();

        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.You, TalkUI.PictureType.Sad, false, true);
        ui.SetMessage("悠", "あたたたた");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.You, TalkUI.PictureType.Normal, false);
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, true, true);
        ui.SetMessage("エラ", "ほら帰りますよ");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true, true);
        ui.SetMessage("クー", "この世界はもう大丈夫そうだね");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Worra, TalkUI.PictureType.Normal, true, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true);
        ui.SetMessage("ウーラ", "早かったわね、6ステージぐらいしか無かったような気がするわ");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, true, true);
        ui.SetMessage("エグザ", "ステージって何だよ");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Laugh, false, true);
        ui.SetMessage("クー", "同じことやるだけのゲームでステージいっぱい作ってもしょうがないしね");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, false, true);
        ui.SetMessage("クー", "あとはエンドレスモードでも遊べばいいんじゃないかな");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, false);
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true, true);
        ui.SetMessage("ドロシー", "長く遊ばせるための育成要素みたいなのは無いのか");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, false, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true);
        ui.SetMessage("クー", "こんなゲームに人生の貴重な時間使うより、次に行ったほうがいいと思う");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, false);
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Sad, true, true);
        ui.SetMessage("エグザ", "お前それ…");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, true, true);
        ui.SetMessage("エグザ", "まあいいか");
        yield return ui.WaitForClick();
        ui.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, true);
        ui.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.You, TalkUI.PictureType.Normal, false, true);
        ui.SetMessage("悠", "また何かへちょい嘘広告ゲーがあったら作ってみようか");
        yield return ui.WaitForClick();

        ui.Close();
    }
    #endregion
}
