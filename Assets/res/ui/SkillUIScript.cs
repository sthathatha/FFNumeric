using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillUIScript : MonoBehaviour, IPointerClickHandler
{
    private const float GAUGE_HEIGHT_MAX = 100;

    /// <summary>ゲージ Height 0～100</summary>
    private GameObject gauge;
    /// <summary>表示文字</summary>
    private GameObject stockNum;

    /// <summary>
    /// オブジェクト初期化
    /// </summary>
    public void InitObject()
    {
        gauge = transform.Find("gauge").gameObject;
        stockNum = transform.Find("stockNum").gameObject;
    }

    /// <summary>
    /// 表示
    /// </summary>
    /// <param name="gaugeNow">ゲージ現在値</param>
    /// <param name="gaugeMax">ゲージ最大値</param>
    /// <param name="stockNow">ストック現在値</param>
    /// <param name="stockMax">ストック最大値</param>
    public void SetSkillState(int gaugeNow, int gaugeMax, int stockNow, int stockMax)
    {
        // 最大0以下はAuto
        if (stockMax <= 0)
        {
            stockNum.GetComponent<TMP_Text>().SetText("A");
        }
        else
        {
            stockNum.GetComponent<TMP_Text>().SetText(stockNow.ToString());
        }

        // ストックMAX時はゲージMAX
        float height;
        if (stockNow >= stockMax && stockMax > 0)
        {
            height = GAUGE_HEIGHT_MAX;
        } else
        {
            height = GAUGE_HEIGHT_MAX * gaugeNow / gaugeMax;
        }

        gauge.GetComponent<RectTransform>().sizeDelta = new Vector2(GAUGE_HEIGHT_MAX, height);
    }

    /// <summary>
    /// クリック時
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        var system = GameObject.Find("GameSystem").GetComponent<GameSceneBaseScript>();
        if (system == null) return;

        system.SkillUIClickEvent();
    }
}
