using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayRecordsScript : MonoBehaviour
{
    public TMP_Text drowsNormalHP;
    public TMP_Text drowsNormalTurn;
    public TMP_Text drowsHardHP;
    public TMP_Text drowsHardTurn;

    public TMP_Text erapsNormalHP;
    public TMP_Text erapsNormalTurn;
    public TMP_Text erapsHardHP;
    public TMP_Text erapsHardTurn;

    public TMP_Text exaNormalHP;
    public TMP_Text exaNormalTurn;
    public TMP_Text exaHardHP;
    public TMP_Text exaHardTurn;

    public TMP_Text worraNormalHP;
    public TMP_Text worraNormalTurn;
    public TMP_Text worraHardHP;
    public TMP_Text worraHardTurn;

    public TMP_Text koobNormalHP;
    public TMP_Text koobNormalTurn;
    public TMP_Text koobHardHP;
    public TMP_Text koobHardTurn;

    public TMP_Text youNormalHP;
    public TMP_Text youNormalTurn;
    public TMP_Text youHardHP;
    public TMP_Text youHardTurn;

    /// <summary>
    /// èâä˙âª
    /// </summary>
    void Start()
    {
        var rec = Global.GetSaveData();
        drowsNormalHP.SetText(Util.ToHpSpViewString(rec.recordDrows.normalMaxHp));
        drowsNormalTurn.SetText(rec.recordDrows.normalTurn.ToString());
        drowsHardHP.SetText(Util.ToHpSpViewString(rec.recordDrows.hardMaxHp));
        drowsHardTurn.SetText(rec.recordDrows.hardTurn.ToString());

        erapsNormalHP.SetText(Util.ToHpSpViewString(rec.recordEraps.normalMaxHp));
        erapsNormalTurn.SetText(rec.recordEraps.normalTurn.ToString());
        erapsHardHP.SetText(Util.ToHpSpViewString(rec.recordEraps.hardMaxHp));
        erapsHardTurn.SetText(rec.recordEraps.hardTurn.ToString());

        exaNormalHP.SetText(Util.ToHpSpViewString(rec.recordExa.normalMaxHp));
        exaNormalTurn.SetText(rec.recordExa.normalTurn.ToString());
        exaHardHP.SetText(Util.ToHpSpViewString(rec.recordExa.hardMaxHp));
        exaHardTurn.SetText(rec.recordExa.hardTurn.ToString());

        worraNormalHP.SetText(Util.ToHpSpViewString(rec.recordWorra.normalMaxHp));
        worraNormalTurn.SetText(rec.recordWorra.normalTurn.ToString());
        worraHardHP.SetText(Util.ToHpSpViewString(rec.recordWorra.hardMaxHp));
        worraHardTurn.SetText(rec.recordWorra.hardTurn.ToString());

        koobNormalHP.SetText(Util.ToHpSpViewString(rec.recordKoob.normalMaxHp));
        koobNormalTurn.SetText(rec.recordKoob.normalTurn.ToString());
        koobHardHP.SetText(Util.ToHpSpViewString(rec.recordKoob.hardMaxHp));
        koobHardTurn.SetText(rec.recordKoob.hardTurn.ToString());

        youNormalHP.SetText(Util.ToHpSpViewString(rec.recordYou.normalMaxHp));
        youNormalTurn.SetText(rec.recordYou.normalTurn.ToString());
        youHardHP.SetText(Util.ToHpSpViewString(rec.recordYou.hardMaxHp));
        youHardTurn.SetText(rec.recordYou.hardTurn.ToString());
    }

    /// <summary>
    /// ñﬂÇÈ
    /// </summary>
    public void BackButton()
    {
        SoundManager.GetInstance().PlaySE(SoundManager.SeID.SYSTEM_CANCEL);
        ManagerScript.GetInstance().ChangeScene("TitleScene");
    }
}
