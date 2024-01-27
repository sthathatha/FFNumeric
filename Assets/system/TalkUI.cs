using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class TalkUI : MonoBehaviour
{
    protected static TalkUI instance = null;
    public static TalkUI GetInstance() { return instance; }

    /// <summary>
    /// 立ち絵表示位置
    /// </summary>
    public enum PicturePos : int
    {
        Left = 0,
        Center,
        Right,
    }

    /// <summary>
    /// キャラクター表情
    /// </summary>
    public enum PictureType : int
    {
        Normal = 0,
        Laugh,
        Sad,
        Sp1,
        Sp2,
    }

    public GameObject canvas;
    public GameObject charaL;
    public GameObject charaC;
    public GameObject charaR;
    public GameObject nameBox;
    public TMP_Text nameText;
    public GameObject messageBox;
    public TMP_Text messageText;

    private List<List<ResourceRequest>> spriteListList;
    private List<List<ResourceRequest>> spriteListListOpen;

    /// <summary>
    /// 立ち絵パラメータ
    /// </summary>
    private struct PictureParam
    {
        public bool active;

        public Constant.PlayerID playerID;
        public PictureType pictureType;
        public bool isRight;

        public PictureParam(bool _active)
        {
            active = _active;
            playerID = Constant.PlayerID.Drows;
            pictureType = PictureType.Normal;
            isRight = false;
        }
    }
    private List<PictureParam> pictureParams;

    private bool waitForClick;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public TalkUI()
    {
        instance = this;

        spriteListList = new List<List<ResourceRequest>>();
        spriteListListOpen = new List<List<ResourceRequest>>();
        pictureParams = new List<PictureParam>()
        {
            new(false),
            new(false),
            new(false)
        };
    }

    /// <summary>
    /// 初期化
    /// </summary>
    void Start()
    {
        var resDir = "CharacterPic/";
        for (int i = 0; i < 6; ++i)
        {
            var dir = resDir + Constant.GetPlayerNameE((Constant.PlayerID)i) + "/";
            spriteListList.Add(new List<ResourceRequest>());
            spriteListList[i].Add(Resources.LoadAsync<Sprite>(dir + "001_normal"));
            spriteListList[i].Add(Resources.LoadAsync<Sprite>(dir + "002_warai"));
            spriteListList[i].Add(Resources.LoadAsync<Sprite>(dir + "003_komari"));
            spriteListListOpen.Add(new List<ResourceRequest>());
            spriteListListOpen[i].Add(Resources.LoadAsync<Sprite>(dir + "001_normal_o"));
            spriteListListOpen[i].Add(Resources.LoadAsync<Sprite>(dir + "002_warai_o"));
            spriteListListOpen[i].Add(Resources.LoadAsync<Sprite>(dir + "003_komari_o"));

            if (i == (int)Constant.PlayerID.Koob
                || i == (int)Constant.PlayerID.You)
            {
                spriteListList[i].Add(Resources.LoadAsync<Sprite>(dir + "004_sp1"));
                spriteListListOpen[i].Add(Resources.LoadAsync<Sprite>(dir + "004_sp1_o"));
            }
        }
    }

    /// <summary>
    /// リソース読み込み待ち
    /// </summary>
    /// <returns></returns>
    public bool IsReady()
    {
        foreach (var list in spriteListList)
        {
            foreach (var item in list)
            {
                if (!item?.isDone == true) { return false; }
            }
        }

        foreach (var list in spriteListListOpen)
        {
            foreach (var item in list)
            {
                if (!item?.isDone == true) { return false; }
            }
        }

        return true;
    }

    /// <summary>
    /// 終了
    /// </summary>
    private void OnDestroy()
    {
        foreach (var list in spriteListList)
        {
            foreach (var item in list)
            {
                Resources.UnloadAsset(item.asset);
            }
            list.Clear();
        }
        spriteListList.Clear();

        foreach (var list in spriteListListOpen)
        {
            foreach (var item in list)
            {
                Resources.UnloadAsset(item.asset);
            }
            list.Clear();
        }
        spriteListListOpen.Clear();

        pictureParams.Clear();
    }

    #region 使用メソッド
    /// <summary>
    /// 開く
    /// </summary>
    public void Open()
    {
        charaL.SetActive(false);
        charaC.SetActive(false);
        charaR.SetActive(false);
        nameText.SetText(string.Empty);
        messageText.SetText(string.Empty);
        nameBox.SetActive(true);
        messageBox.SetActive(true);

        canvas.SetActive(true);
    }

    /// <summary>
    /// 閉じる
    /// </summary>
    public void Close()
    {
        canvas.SetActive(false);
    }

    /// <summary>
    /// 画像表示
    /// </summary>
    /// <param name="pos">表示位置</param>
    /// <param name="charaId">キャラID</param>
    /// <param name="type">表情</param>
    /// <param name="isRight">右向き</param>
    /// <param name="isOpen">口開く</param>
    public void SetCharacterPic(PicturePos pos, Constant.PlayerID charaId, PictureType type, bool isRight, bool isOpen = false)
    {
        var charaInt = (int)charaId;
        var typeInt = (int)type;

        Sprite spr;
        if (isOpen)
        {
            spr = spriteListListOpen[charaInt][typeInt].asset as Sprite;
        }
        else
        {
            spr = spriteListList[charaInt][typeInt].asset as Sprite;
        }

        GameObject obj;
        switch (pos)
        {
            case PicturePos.Left: obj = charaL; break;
            case PicturePos.Center: obj = charaC; break;
            default: obj = charaR; break;
        }

        var prm = pictureParams[(int)pos];

        obj.GetComponent<Image>().sprite = spr;
        obj.GetComponent<RectTransform>().localScale = new Vector3(isRight ? -1 : 1, 1, 1);
        obj.SetActive(true);
        if (!(prm.active && prm.playerID == charaId))
        {
            // ほぼ同じキャラの場合切り替えアニメーションしない
            obj.GetComponent<Animator>().Play("uiAppear", 0, 0f);
        }

        prm.active = true;
        prm.playerID = charaId;
        prm.pictureType = type;
        prm.isRight = isRight;
        pictureParams[(int)pos] = prm;
    }

    /// <summary>
    /// 立ち絵を消す
    /// </summary>
    /// <param name="pos"></param>
    public void RemoveCharacterPic(PicturePos pos)
    {
        GameObject obj;
        switch (pos)
        {
            case PicturePos.Left: obj = charaL; break;
            case PicturePos.Center: obj = charaC; break;
            default: obj = charaR; break;
        }

        obj.SetActive(false);
        pictureParams[(int)pos] = new PictureParam(false);
    }

    /// <summary>
    /// メッセージ表示
    /// </summary>
    /// <param name="name">話者名</param>
    /// <param name="message">テキスト</param>
    public void SetMessage(string name, string message)
    {
        if (string.IsNullOrEmpty(name))
        {
            nameBox.SetActive(false);
        }
        else
        {
            nameBox.SetActive(true);
        }
        nameText.SetText(name);

        messageText.SetText(message);
    }
    #endregion

    /// <summary>
    /// クリックを待つ
    /// </summary>
    /// <returns></returns>
    public IEnumerator WaitForClick()
    {
        waitForClick = true;

        yield return new WaitWhile(() => waitForClick);
    }

    /// <summary>
    /// クリック処理
    /// </summary>
    public void MessageClick()
    {
        waitForClick = false;
    }
}
