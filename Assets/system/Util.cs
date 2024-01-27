using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    private static readonly Vector2Int NORMAL_RIGHT = new Vector2Int(1, 0);
    private static readonly Vector2Int NORMAL_RIGHTUP = new Vector2Int(1, 1);
    private static readonly Vector2Int NORMAL_RIGHTDOWN = new Vector2Int(0, -1);
    private static readonly Vector2Int NORMAL_LEFT = new Vector2Int(-1, 0);
    private static readonly Vector2Int NORMAL_LEFTUP = new Vector2Int(0, 1);
    private static readonly Vector2Int NORMAL_LEFTDOWN = new Vector2Int(-1, -1);

    /// <summary>
    /// フィールド位置から基本座標を計算
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static Vector3 GetBasePosition(int x, int y)
    {
        var posX = x * Constant.CELL_POSITION_X - y * Constant.CELL_POSITION_X_Y_ADD;
        var posY = y * Constant.CELL_POSITION_Y;

        return new Vector3(posX, posY, 0);
    }

    /// <summary>
    /// ２地点の距離を計算
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <returns></returns>
    public static int CalcLocationDistance(Vector2Int pos1, Vector2Int pos2)
    {
        Vector2Int dist = pos2 - pos1;
        int absX = Mathf.Abs(dist.x);
        int absY = Mathf.Abs(dist.y);

        // 符号が逆の場合は絶対値の合計
        if (dist.x > 0 && dist.y < 0 || dist.x < 0 && dist.y > 0)
        {
            return absX + absY;
        }

        // それ以外の方向は大きい方
        return absX > absY ? absX : absY;
    }

    /// <summary>
    /// 周囲６マスを取得
    /// </summary>
    /// <returns></returns>
    public static List<Vector2Int> GetAroundLocations(Vector2Int _center)
    {
        var ret = new List<Vector2Int>
        {
            _center + NORMAL_RIGHT,
            _center + NORMAL_RIGHTDOWN,
            _center + NORMAL_LEFTDOWN,
            _center + NORMAL_LEFT,
            _center + NORMAL_LEFTUP,
            _center + NORMAL_RIGHTUP
        };

        return ret;
    }

    /// <summary>
    /// 方向の左右を取得
    /// </summary>
    /// <param name="_normal"></param>
    /// <returns></returns>
    public static List<Vector2Int> GetLRWingLocations(Vector2Int _normal)
    {
        var ret = new List<Vector2Int>();

        if (_normal.Equals(NORMAL_RIGHT))
        {
            ret.Add(NORMAL_RIGHTUP);
            ret.Add(NORMAL_RIGHTDOWN);
        }
        else if (_normal.Equals(NORMAL_RIGHTDOWN))
        {
            ret.Add(NORMAL_RIGHT);
            ret.Add(NORMAL_LEFTDOWN);
        }
        else if (_normal.Equals(NORMAL_LEFTDOWN))
        {
            ret.Add(NORMAL_RIGHTDOWN);
            ret.Add(NORMAL_LEFT);
        }
        else if (_normal.Equals(NORMAL_LEFT))
        {
            ret.Add(NORMAL_LEFTDOWN);
            ret.Add(NORMAL_LEFTUP);
        }
        else if (_normal.Equals(NORMAL_LEFTUP))
        {
            ret.Add(NORMAL_LEFT);
            ret.Add(NORMAL_RIGHTUP);
        }
        else if (_normal.Equals(NORMAL_RIGHTUP))
        {
            ret.Add(NORMAL_LEFTUP);
            ret.Add(NORMAL_RIGHT);
        }

        return ret;
    }

    /// <summary>
    /// HPの値を調整
    /// 小数部分を削除
    /// HP最大値を超えたら最大値に設定
    /// </summary>
    /// <param name="_hp"></param>
    public static double FixHpValue(double _hp)
    {
        if (_hp >= Constant.HP_LIMIT) { return Constant.HP_LIMIT; }

        return System.Math.Floor(_hp);
    }

    /// <summary>
    /// シールドの値を調整
    /// 小数部分を削除
    /// シールド最大値を超えたら最大値に設定
    /// </summary>
    /// <param name="_sp"></param>
    /// <returns></returns>
    public static double FixShieldValue(double _sp)
    {
        if (_sp >= Constant.SHIELD_LIMIT) { return Constant.SHIELD_LIMIT; }

        return System.Math.Floor(_sp);
    }

    /// <summary>
    /// HPとシールドの値を表示用文字列に変換
    /// </summary>
    /// <param name="_val"></param>
    /// <returns></returns>
    public static string ToHpSpViewString(double _val)
    {
        // 1000000以上になると有効3桁で四捨五入してe+表記
        if (_val > Constant.HPSP_VIEW_EPLUS_LIM)
        {
            return _val.ToString("e1");
        }

        return System.Math.Floor(_val).ToString();
    }

    /// <summary>
    /// サインカーブのfloatに変換
    /// </summary>
    /// <param name="_val">0～1</param>
    /// <param name="_type">加減速選択</param>
    /// <returns>0～1</returns>
    public static float SinCurve(float _val, Constant.SinCurveType _type)
    {
        float theta;
        switch (_type)
        {
            case Constant.SinCurveType.Accel:
                theta = Mathf.PI * (_val / 2f - 0.5f);
                return Mathf.Sin(theta) + 1f;
            case Constant.SinCurveType.Decel:
                theta = Mathf.PI * (_val / 2f);
                return Mathf.Sin(theta);
            case Constant.SinCurveType.Both:
                theta = Mathf.PI * (_val - 0.5f);
                return (Mathf.Sin(theta) + 1f) / 2f;
        }

        return 0f;
    }

    /// <summary>
    /// 補間値
    /// </summary>
    /// <param name="_rate"></param>
    /// <param name="_val1"></param>
    /// <param name="_val2"></param>
    /// <returns></returns>
    public static float CalcBetweenFloat(float _rate, float _val1, float _val2)
    {
        return _val1 + (_val2 - _val1) * _rate;
    }
}
