using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constant
{
    public enum CharacterType : int
    {
        Empty = 0,
        Player,
        Enemy,
        Boss,
    }

    public enum GameState : int
    {
        /// <summary>読み込み中</summary>
        Loading = 0,
        /// <summary>操作待ち</summary>
        Idle,
        /// <summary>戦闘中</summary>
        Battle,
        /// <summary>GameOverフェード中</summary>
        GameOverFade,
        /// <summary>GameOver表示</summary>
        GameOver,
        /// <summary>スキル使用選択中</summary>
        SkillWait,
        /// <summary>ステージクリア</summary>
        GameClear,
    }

    /// <summary>
    /// プレイヤーID
    /// </summary>
    public enum PlayerID : int
    {
        Drows = 0,
        Eraps,
        Exa,
        Worra,
        Koob,
        You,
    }

    /// <summary>
    /// キャラ名文字列小文字
    /// </summary>
    /// <param name="playerID"></param>
    /// <returns></returns>
    public static string GetPlayerNameE(PlayerID playerID)
    {
        switch (playerID)
        {
            case PlayerID.Drows: return "drows";
            case PlayerID.Eraps: return "eraps";
            case PlayerID.Exa: return "exa";
            case PlayerID.Worra: return "worra";
            case PlayerID.Koob: return "koob";
            case PlayerID.You: return "you";
        }

        return "";
    }

    /// <summary>
    /// エンドレスモード難易度
    /// </summary>
    public enum Difficulty : int
    {
        Normal = 0,
        Hard,
    }

    /// <summary>
    /// サインカーブ
    /// </summary>
    public enum SinCurveType : int
    {
        /// <summary>加速</summary>
        Accel = 0,
        /// <summary>減速</summary>
        Decel,
        /// <summary>加減速</summary>
        Both,
    }

    /// <summary>セル位置計算X座標</summary>
    public const float CELL_POSITION_X = 3.2f;
    /// <summary>セル位置計算X座標にYの加算分</summary>
    public const float CELL_POSITION_X_Y_ADD = 1.6f;
    /// <summary>セル位置計算Y座標</summary>
    public const float CELL_POSITION_Y = 2.0f;

    /// <summary>基準位置からキャラObjectの位置差分</summary>
    public static readonly Vector3 CHARACTER_CELL_OFFSET = new(0, -0.75f, 0);

    /// <summary>エンドレスモード　X座標作成量</summary>
    public const int ENDLESS_FIELD_X = 10;
    /// <summary>エンドレスモード　Y座標作成量</summary>
    public const int ENDLESS_FIELD_Y = 9;

    /// <summary>エンドレスモード　敵が復活する時間</summary>
    public const int ENDLESS_ENEMY_POP_TURN = 3;

    /// <summary>HPの最大値</summary>
    public const double HP_LIMIT = 1e300;
    /// <summary>シールドの最大値</summary>
    public const double SHIELD_LIMIT = 1e305;

    /// <summary>これを超えるとe+表記にする</summary>
    public const double HPSP_VIEW_EPLUS_LIM = 999999;

    /// <summary>敵HP自動生成の最低値</summary>
    public const double ENEMY_HP_MIN = 15;

    /// <summary>
    /// キャラクター定数用
    /// </summary>
    public struct CharacterParams
    {
        /// <summary>初期HP</summary>
        readonly public double InitHP;

        /// <summary>スキルゲージ長さ</summary>
        readonly public int SkillGaugeLength;
        /// <summary>スキルストック数</summary>
        readonly public int SkillStockMax;

        public CharacterParams(double _initHp, int _skillGaugeLen, int _skillStockMax)
        {
            InitHP = _initHp;
            SkillGaugeLength = _skillGaugeLen;
            SkillStockMax = _skillStockMax;
        }
    }

    /// <summary>ドロシー　パラメータ</summary>
    public static readonly CharacterParams DROWS_PARAM = new CharacterParams(25, 50, 1);
    /// <summary>エラ　パラメータ</summary>
    public static readonly CharacterParams ERAPS_PARAM = new CharacterParams(12, 40, -1);
    /// <summary>エグザ　パラメータ</summary>
    public static readonly CharacterParams EXA_PARAM = new CharacterParams(15, 30, 3);
    /// <summary>ウーラ　パラメータ</summary>
    public static readonly CharacterParams WORRA_PARAM = new CharacterParams(10, 48, -1);
    /// <summary>クー　パラメータ</summary>
    public static readonly CharacterParams KOOB_PARAM = new CharacterParams(8, 50, 1);
    /// <summary>悠　パラメータ</summary>
    public static readonly CharacterParams YOU_PARAM = new CharacterParams(20, 40, 1);

    /// <summary>ターン終了時敵成長</summary>
    public const double ENEMY_TURN_POWUP_RATE = 0.5;

    /// <summary>能力　ドロシー　連続攻撃上限</summary>
    public const int SKILL_DROWS_ATTACK_LIMIT = 10;
    /// <summary>能力　ドロシー　スキル中攻撃倍率</summary>
    public const double SKILL_DROWS_ATTACK_RATE = 100.0;
    /// <summary>能力　ドロシー　スキル持続時間</summary>
    public const int SKILL_DROWS_KEEP_TURN = 3;

    /// <summary>能力　エラ　初期シールド成長速度</summary>
    public const double SKILL_ERAPS_INIT_SP_RATE = 0.2;
    /// <summary>能力　エラ　スキル使用成長速度アップ</summary>
    public const double SKILL_ERAPS_SP_UPDATE_RATE = 0.1;
    /// <summary>能力　エラ　ターン開始時生成シールド</summary>
    public const double SKILL_ERAPS_ADD_SP_RATE = 0.5;

    /// <summary>能力　エグザ　左右に与えるダメージ</summary>
    public const double SKILL_EXA_SIDE_ATTACK_RATE = 0.8;

    /// <summary>能力　ウーラ　初期射程</summary>
    public const int SKILL_WORRA_INIT_RANGE = 1;
    /// <summary>能力　ウーラ　最大射程</summary>
    public const int SKILL_WORRA_MAX_RANGE = 4;
    /// <summary>能力　ウーラ　初期攻撃回数</summary>
    public const int SKILL_WORRA_INIT_ATTACK_COUNT = 2;

    /// <summary>能力　クー　射程</summary>
    public const int SKILL_KOOB_ATTACK_RANGE = 2;
    /// <summary>能力　クー　成長率</summary>
    public const double SKILL_KOOB_POWUP_RATE = 1.5;
    /// <summary>能力　クー　ラグナロク攻撃力</summary>
    public const double SKILL_KOOB_RAGNAROK_RATE = 3.0;
    /// <summary>能力　クー　ラグナロク範囲</summary>
    public const int SKILL_KOOB_RAGNAROK_RANGE = 3;
    /// <summary>能力　クー　ラグナロク反撃不可ターン</summary>
    public const int SKILL_KOOB_RAGNAROK_PARALYZE_TURN = 2;
    /// <summary>能力　クー　ラグナロク成長不可ターン</summary>
    public const int SKILL_KOOB_RAGNAROK_POW_INVALID_TURN = 2;

    /// <summary>能力　悠　攻撃力倍率</summary>
    public const double SKILL_YOU_ATTACK_RATE = 1.5;
    /// <summary>能力　悠　被ダメージ倍率</summary>
    public const double SKILL_YOU_DAMAGE_RATE = 0.6;
    /// <summary>能力　悠　居合抜き射程</summary>
    public const int SKILL_YOU_KILL_RANGE = 3;


    /// <summary>カメラ移動時間</summary>
    public const float CAMERA_MOVE_TIME = 0.3f;

    /// <summary>最終ステージ番号</summary>
    public const int LAST_STAGE = 6;

    /// <summary>背景色ドロシー</summary>
    public static readonly Color COLOR_BG_DROWS = new Color(0.4f, 0.6f, 0.3f);
    /// <summary>背景色エラ</summary>
    public static readonly Color COLOR_BG_ERAPS = new Color(0.6f, 0.6f, 0.6f);
    /// <summary>背景色エグザ</summary>
    public static readonly Color COLOR_BG_EXA = new Color(0.5f, 0.5f, 0.1f);
    /// <summary>背景色ウーラ</summary>
    public static readonly Color COLOR_BG_WORRA = new Color(0.1f, 0.5f, 0.1f);
    /// <summary>背景色クー</summary>
    public static readonly Color COLOR_BG_KOOB = new Color(0.2f, 0.2f, 0.5f);
    /// <summary>背景色悠</summary>
    public static readonly Color COLOR_BG_YOU = new Color(0.2f, 0.2f, 0.2f);
}
