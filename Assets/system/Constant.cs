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
        /// <summary>�ǂݍ��ݒ�</summary>
        Loading = 0,
        /// <summary>����҂�</summary>
        Idle,
        /// <summary>�퓬��</summary>
        Battle,
        /// <summary>GameOver�t�F�[�h��</summary>
        GameOverFade,
        /// <summary>GameOver�\��</summary>
        GameOver,
        /// <summary>�X�L���g�p�I��</summary>
        SkillWait,
        /// <summary>�X�e�[�W�N���A</summary>
        GameClear,
    }

    /// <summary>
    /// �v���C���[ID
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
    /// �L�����������񏬕���
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
    /// �G���h���X���[�h��Փx
    /// </summary>
    public enum Difficulty : int
    {
        Normal = 0,
        Hard,
    }

    /// <summary>
    /// �T�C���J�[�u
    /// </summary>
    public enum SinCurveType : int
    {
        /// <summary>����</summary>
        Accel = 0,
        /// <summary>����</summary>
        Decel,
        /// <summary>������</summary>
        Both,
    }

    /// <summary>�Z���ʒu�v�ZX���W</summary>
    public const float CELL_POSITION_X = 3.2f;
    /// <summary>�Z���ʒu�v�ZX���W��Y�̉��Z��</summary>
    public const float CELL_POSITION_X_Y_ADD = 1.6f;
    /// <summary>�Z���ʒu�v�ZY���W</summary>
    public const float CELL_POSITION_Y = 2.0f;

    /// <summary>��ʒu����L����Object�̈ʒu����</summary>
    public static readonly Vector3 CHARACTER_CELL_OFFSET = new(0, -0.75f, 0);

    /// <summary>�G���h���X���[�h�@X���W�쐬��</summary>
    public const int ENDLESS_FIELD_X = 10;
    /// <summary>�G���h���X���[�h�@Y���W�쐬��</summary>
    public const int ENDLESS_FIELD_Y = 9;

    /// <summary>�G���h���X���[�h�@�G���������鎞��</summary>
    public const int ENDLESS_ENEMY_POP_TURN = 3;

    /// <summary>HP�̍ő�l</summary>
    public const double HP_LIMIT = 1e300;
    /// <summary>�V�[���h�̍ő�l</summary>
    public const double SHIELD_LIMIT = 1e305;

    /// <summary>����𒴂����e+�\�L�ɂ���</summary>
    public const double HPSP_VIEW_EPLUS_LIM = 999999;

    /// <summary>�GHP���������̍Œ�l</summary>
    public const double ENEMY_HP_MIN = 15;

    /// <summary>
    /// �L�����N�^�[�萔�p
    /// </summary>
    public struct CharacterParams
    {
        /// <summary>����HP</summary>
        readonly public double InitHP;

        /// <summary>�X�L���Q�[�W����</summary>
        readonly public int SkillGaugeLength;
        /// <summary>�X�L���X�g�b�N��</summary>
        readonly public int SkillStockMax;

        public CharacterParams(double _initHp, int _skillGaugeLen, int _skillStockMax)
        {
            InitHP = _initHp;
            SkillGaugeLength = _skillGaugeLen;
            SkillStockMax = _skillStockMax;
        }
    }

    /// <summary>�h���V�[�@�p�����[�^</summary>
    public static readonly CharacterParams DROWS_PARAM = new CharacterParams(25, 50, 1);
    /// <summary>�G���@�p�����[�^</summary>
    public static readonly CharacterParams ERAPS_PARAM = new CharacterParams(12, 40, -1);
    /// <summary>�G�O�U�@�p�����[�^</summary>
    public static readonly CharacterParams EXA_PARAM = new CharacterParams(15, 30, 3);
    /// <summary>�E�[���@�p�����[�^</summary>
    public static readonly CharacterParams WORRA_PARAM = new CharacterParams(10, 48, -1);
    /// <summary>�N�[�@�p�����[�^</summary>
    public static readonly CharacterParams KOOB_PARAM = new CharacterParams(8, 50, 1);
    /// <summary>�I�@�p�����[�^</summary>
    public static readonly CharacterParams YOU_PARAM = new CharacterParams(20, 40, 1);

    /// <summary>�^�[���I�����G����</summary>
    public const double ENEMY_TURN_POWUP_RATE = 0.5;

    /// <summary>�\�́@�h���V�[�@�A���U�����</summary>
    public const int SKILL_DROWS_ATTACK_LIMIT = 10;
    /// <summary>�\�́@�h���V�[�@�X�L�����U���{��</summary>
    public const double SKILL_DROWS_ATTACK_RATE = 100.0;
    /// <summary>�\�́@�h���V�[�@�X�L����������</summary>
    public const int SKILL_DROWS_KEEP_TURN = 3;

    /// <summary>�\�́@�G���@�����V�[���h�������x</summary>
    public const double SKILL_ERAPS_INIT_SP_RATE = 0.2;
    /// <summary>�\�́@�G���@�X�L���g�p�������x�A�b�v</summary>
    public const double SKILL_ERAPS_SP_UPDATE_RATE = 0.1;
    /// <summary>�\�́@�G���@�^�[���J�n�������V�[���h</summary>
    public const double SKILL_ERAPS_ADD_SP_RATE = 0.5;

    /// <summary>�\�́@�G�O�U�@���E�ɗ^����_���[�W</summary>
    public const double SKILL_EXA_SIDE_ATTACK_RATE = 0.8;

    /// <summary>�\�́@�E�[���@�����˒�</summary>
    public const int SKILL_WORRA_INIT_RANGE = 1;
    /// <summary>�\�́@�E�[���@�ő�˒�</summary>
    public const int SKILL_WORRA_MAX_RANGE = 4;
    /// <summary>�\�́@�E�[���@�����U����</summary>
    public const int SKILL_WORRA_INIT_ATTACK_COUNT = 2;

    /// <summary>�\�́@�N�[�@�˒�</summary>
    public const int SKILL_KOOB_ATTACK_RANGE = 2;
    /// <summary>�\�́@�N�[�@������</summary>
    public const double SKILL_KOOB_POWUP_RATE = 1.5;
    /// <summary>�\�́@�N�[�@���O�i���N�U����</summary>
    public const double SKILL_KOOB_RAGNAROK_RATE = 3.0;
    /// <summary>�\�́@�N�[�@���O�i���N�͈�</summary>
    public const int SKILL_KOOB_RAGNAROK_RANGE = 3;
    /// <summary>�\�́@�N�[�@���O�i���N�����s�^�[��</summary>
    public const int SKILL_KOOB_RAGNAROK_PARALYZE_TURN = 2;
    /// <summary>�\�́@�N�[�@���O�i���N�����s�^�[��</summary>
    public const int SKILL_KOOB_RAGNAROK_POW_INVALID_TURN = 2;

    /// <summary>�\�́@�I�@�U���͔{��</summary>
    public const double SKILL_YOU_ATTACK_RATE = 1.5;
    /// <summary>�\�́@�I�@��_���[�W�{��</summary>
    public const double SKILL_YOU_DAMAGE_RATE = 0.6;
    /// <summary>�\�́@�I�@���������˒�</summary>
    public const int SKILL_YOU_KILL_RANGE = 3;


    /// <summary>�J�����ړ�����</summary>
    public const float CAMERA_MOVE_TIME = 0.3f;

    /// <summary>�ŏI�X�e�[�W�ԍ�</summary>
    public const int LAST_STAGE = 6;

    /// <summary>�w�i�F�h���V�[</summary>
    public static readonly Color COLOR_BG_DROWS = new Color(0.4f, 0.6f, 0.3f);
    /// <summary>�w�i�F�G��</summary>
    public static readonly Color COLOR_BG_ERAPS = new Color(0.6f, 0.6f, 0.6f);
    /// <summary>�w�i�F�G�O�U</summary>
    public static readonly Color COLOR_BG_EXA = new Color(0.5f, 0.5f, 0.1f);
    /// <summary>�w�i�F�E�[��</summary>
    public static readonly Color COLOR_BG_WORRA = new Color(0.1f, 0.5f, 0.1f);
    /// <summary>�w�i�F�N�[</summary>
    public static readonly Color COLOR_BG_KOOB = new Color(0.2f, 0.2f, 0.5f);
    /// <summary>�w�i�F�I</summary>
    public static readonly Color COLOR_BG_YOU = new Color(0.2f, 0.2f, 0.2f);
}
