using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 時間変化する値
/// </summary>
public class DeltaFloat
{
    /// <summary>
    /// 変化タイプ
    /// </summary>
    public enum MoveType : int
    {
        /// <summary>直線</summary>
        LINE = 0,
        /// <summary>加速</summary>
        ACCEL,
        /// <summary>減速</summary>
        DECEL,
        /// <summary>加減速</summary>
        BOTH,
    }

    private MoveType moveType;
    private float endTime;
    private float nowTime;
    private float beforeValue;
    private float nowValue;
    private float afterValue;
    private bool active;

    public DeltaFloat()
    {
        nowTime = 0;
        endTime = 0;
        active = false;
    }

    /// <summary>
    /// 移動中
    /// </summary>
    /// <returns></returns>
    public bool IsActive() { return active; }

    /// <summary>
    /// 現在値
    /// </summary>
    /// <returns></returns>
    public float Get() { return nowValue; }

    /// <summary>
    /// 即時変更
    /// </summary>
    /// <param name="val"></param>
    public void Set(float val)
    {
        beforeValue = val;
        afterValue = val;
        nowValue = val;
        nowTime = 0;
        endTime = 0;
        active = false;
    }

    /// <summary>
    /// 指定時間で値を変える
    /// </summary>
    /// <param name="_val"></param>
    /// <param name="_time"></param>
    /// <param name="_moveType"></param>
    public void MoveTo(float _val, float _time, MoveType _moveType)
    {
        beforeValue = nowValue;
        afterValue = _val;
        nowTime = 0;
        endTime = _time;

        active = true;
        moveType = _moveType;
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="deltaTime"></param>
    public void Update(float deltaTime)
    {
        if (!IsActive())
        {
            return;
        }

        nowTime += deltaTime;
        if (nowTime > endTime)
        {
            nowValue = afterValue;
            active = false;
            return;
        }


        var timePer = nowTime / endTime;
        float valPer = timePer;
        switch (moveType)
        {
            case MoveType.ACCEL:
                valPer = Util.SinCurve(timePer, Constant.SinCurveType.Accel);
                break;
            case MoveType.DECEL:
                valPer = Util.SinCurve(timePer, Constant.SinCurveType.Decel);
                break;
            case MoveType.BOTH:
                valPer = Util.SinCurve(timePer, Constant.SinCurveType.Both);
                break;
        }

        nowValue = Util.CalcBetweenFloat(valPer, beforeValue, afterValue);
    }
}
