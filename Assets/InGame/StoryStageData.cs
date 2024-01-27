using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// ストーリーモード用フィールド設定作成
/// </summary>
public class StoryStageData
{
    private struct LineHpList
    {
        int leftX;
        List<int> hpList;
    }

    /// <summary>
    /// 戻り値用
    /// </summary>
    public struct FieldInitData
    {
        public Vector2Int loc;
        public double initHp;
        public Constant.CharacterType type;
    }

    /// <summary>
    /// 作成
    /// </summary>
    /// <returns></returns>
    public static List<FieldInitData> GetFieldList()
    {
        // ボス位置
        var bossLoc = new Vector2Int(0, 0);
        // HPリスト
        Dictionary<Vector2Int, List<double>> dict = null;

        switch (Global.GetTemporaryData().selectStage)
        {
            case 1:
                dict = new Dictionary<Vector2Int, List<double>>
                {
                    { new Vector2Int(2, 1), new List<double>() { 43, 66, 130 } },
                    { new Vector2Int(1, 0), new List<double>() { 30, 26, 53, 100 } },
                    { new Vector2Int(1, -1), new List<double>() { 60, 124, 145, 133 } },
                    { new Vector2Int(1, -2), new List<double>() {  1301, 671, 312, 266 } },
                    { new Vector2Int(1, -3), new List<double>() {   1750, 644, 1677, 2290 } },
                    { new Vector2Int(1, -4), new List<double>() {    3104, 5525, 8911, 99999 } },
                };
                bossLoc.x = 4; bossLoc.y = -4;
                break;
            case 2:
                dict = new Dictionary<Vector2Int, List<double>>
                {
                    { new Vector2Int(1, 1), new List<double>() {  13, 28, 40, 77, 98 } },
                    { new Vector2Int(1, 0), new List<double>() {    0,  31,  55,  0, 14000 } },
                    { new Vector2Int(0, -1), new List<double>() { 17, 22, 30, 51, 89 } },
                };
                bossLoc.x = 5; bossLoc.y = 0;
                break;
            case 3:
                dict = new Dictionary<Vector2Int, List<double>>
                {
                    { new Vector2Int(2, 3), new List<double>() {        32,   123 } },
                    { new Vector2Int(1, 2), new List<double>() {      91, 149,   201, 220 } },
                    { new Vector2Int(0, 1), new List<double>() {     13, 45, 123456, 256 } },
                    { new Vector2Int(-1, 0), new List<double>() {   9, 0,  586, 392, 277 } },
                    { new Vector2Int(-1, -1), new List<double>() {    16, 38, 301, 418 } },
                };
                bossLoc.x = 2; bossLoc.y = 1;
                break;
            case 4:
                dict = new Dictionary<Vector2Int, List<double>>
                {
                    { new Vector2Int(2, 1), new List<double>() {     18, 0, 370 } },
                    { new Vector2Int(1, 0), new List<double>() {   16, 25, 189, 703, 66666 } },
                    { new Vector2Int(0, -1), new List<double>() { 22, 0, 70,  0,  1514 } },
                };
                bossLoc.x = 5; bossLoc.y = 0;
                break;
            case 5:
                dict = new Dictionary<Vector2Int, List<double>>
                {
                    { new Vector2Int(-2, 1), new List<double>() {     56,   14,     453, 25611, 51, 13147 } },
                    { new Vector2Int(-3, 0), new List<double>() { 2.5e7, 187804, 40157,   0,  1942, 13, 4177 } },
                    { new Vector2Int(-3, -1), new List<double>() {    31,   115,    906,  123, 8765, 20 } },
                };
                bossLoc.x = -3; bossLoc.y = 0;
                break;
            case 6:
                dict = new Dictionary<Vector2Int, List<double>>
                {
                    { new Vector2Int(2, 4), new List<double>() {     51,   83,  127 } },
                    { new Vector2Int(1, 3), new List<double>() {   35,  219,   271,  125 } },
                    { new Vector2Int(0, 2), new List<double>() { 24, 186, 90000,  172,  101 } },
                    { new Vector2Int(0, 1), new List<double>() {   11,  33,   124,  47 } },
                    { new Vector2Int(0, 0), new List<double>() {     0,    16,   30 } },
                };
                bossLoc.x = 2; bossLoc.y = 2;
                break;
        }

        // 返却用の形に変換
        var ret = new List<FieldInitData>();
        foreach (var line in dict)
        {
            var loc = line.Key;
            foreach (var hp in line.Value)
            {
                if (hp > 0)
                {
                    var addRet = new FieldInitData
                    {
                        initHp = hp,
                        loc = loc,
                        type = loc == bossLoc ? Constant.CharacterType.Boss : Constant.CharacterType.Enemy
                    };
                    ret.Add(addRet);
                }

                loc.x += 1;
            }
        }

        return ret;
    }
}
