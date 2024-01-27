using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillUIScript : MonoBehaviour, IPointerClickHandler
{
    private const float GAUGE_HEIGHT_MAX = 100;

    /// <summary>�Q�[�W Height 0�`100</summary>
    private GameObject gauge;
    /// <summary>�\������</summary>
    private GameObject stockNum;

    /// <summary>
    /// �I�u�W�F�N�g������
    /// </summary>
    public void InitObject()
    {
        gauge = transform.Find("gauge").gameObject;
        stockNum = transform.Find("stockNum").gameObject;
    }

    /// <summary>
    /// �\��
    /// </summary>
    /// <param name="gaugeNow">�Q�[�W���ݒl</param>
    /// <param name="gaugeMax">�Q�[�W�ő�l</param>
    /// <param name="stockNow">�X�g�b�N���ݒl</param>
    /// <param name="stockMax">�X�g�b�N�ő�l</param>
    public void SetSkillState(int gaugeNow, int gaugeMax, int stockNow, int stockMax)
    {
        // �ő�0�ȉ���Auto
        if (stockMax <= 0)
        {
            stockNum.GetComponent<TMP_Text>().SetText("A");
        }
        else
        {
            stockNum.GetComponent<TMP_Text>().SetText(stockNow.ToString());
        }

        // �X�g�b�NMAX���̓Q�[�WMAX
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
    /// �N���b�N��
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        var system = GameObject.Find("GameSystem").GetComponent<GameSceneBaseScript>();
        if (system == null) return;

        system.SkillUIClickEvent();
    }
}
