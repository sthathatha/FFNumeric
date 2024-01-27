using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static Unity.VisualScripting.Member;

/// <summary>
/// BGM・SEの管理
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager _instance = null;
    private static GameObject bgmSource = null;

    #region 列挙
    /// <summary>
    /// BGM
    /// </summary>
    public enum BgmID : int
    {
        TITLE = 0,
        GAME_DROWS,
        GAME_ERAPS,
        GAME_EXA,
        GAME_WORRA,
        GAME_KOOB,
        GAME_YOU,

        NONE = -1,
    }

    /// <summary>
    /// SE
    /// </summary>
    public enum SeID : int
    {
        ATTACK_DROWS = 0,
        ATTACK_ERAPS,
        ATTACK_EXA,
        ATTACK_WORRA,
        ATTACK_KOOB,
        ATTACK_YOU,
        DAMAGE_NORMAL,
        DAMAGE_SHIELD,
        GAMEOVER,
        SKILL_EFFECT,
        SKILL_EXA,
        SKILL_KOOB,
        SKILL_YOU,
        SYSTEM_OK,
        SYSTEM_CANCEL,
        SYSTEM_MOVE,
    }
    #endregion

    private List<GameObject> seSources;

    private ResourceRequest bgmResource;
    private BgmID playID;

    #region 初期化
    public static SoundManager GetInstance()
    {
        if (_instance == null)
        {
            bgmSource = new GameObject();
            bgmSource.isStatic = true;
            bgmSource.AddComponent<AudioSource>();
            bgmSource.AddComponent<SoundManager>();

            var audioSource = bgmSource.GetComponent<AudioSource>();
            audioSource.loop = true;

            _instance = bgmSource.GetComponent<SoundManager>();
        }
        return _instance;
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public SoundManager()
    {
        seSources = new List<GameObject>();

        playID = BgmID.NONE;
    }
    #endregion

    #region 使用

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="_id"></param>
    public void PlayBGM(BgmID _id)
    {
        if (playID == _id) { return; }

        StartCoroutine(PlayBGMCoroutine(_id));
    }

    /// <summary>
    /// BGM再生コルーチン
    /// </summary>
    /// <param name="_id"></param>
    /// <returns></returns>
    private IEnumerator PlayBGMCoroutine(BgmID _id)
    {
        var source = bgmSource.GetComponent<AudioSource>();

        if (playID != BgmID.NONE)
        {
            var fade = new DeltaFloat();
            fade.Set(source.volume);
            fade.MoveTo(0f, 1f, DeltaFloat.MoveType.LINE);

            while(fade.IsActive())
            {
                fade.Update(Time.deltaTime);
                source.volume = fade.Get();

                yield return null;
            }

            source.Stop();
            Resources.UnloadAsset(bgmResource.asset);
        }

        playID = _id;

        var bgmName = "";
        switch (playID)
        {
            case BgmID.TITLE:
                bgmName = "amenoshita";
                break;
            case BgmID.GAME_DROWS:
                bgmName = "prairie4";
                break;
            case BgmID.GAME_ERAPS:
                bgmName = "retroRPGBattle2";
                break;
            case BgmID.GAME_EXA:
                bgmName = "desert6";
                break;
            case BgmID.GAME_WORRA:
                bgmName = "deepWoods4";
                break;
            case BgmID.GAME_KOOB:
                bgmName = "breeze4";
                break;
            case BgmID.GAME_YOU:
                bgmName = "kengeki";
                break;
        }
        bgmResource = Resources.LoadAsync<AudioClip>("Bgm/" + bgmName);
        yield return bgmResource;

        var clip = bgmResource.asset as AudioClip;

        UpdateVol();
        source.clip = clip;
        source.PlayDelayed(0.5f);
    }

    /// <summary>
    /// オプション設定に従って音量設定
    /// </summary>
    public void UpdateVol()
    {
        var source = bgmSource.GetComponent<AudioSource>();
        source.volume = Global.GetSaveData().option.bgmVolume / 127f * 0.25f; //PeriTuneのデータは音量大きいので小さめ
    }

    /// <summary>
    /// SE
    /// </summary>
    /// <param name="_id"></param>
    public void PlaySE(SeID _id)
    {
        StartCoroutine(PlaySECoroutine(_id));
    }

    /// <summary>
    /// SEコルーチン
    /// </summary>
    /// <param name="_id"></param>
    /// <returns></returns>
    private IEnumerator PlaySECoroutine(SeID _id)
    {
        var seName = "";
        switch (_id)
        {
            case SeID.GAMEOVER: seName = "gameover"; break;
            case SeID.DAMAGE_SHIELD: seName = "damage_shield"; break;
            case SeID.DAMAGE_NORMAL: seName = "damage_normal"; break;
            case SeID.ATTACK_DROWS: seName = "attack_drows"; break;
            case SeID.ATTACK_ERAPS: seName = "attack_eraps"; break;
            case SeID.ATTACK_EXA: seName = "attack_exa"; break;
            case SeID.ATTACK_WORRA: seName = "attack_worra"; break;
            case SeID.ATTACK_KOOB: seName = "attack_koob"; break;
            case SeID.ATTACK_YOU: seName = "attack_you"; break;
            case SeID.SKILL_EFFECT: seName = "skill_effect"; break;
            case SeID.SKILL_EXA: seName = "skill_exa"; break;
            case SeID.SKILL_KOOB: seName = "skill_koob"; break;
            case SeID.SKILL_YOU: seName = "skill_you"; break;
            case SeID.SYSTEM_OK: seName = "system_ok"; break;
            case SeID.SYSTEM_CANCEL: seName = "system_cancel"; break;
            case SeID.SYSTEM_MOVE: seName = "system_move"; break;
        }

        var seResource = Resources.LoadAsync<AudioClip>("SE/" + seName);
        yield return seResource;

        var clip = seResource.asset as AudioClip;
        var vol = Global.GetSaveData().option.seVolume / 127f;

        var dmyObj = new GameObject(seName);
        dmyObj.AddComponent<AudioSource>();
        var source = dmyObj.GetComponent<AudioSource>();
        source.clip = clip;
        source.volume = vol;
        source.PlayOneShot(clip, 1f);

        yield return new WaitWhile(() => source.isPlaying);
        Destroy(dmyObj);

        Resources.UnloadAsset(seResource.asset);
    }

    #endregion
}
