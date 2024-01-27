using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class CharacterSceneScript : MonoBehaviour
{
    public Image imgCharaPic;
    public TMP_Text txtPersonalityExp;
    public TMP_Text txtPersonalityName;
    public TMP_Text txtSkillExp;
    public TMP_Text txtSkillName;
    public TMP_Text txtSkillCharge;
    public TMP_Text txtSkillStock;
    public TMP_Text txtInitHp;
    public TMP_Text txtInitRange;
    public TMP_Text txtNameE;
    public TMP_Text txtNameJ;

    private List<ResourceRequest> textureList;
    private int displayCharacter;
    private Coroutine changeCoroutine;

    #region 表示文字列
    private struct DisplayStatus
    {
        public string nameE;
        public string nameJ;
        public int initHP;
        public int initRange;
        public string personalityName;
        public string personalityExplain;
        public string skillName;
        public string skillExplain;
        public int skillCharge;
        public int skillStock;
    }

    private readonly DisplayStatus STAT_DROWS = new DisplayStatus()
    {
        nameE = "DROWS",
        nameJ = "ドロシー",
        initHP = (int)System.Math.Floor(Constant.DROWS_PARAM.InitHP),
        initRange = 1,
        personalityName = "ベルセルク",
        personalityExplain = $"最大{Constant.SKILL_DROWS_ATTACK_LIMIT}回まで、一撃で敵を倒す限りターンを終了せずに移動し同じ方向へ追加攻撃を行う" +
        "\n追加攻撃で倒せなかった場合は反撃を受けず、ターンが終了する",
        skillName = "カルネージドライヴ",
        skillExplain = $"{Constant.SKILL_DROWS_KEEP_TURN}ターンの間、与えるダメージが{(int)Constant.SKILL_DROWS_ATTACK_RATE * 100}％になる",
        skillCharge = Constant.DROWS_PARAM.SkillGaugeLength,
        skillStock = Constant.DROWS_PARAM.SkillStockMax
    };
    private readonly DisplayStatus STAT_ERAPS = new DisplayStatus()
    {
        nameE = "ERAPS",
        nameJ = "エラ",
        initHP = (int)System.Math.Floor(Constant.ERAPS_PARAM.InitHP),
        initRange = 1,
        personalityName = "騎士道",
        personalityExplain = "ターン開始時と敵の撃破時にシールドを生成する",
        skillName = "踏み出す一歩に迷い無く",
        skillExplain = "シールドの生成速度が上昇する（ステージ中永続）",
        skillCharge = Constant.ERAPS_PARAM.SkillGaugeLength,
        skillStock = Constant.ERAPS_PARAM.SkillStockMax
    };
    private readonly DisplayStatus STAT_EXA = new DisplayStatus()
    {
        nameE = "EXA",
        nameJ = "エグザ",
        initHP = (int)System.Math.Floor(Constant.EXA_PARAM.InitHP),
        initRange = 1,
        personalityName = "掃除屋",
        personalityExplain = "攻撃時、左右の敵にもダメージを与える",
        skillName = "ギャングスター",
        skillExplain = "対象と左右に100％ダメージを与え、与えたダメージ分成長する\n対象を倒した場合は移動する",
        skillCharge = Constant.EXA_PARAM.SkillGaugeLength,
        skillStock = Constant.EXA_PARAM.SkillStockMax
    };
    private readonly DisplayStatus STAT_WORRA = new DisplayStatus()
    {
        nameE = "WORRA",
        nameJ = "ウーラ",
        initHP = (int)System.Math.Floor(Constant.WORRA_PARAM.InitHP),
        initRange = 1,
        personalityName = "連射",
        personalityExplain = "連続攻撃を行う\n途中で倒した場合、成長前に奥の敵に攻撃する\n遠距離戦闘の場合は対象の周囲６体",
        skillName = "サテライトスナイプ",
        skillExplain = $"戦闘範囲が1増える（ステージ中永続：最大{Constant.SKILL_WORRA_MAX_RANGE}）" +
        $"\n射程最大時には攻撃回数が1増える（ステージ中永続）",
        skillCharge = Constant.WORRA_PARAM.SkillGaugeLength,
        skillStock = Constant.WORRA_PARAM.SkillStockMax
    };
    private readonly DisplayStatus STAT_KOOB = new DisplayStatus()
    {
        nameE = "KOOB",
        nameJ = "クー",
        initHP = (int)System.Math.Floor(Constant.KOOB_PARAM.InitHP),
        initRange = Constant.SKILL_KOOB_ATTACK_RANGE,
        personalityName = "天才",
        personalityExplain = $"成長速度が{(int)(Constant.SKILL_KOOB_POWUP_RATE * 100)}％になる",
        skillName = "ラグナロク",
        skillExplain = $"{Constant.SKILL_KOOB_RAGNAROK_RANGE}マス内の敵全員に{(int)Constant.SKILL_KOOB_RAGNAROK_RATE * 100}％ダメージを与え、{Constant.SKILL_KOOB_RAGNAROK_POW_INVALID_TURN}ターンの成長不可と{Constant.SKILL_KOOB_RAGNAROK_PARALYZE_TURN}回の反撃不可を付与する" +
        "\n敵を倒した場合、残った敵の近くに移動する",
        skillCharge = Constant.KOOB_PARAM.SkillGaugeLength,
        skillStock = Constant.KOOB_PARAM.SkillStockMax
    };
    private readonly DisplayStatus STAT_YOU = new DisplayStatus()
    {
        nameE = "YOU",
        nameJ = "悠",
        initHP = (int)System.Math.Floor(Constant.YOU_PARAM.InitHP),
        initRange = 1,
        personalityName = "武士道",
        personalityExplain = $"受けるダメージが{(int)(Constant.SKILL_YOU_DAMAGE_RATE * 100)}％になる" +
        $"\n与えるダメージが{(int)(Constant.SKILL_YOU_ATTACK_RATE * 100)}％になる",
        skillName = "熾影迅閃",
        skillExplain = $"{Constant.SKILL_YOU_KILL_RANGE}マス以内の対象1体を即死させ、移動する。",
        skillCharge = Constant.YOU_PARAM.SkillGaugeLength,
        skillStock = Constant.YOU_PARAM.SkillStockMax
    };
    #endregion

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public CharacterSceneScript()
    {
        textureList = new List<ResourceRequest>();
    }

    /// <summary>
    /// 削除
    /// </summary>
    public void OnDestroy()
    {
        foreach (var handle in textureList)
        {
            //if (handle.IsValid())
            if (handle != null)
            {
                Resources.UnloadAsset(handle.asset);
                //Addressables.Release(handle);
            }
        }

        textureList.Clear();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Start()
    {
        textureList.Add(Resources.LoadAsync<Sprite>("CharacterListScene/drows"));
        textureList.Add(Resources.LoadAsync<Sprite>("CharacterListScene/eraps"));
        textureList.Add(Resources.LoadAsync<Sprite>("CharacterListScene/exa"));
        textureList.Add(Resources.LoadAsync<Sprite>("CharacterListScene/worra"));
        textureList.Add(Resources.LoadAsync<Sprite>("CharacterListScene/koob"));
        textureList.Add(Resources.LoadAsync<Sprite>("CharacterListScene/you"));
        //textureList.Add(Addressables.LoadAssetAsync<Sprite>("charaui_drows_0"));
        //textureList.Add(Addressables.LoadAssetAsync<Sprite>("charaui_eraps_0"));
        //textureList.Add(Addressables.LoadAssetAsync<Sprite>("charaui_exa_0"));
        //textureList.Add(Addressables.LoadAssetAsync<Sprite>("charaui_worra_0"));
        //textureList.Add(Addressables.LoadAssetAsync<Sprite>("charaui_koob_0"));
        //textureList.Add(Addressables.LoadAssetAsync<Sprite>("charaui_you_0"));

        displayCharacter = (int)Constant.PlayerID.Drows;
        changeCoroutine = null;
        UpdateCharacterInfo(false);
    }

    /// <summary>
    /// 右ボタン
    /// </summary>
    public void ChangeRight()
    {
        if (changeCoroutine != null)
        {
            return;
        }

        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_MOVE);
        changeCoroutine = StartCoroutine(ChangeRightCoroutine());
    }

    public IEnumerator ChangeRightCoroutine()
    {
        imgCharaPic.GetComponent<Animator>().SetTrigger("leftDisapp");
        yield return new WaitForSeconds(0.25f);

        displayCharacter++;
        if (displayCharacter > (int)Constant.PlayerID.You)
        {
            displayCharacter = (int)Constant.PlayerID.Drows;
        }
        yield return textureList[displayCharacter];

        UpdateCharacterInfo();

        changeCoroutine = null;

        imgCharaPic.GetComponent<Animator>().SetTrigger("rightApp");
    }

    /// <summary>
    /// 左ボタン
    /// </summary>
    public void ChangeLeft()
    {
        if (changeCoroutine != null)
        {
            return;
        }

        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_MOVE);
        changeCoroutine = StartCoroutine(ChangeLeftCoroutine());
    }

    public IEnumerator ChangeLeftCoroutine()
    {
        imgCharaPic.GetComponent<Animator>().SetTrigger("rightDisapp");
        yield return new WaitForSeconds(0.25f);

        displayCharacter--;
        if (displayCharacter < (int)Constant.PlayerID.Drows)
        {
            displayCharacter = (int)Constant.PlayerID.You;
        }
        yield return textureList[displayCharacter];

        UpdateCharacterInfo();

        changeCoroutine = null;

        imgCharaPic.GetComponent<Animator>().SetTrigger("leftApp");
    }

    /// <summary>
    /// displayCharacterの情報を表示
    /// </summary>
    /// <param name="picture">画像変更</param>
    private void UpdateCharacterInfo(bool picture = true)
    {
        DisplayStatus disp;
        switch ((Constant.PlayerID)displayCharacter)
        {
            case Constant.PlayerID.Drows: disp = STAT_DROWS; break;
            case Constant.PlayerID.Eraps: disp = STAT_ERAPS; break;
            case Constant.PlayerID.Exa: disp = STAT_EXA; break;
            case Constant.PlayerID.Worra: disp = STAT_WORRA; break;
            case Constant.PlayerID.Koob: disp = STAT_KOOB; break;
            default: disp = STAT_YOU; break;
        }

        if (picture)
        {
            //imgCharaPic.sprite = textureList[displayCharacter].Result;
            imgCharaPic.sprite = textureList[displayCharacter].asset as Sprite;
        }

        txtPersonalityName.SetText(disp.personalityName);
        txtPersonalityExp.SetText(disp.personalityExplain);
        txtSkillName.SetText(disp.skillName);
        txtSkillExp.SetText(disp.skillExplain);
        txtSkillCharge.SetText(disp.skillCharge.ToString());
        txtSkillStock.SetText(disp.skillStock > 0 ? disp.skillStock.ToString() : "自動");
        txtInitHp.SetText(disp.initHP.ToString());
        txtInitRange.SetText(disp.initRange.ToString());
        txtNameE.SetText(disp.nameE);
        txtNameJ.SetText(disp.nameJ);
    }

    /// <summary>
    /// 戻る
    /// </summary>
    public void BackButton()
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_CANCEL);
        ManagerScript.GetInstance().ChangeScene("TitleScene");
    }
}
