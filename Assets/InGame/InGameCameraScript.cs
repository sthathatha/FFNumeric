using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InGameCameraScript : MonoBehaviour
{
    /// <summary>移動前</summary>
    private Vector3 beforePosition;
    /// <summary>移動後</summary>
    private Vector3 afterPosition;
    /// <summary>移動終了時間</summary>
    private float moveOverTime;
    /// <summary>現在時間</summary>
    private float moveNowTime;

    private bool moving;

    // Start is called before the first frame update
    void Start()
    {
        moving = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            moveNowTime += Time.deltaTime;
            var timePer = moveNowTime / moveOverTime;
            if (timePer >= 1)
            {
                moving = false;
                timePer = 1;
            }

            gameObject.transform.position = Vector3.Lerp(beforePosition, afterPosition, Util.SinCurve(timePer, Constant.SinCurveType.Both));
        }
    }

    /// <summary>
    /// 移動開始
    /// </summary>
    /// <param name="_position"></param>
    /// <param name="_time"></param>
    public void MoveTo(Vector3 _position, float _time)
    {
        beforePosition = gameObject.transform.position;
        afterPosition = new Vector3(_position.x, _position.y, -10);

        moveOverTime = _time;
        moveNowTime = 0;

        moving = true;
    }
}
