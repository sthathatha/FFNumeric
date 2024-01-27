using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InGameCameraScript : MonoBehaviour
{
    /// <summary>�ړ��O</summary>
    private Vector3 beforePosition;
    /// <summary>�ړ���</summary>
    private Vector3 afterPosition;
    /// <summary>�ړ��I������</summary>
    private float moveOverTime;
    /// <summary>���ݎ���</summary>
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
    /// �ړ��J�n
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
