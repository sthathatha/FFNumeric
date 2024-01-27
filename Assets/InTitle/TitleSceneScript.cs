using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneScript : MonoBehaviour
{
    public GameObject wndStory;
    public GameObject btnStory0;
    public GameObject btnStory1;
    public GameObject btnStory2;
    public GameObject btnStory3;
    public GameObject btnStory4;
    public GameObject btnStory5;
    public GameObject btnStory6;

    public GameObject wndStory6;

    public GameObject wndEndless;
    public Button btnEndlessDrows;
    public Button btnEndlessEraps;
    public Button btnEndlessExa;
    public Button btnEndlessWorra;
    public Button btnEndlessKoob;
    public Button btnEndlessYou;
    public Toggle chkHardMode;

    public GameObject wndOption;
    public Toggle chkBattleSkip;
    public Slider slBgmVol;
    public Slider slSEVol;

    public GameObject wndCredit;

    // Start is called before the first frame update
    void Start()
    {
        Global.GetSaveData().Load();
        var opt = Global.GetSaveData().option;
        chkBattleSkip.isOn = (opt.skipEffect == 1);
        slBgmVol.value = opt.bgmVolume;
        slSEVol.value = opt.seVolume;

        SoundManager.GetInstance().PlayBGM(SoundManager.BgmID.TITLE);

        // ステージクリア状況により表示
        var saveData = Global.GetSaveData();
        btnStory1.SetActive(saveData.stage0Clear == 1);
        btnStory2.SetActive(saveData.stage1Clear == 1);
        btnStory3.SetActive(saveData.stage2Clear == 1);
        btnStory4.SetActive(saveData.stage3Clear == 1);
        btnStory5.SetActive(saveData.stage4Clear == 1);
        btnStory6.SetActive(saveData.stage5Clear == 1);

        // ハードモードチェック
        chkHardMode.isOn = (Global.GetTemporaryData().difficulty == Constant.Difficulty.Hard);
    }

    #region ウィンドウ開閉
    /// <summary>
    /// ストーリーウィンドウ開く
    /// </summary>
    public void OpenStoryWindow()
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_OK);
        wndStory.SetActive(true);
    }
    /// <summary>
    /// ストーリーウィンドウ閉じる
    /// </summary>
    public void CloseStoryWindow()
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_CANCEL);
        wndStory.SetActive(false);
    }

    /// <summary>
    /// ストーリー６キャラ選択開く
    /// </summary>
    public void OpenStory6Window()
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_OK);
        wndStory6.SetActive(true);
    }

    /// <summary>
    /// ストーリー６キャラ選択閉じる
    /// </summary>
    public void CloseStory6Window()
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_CANCEL);
        wndStory6.SetActive(false);
    }

    /// <summary>
    /// エンドレス開く
    /// </summary>
    public void OpenEndlessWindow()
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_OK);
        wndEndless.SetActive(true);
    }
    /// <summary>
    /// エンドレスウィンドウ閉じる
    /// </summary>
    public void CloseEndlessWindow()
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_CANCEL);
        wndEndless.SetActive(false);
    }

    /// <summary>
    /// オプション開く
    /// </summary>
    public void OpenOptionWindow()
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_OK);
        wndOption.SetActive(true);
    }
    /// <summary>
    /// オプション閉じる
    /// </summary>
    public void CloseOptionWindow()
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_CANCEL);
        wndOption.SetActive(false);

        Global.GetSaveData().Save();
    }

    /// <summary>
    /// クレジット開く
    /// </summary>
    public void OpenCreditWindow()
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_OK);
        wndCredit.SetActive(true);
    }
    /// <summary>
    /// クレジット閉じる
    /// </summary>
    public void CloseCreditWindow()
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_CANCEL);
        wndCredit.SetActive(false);
    }
    #endregion

    #region シーン移動
    /// <summary>
    /// ストーリーモード開始
    /// </summary>
    /// <param name="stageNo"></param>
    public void StartStoryMode(int stageNo)
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_OK);
        if (stageNo == 0)
        {
            ManagerScript.GetInstance().ChangeScene("TutorialScene");
        } else
        {
            Global.GetTemporaryData().selectStage = stageNo;
            Global.GetTemporaryData().selectPlayer = (Constant.PlayerID)(stageNo - 1);
            ManagerScript.GetInstance().ChangeScene("GameScene");
        }
    }

    /// <summary>
    /// ストーリー６開始
    /// </summary>
    /// <param name="playerID"></param>
    public void StartStory6(int playerID)
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_OK);
        Global.GetTemporaryData().selectStage = 6;
        Global.GetTemporaryData().selectPlayer = (Constant.PlayerID)playerID;

        ManagerScript.GetInstance().ChangeScene("GameScene");
    }

    /// <summary>
    /// エンドレスモード開始
    /// </summary>
    /// <param name="playerID"></param>
    public void StartEndlessMode(int playerID)
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_OK);
        Global.GetTemporaryData().selectPlayer = (Constant.PlayerID)playerID;

        ManagerScript.GetInstance().ChangeScene("GameScene2");
    }

    /// <summary>
    /// キャラクター画面
    /// </summary>
    public void StartCharacterListScene()
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_OK);
        ManagerScript.GetInstance().ChangeScene("CharacterScene");
    }

    /// <summary>
    /// レコード画面
    /// </summary>
    public void StartRecordScene()
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_OK);
        ManagerScript.GetInstance().ChangeScene("PlayRecordsScene");
    }
    #endregion

    #region オプション
    /// <summary>
    /// 戦闘スキップ
    /// </summary>
    /// <param name="skip"></param>
    public void OptionBattleSkip(bool skip)
    {
        Global.GetSaveData().option.skipEffect = chkBattleSkip.isOn ? 1 : 0;
    }

    /// <summary>
    /// BGM音量
    /// </summary>
    /// <param name="vol"></param>
    public void OptionBgmVol(float vol)
    {
        Global.GetSaveData().option.bgmVolume = (int)slBgmVol.value;
        SoundManager.GetInstance().UpdateVol();
    }

    /// <summary>
    /// SE音量
    /// </summary>
    /// <param name="vol"></param>
    public void OptionSEVol(float vol)
    {
        Global.GetSaveData().option.seVolume = (int)slSEVol.value;
    }

    /// <summary>
    /// ハードモード
    /// </summary>
    /// <param name="_chk"></param>
    public void HardModeCheck(bool _chk)
    {
        Global.GetTemporaryData().difficulty = chkHardMode.isOn ? Constant.Difficulty.Hard : Constant.Difficulty.Normal;
    }
    #endregion

    #region 外部リンク
    /// <summary>
    /// 外部リンクボタン
    /// </summary>
    /// <param name="address"></param>
    public void LinkButton(string address)
    {
        Application.OpenURL(address);
    }
    #endregion

    /// <summary>
    /// test
    /// </summary>
    public void testButton()
    {
        PlayerPrefs.DeleteKey("stage0Clear");
        PlayerPrefs.DeleteAll();
    }
}
