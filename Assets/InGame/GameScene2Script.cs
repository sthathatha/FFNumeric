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
    /// 削除時
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
    /// 範囲フィールド全部生成
    /// </summary>
    private void CreateAllField()
    {
        // HP生成に使用するため最初に周囲の６箇所生成
        var plrLoc = GetPlayerLocation();
        var aroundLocs = Util.GetAroundLocations(plrLoc);
        foreach (var around in aroundLocs)
        {
            CreateEnemyField(around);
        }

        // 残り
        for (int y = -Constant.ENDLESS_FIELD_Y; y <= Constant.ENDLESS_FIELD_Y; ++y)
            for (int x = -Constant.ENDLESS_FIELD_X + y / 2; x <= Constant.ENDLESS_FIELD_X + y / 2; ++x)
            {
                CreateEnemyField(plrLoc + new Vector2Int(x, y));
            }
    }

    /// <summary>
    /// X軸の視界内か判定
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
    /// 視界の外のフィールドを消す
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
    /// 指定位置にEnemy生成
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
    /// 新規生成敵の初期HPを計算
    /// </summary>
    /// <param name="_enemyLoc">生成座標</param>
    /// <returns></returns>
    protected double CalcNewEnemyHp(Vector2Int _enemyLoc)
    {
        // 基本値 プレイヤーの周囲６マスの平均値
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
            // 存在しない場合はプレイヤーHP
            baseValue = GetCharacterScript(plrLoc).GetHp();
        }
        else
        {
            baseValue /= aroundCount;
        }

        bool isHard = Global.GetTemporaryData().difficulty == Constant.Difficulty.Hard;
        // プレイヤーとの距離が１離れる毎に２倍
        var distance = Util.CalcLocationDistance(plrLoc, _enemyLoc);
        // 倍率 ^ (距離-1)
        var distMultiValue = System.Math.Pow(isHard ? 1.9 : 1.7, distance - 1);

        //バランス調整
        // 倍率に乱数を含める
        var randMin = isHard ? 1f : 0.9f;
        var randMax = isHard ? 1.2f : 1.15f;
        // 乱数にターン数が影響
        if (gameTurn > 5)
        {
            randMax += 2.0f * (gameTurn - 5) / (isHard ? 450.0f : 750.0f);
        }
        distMultiValue *= Random.Range(randMin, randMax);

        return Util.FixHpValue(baseValue * distMultiValue);
    }

    /// <summary>
    /// ターン開始処理
    /// </summary>
    override protected IEnumerator TurnStart()
    {
        yield break;
    }

    /// <summary>
    /// ターン終了処理
    /// </summary>
    override protected IEnumerator TurnEnd()
    {
        CreateAllField();
        DeleteFarField();

        // 上限に達していたらクリア
        var plrScr = GetPlayerObject().GetComponent<CharacterScript>();
        if (plrScr.GetHp() >= Constant.HP_LIMIT)
        {
            state = Constant.GameState.GameClear;

            var talk = TalkUI.GetInstance();

            yield return clearCharacterResource;
            imgClearCharacter.sprite = clearCharacterResource.asset as Sprite;
            txtClearTurn.SetText(gameTurn.ToString());
            // NewRecord表示判定
            CheckNewRecord();

            // 初回クリアで会話出す
            if (Global.GetSaveData().endlessClear == 0)
            {
                yield return ClearTalk();
                Global.GetSaveData().endlessClear = 1;
            }

            // 記録セーブ
            SaveRecord();

            //リザルト画面フェード
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
                    talk.SetMessage("", "やっぱりオレが最強だったな！");
                    break;
                case Constant.PlayerID.Eraps:
                    talk.SetMessage("", "私の力で、守りきれたでしょうか");
                    break;
                case Constant.PlayerID.Exa:
                    talk.SetMessage("", "今なら誰にも負ける気がしないぜ");
                    break;
                case Constant.PlayerID.Worra:
                    talk.SetMessage("", "こんなところで負けるわけにはいかないの");
                    break;
                case Constant.PlayerID.Koob:
                    talk.SetMessage("", "この世界の事はだいたいわかってきたな");
                    break;
                case Constant.PlayerID.You:
                    talk.SetMessage("", "帰ってゲームするぜ");
                    break;
            }
            yield return talk.WaitForClick();
            talk.Close();
            yield return new WaitForSeconds(0.5f);
            ManagerScript.GetInstance().ChangeScene("TitleScene");
        }
    }

    /// <summary>
    /// ターン終了以外でプレイヤーが動いた瞬間
    /// </summary>
    protected override void ImmediatePlayerMove()
    {
        base.ImmediatePlayerMove();

        CreateAllField();
        DeleteFarField();
    }

    /// <summary>
    /// ドロシー連続攻撃　終了毎
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator DrowsAttackEnd()
    {
        CreateAllField();

        yield break;
    }

    /// <summary>
    /// プレイ記録保存
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
    /// NewRecord表示判定
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
    /// 初クリア時の会話
    /// </summary>
    /// <returns></returns>
    protected IEnumerator ClearTalk()
    {
        var talk = TalkUI.GetInstance();

        talk.Open();
        talk.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true, true);
        talk.SetMessage("クー", "まじでか…");
        yield return talk.WaitForClick();
        talk.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, true);
        talk.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true, true);
        talk.SetMessage("ドロシー", "どうした？");
        yield return talk.WaitForClick();
        talk.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, false, true);
        talk.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Drows, TalkUI.PictureType.Normal, true);
        talk.SetMessage("クー", "まさかこんなに続けられるとは思わなかったからさ\nさすがにこれ以上の数値は管理できないからここでクリア扱いで勘弁してもらおう");
        yield return talk.WaitForClick();
        talk.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, false);
        talk.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, true, true);
        talk.SetMessage("エラ", "エンドレスモードなのに終わるんですか");
        yield return talk.WaitForClick();
        talk.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.You, TalkUI.PictureType.Normal, true, true);
        talk.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Eraps, TalkUI.PictureType.Normal, true);
        talk.SetMessage("悠", "金かえせー");
        yield return talk.WaitForClick();
        talk.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.You, TalkUI.PictureType.Normal, true);
        talk.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Worra, TalkUI.PictureType.Normal, true, true);
        talk.SetMessage("ウーラ", "無料ゲームなのよ");
        yield return talk.WaitForClick();
        talk.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, false, true);
        talk.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Worra, TalkUI.PictureType.Normal, true);
        talk.SetMessage("クー", "あの名作RPGのスーパージャンプも100回で打ち止めだしね");
        yield return talk.WaitForClick();
        talk.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Koob, TalkUI.PictureType.Normal, false);
        talk.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, true, true);
        talk.SetMessage("エグザ", "あれSwitchでリメイク出たけどどうなったんだ");
        yield return talk.WaitForClick();
        talk.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Koob, TalkUI.PictureType.Laugh, false, true);
        talk.SetCharacterPic(TalkUI.PicturePos.Left, Constant.PlayerID.Exa, TalkUI.PictureType.Normal, true);
        talk.SetMessage("クー", "それは自分の目で確かみてみろ！");
        yield return talk.WaitForClick();
        talk.SetCharacterPic(TalkUI.PicturePos.Right, Constant.PlayerID.Koob, TalkUI.PictureType.Laugh, false);
        talk.SetCharacterPic(TalkUI.PicturePos.Center, Constant.PlayerID.Drows, TalkUI.PictureType.Sad, true, true);
        talk.SetMessage("ドロシー", "おいおい");
        yield return talk.WaitForClick();
        talk.Close();
    }
}
