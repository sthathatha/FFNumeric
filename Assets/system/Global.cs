using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global
{
    private static SaveData _saveData = null;
    /// <summary>
    /// セーブデータ
    /// </summary>
    /// <returns></returns>
    public static SaveData GetSaveData()
    {
        if (_saveData == null)
        {
            _saveData = new SaveData();
        }
        return _saveData;
    }

    private static TemporaryData _temporaryData = null;
    /// <summary>
    /// 一時データ
    /// </summary>
    /// <returns></returns>
    public static TemporaryData GetTemporaryData()
    {
        if (_temporaryData == null)
        {
            _temporaryData = new TemporaryData();
        }
        return _temporaryData;
    }

    /// <summary>
    /// セーブデータ
    /// </summary>
    public class SaveData
    {
        /// <summary>プロローグ見たフラグ</summary>
        public int stage0Clear;
        /// <summary>ステージ１クリア</summary>
        public int stage1Clear;
        /// <summary>ステージ２クリア</summary>
        public int stage2Clear;
        /// <summary>ステージ３クリア</summary>
        public int stage3Clear;
        /// <summary>ステージ４クリア</summary>
        public int stage4Clear;
        /// <summary>ステージ５クリア</summary>
        public int stage5Clear;
        /// <summary>ステージ６クリア</summary>
        public int stage6Clear;
        /// <summary>エンドレスモードクリア</summary>
        public int endlessClear;

        /// <summary>記録　ドロシー</summary>
        public EndlessRecord recordDrows;
        /// <summary>記録　エラ</summary>
        public EndlessRecord recordEraps;
        /// <summary>記録　エグザ</summary>
        public EndlessRecord recordExa;
        /// <summary>記録　ウーラ</summary>
        public EndlessRecord recordWorra;
        /// <summary>記録　クー</summary>
        public EndlessRecord recordKoob;
        /// <summary>記録　悠</summary>
        public EndlessRecord recordYou;

        /// <summary>オプション</summary>
        public OptionData option;

        /// <summary>
        /// 
        /// </summary>
        public struct EndlessRecord
        {
            /// <summary>normalターン数</summary>
            public int normalTurn;
            /// <summary>normal最大HP</summary>
            public double normalMaxHp;
            /// <summary>ハードターン数</summary>
            public int hardTurn;
            /// <summary>ハード最大HP</summary>
            public double hardMaxHp;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct OptionData
        {
            /// <summary>戦闘演出を飛ばす</summary>
            public int skipEffect;
            /// <summary>BGM</summary>
            public int bgmVolume;
            /// <summary>SE</summary>
            public int seVolume;
        }

        public SaveData()
        {
            stage0Clear = 0;
            stage1Clear = 0;
            stage2Clear = 0;
            stage3Clear = 0;
            stage4Clear = 0;
            stage5Clear = 0;
            stage6Clear = 0;
            endlessClear = 0;

            recordDrows.normalTurn = 0;
            recordEraps.normalTurn = 0;
            recordExa.normalTurn = 0;
            recordWorra.normalTurn = 0;
            recordKoob.normalTurn = 0;
            recordYou.normalTurn = 0;
            recordDrows.normalMaxHp = 0;
            recordEraps.normalMaxHp = 0;
            recordExa.normalMaxHp = 0;
            recordWorra.normalMaxHp = 0;
            recordKoob.normalMaxHp = 0;
            recordYou.normalMaxHp = 0;

            recordDrows.hardTurn = 0;
            recordEraps.hardTurn = 0;
            recordExa.hardTurn = 0;
            recordWorra.hardTurn = 0;
            recordKoob.hardTurn = 0;
            recordYou.hardTurn = 0;
            recordDrows.hardMaxHp = 0;
            recordEraps.hardMaxHp = 0;
            recordExa.hardMaxHp = 0;
            recordWorra.hardMaxHp = 0;
            recordKoob.hardMaxHp = 0;
            recordYou.hardMaxHp = 0;

            option.skipEffect = 0;
            option.bgmVolume = 32;
            option.seVolume = 32;
        }

        /// <summary>
        /// セーブ
        /// </summary>
        public void Save()
        {
            PlayerPrefs.SetInt("stage0Clear", stage0Clear);
            PlayerPrefs.SetInt("stage1Clear", stage1Clear);
            PlayerPrefs.SetInt("stage2Clear", stage2Clear);
            PlayerPrefs.SetInt("stage3Clear", stage3Clear);
            PlayerPrefs.SetInt("stage4Clear", stage4Clear);
            PlayerPrefs.SetInt("stage5Clear", stage5Clear);
            PlayerPrefs.SetInt("stage6Clear", stage6Clear);
            PlayerPrefs.SetInt("endlessClear", endlessClear);

            PlayerPrefs.SetInt("recordDrowsNormalTurn", recordDrows.normalTurn);
            PlayerPrefs.SetInt("recordErapsNormalTurn", recordEraps.normalTurn);
            PlayerPrefs.SetInt("recordExaNormalTurn", recordExa.normalTurn);
            PlayerPrefs.SetInt("recordWorraNormalTurn", recordWorra.normalTurn);
            PlayerPrefs.SetInt("recordKoobNormalTurn", recordKoob.normalTurn);
            PlayerPrefs.SetInt("recordYouNormalTurn", recordYou.normalTurn);
            PlayerPrefs.SetString("recordDrowsNormalMaxHp", recordDrows.normalMaxHp.ToString());
            PlayerPrefs.SetString("recordErapsNormalMaxHp", recordEraps.normalMaxHp.ToString());
            PlayerPrefs.SetString("recordExaNormalMaxHp", recordExa.normalMaxHp.ToString());
            PlayerPrefs.SetString("recordWorraNormalMaxHp", recordWorra.normalMaxHp.ToString());
            PlayerPrefs.SetString("recordKoobNormalMaxHp", recordKoob.normalMaxHp.ToString());
            PlayerPrefs.SetString("recordYouNormalMaxHp", recordYou.normalMaxHp.ToString());

            PlayerPrefs.SetInt("recordDrowsHardTurn", recordDrows.hardTurn);
            PlayerPrefs.SetInt("recordErapsHardTurn", recordEraps.hardTurn);
            PlayerPrefs.SetInt("recordExaHardTurn", recordExa.hardTurn);
            PlayerPrefs.SetInt("recordWorraHardTurn", recordWorra.hardTurn);
            PlayerPrefs.SetInt("recordKoobHardTurn", recordKoob.hardTurn);
            PlayerPrefs.SetInt("recordYouHardTurn", recordYou.hardTurn);
            PlayerPrefs.SetString("recordDrowsHardMaxHp", recordDrows.hardMaxHp.ToString());
            PlayerPrefs.SetString("recordErapsHardMaxHp", recordEraps.hardMaxHp.ToString());
            PlayerPrefs.SetString("recordExaHardMaxHp", recordExa.hardMaxHp.ToString());
            PlayerPrefs.SetString("recordWorraHardMaxHp", recordWorra.hardMaxHp.ToString());
            PlayerPrefs.SetString("recordKoobHardMaxHp", recordKoob.hardMaxHp.ToString());
            PlayerPrefs.SetString("recordYouHardMaxHp", recordYou.hardMaxHp.ToString());

            PlayerPrefs.SetInt("optionSkipEffect", option.skipEffect);
            PlayerPrefs.SetInt("optionBgmVolume", option.bgmVolume);
            PlayerPrefs.SetInt("optionSeVolume", option.seVolume);

            PlayerPrefs.Save();
        }

        /// <summary>
        /// ロード
        /// </summary>
        public void Load()
        {
            stage0Clear = PlayerPrefs.GetInt("stage0Clear", 0);
            stage1Clear = PlayerPrefs.GetInt("stage1Clear", 0);
            stage2Clear = PlayerPrefs.GetInt("stage2Clear", 0);
            stage3Clear = PlayerPrefs.GetInt("stage3Clear", 0);
            stage4Clear = PlayerPrefs.GetInt("stage4Clear", 0);
            stage5Clear = PlayerPrefs.GetInt("stage5Clear", 0);
            stage6Clear = PlayerPrefs.GetInt("stage6Clear", 0);
            endlessClear = PlayerPrefs.GetInt("endlessClear", 0);

            recordDrows.normalTurn = PlayerPrefs.GetInt("recordDrowsNormalTurn", 0);
            recordEraps.normalTurn = PlayerPrefs.GetInt("recordErapsNormalTurn", 0);
            recordExa.normalTurn = PlayerPrefs.GetInt("recordExaNormalTurn", 0);
            recordWorra.normalTurn = PlayerPrefs.GetInt("recordWorraNormalTurn", 0);
            recordKoob.normalTurn = PlayerPrefs.GetInt("recordKoobNormalTurn", 0);
            recordYou.normalTurn = PlayerPrefs.GetInt("recordYouNormalTurn", 0);
            recordDrows.normalMaxHp = double.Parse(PlayerPrefs.GetString("recordDrowsNormalMaxHp", "0"));
            recordEraps.normalMaxHp = double.Parse(PlayerPrefs.GetString("recordErapsNormalMaxHp", "0"));
            recordExa.normalMaxHp = double.Parse(PlayerPrefs.GetString("recordExaNormalMaxHp", "0"));
            recordWorra.normalMaxHp = double.Parse(PlayerPrefs.GetString("recordWorraNormalMaxHp", "0"));
            recordKoob.normalMaxHp = double.Parse(PlayerPrefs.GetString("recordKoobNormalMaxHp", "0"));
            recordYou.normalMaxHp = double.Parse(PlayerPrefs.GetString("recordYouNormalMaxHp", "0"));

            recordDrows.hardTurn = PlayerPrefs.GetInt("recordDrowsHardTurn", 0);
            recordEraps.hardTurn = PlayerPrefs.GetInt("recordErapsHardTurn", 0);
            recordExa.hardTurn = PlayerPrefs.GetInt("recordExaHardTurn", 0);
            recordWorra.hardTurn = PlayerPrefs.GetInt("recordWorraHardTurn", 0);
            recordKoob.hardTurn = PlayerPrefs.GetInt("recordKoobHardTurn", 0);
            recordYou.hardTurn = PlayerPrefs.GetInt("recordYouHardTurn", 0);
            recordDrows.hardMaxHp = double.Parse(PlayerPrefs.GetString("recordDrowsHardMaxHp", "0"));
            recordEraps.hardMaxHp = double.Parse(PlayerPrefs.GetString("recordErapsHardMaxHp", "0"));
            recordExa.hardMaxHp = double.Parse(PlayerPrefs.GetString("recordExaHardMaxHp", "0"));
            recordWorra.hardMaxHp = double.Parse(PlayerPrefs.GetString("recordWorraHardMaxHp", "0"));
            recordKoob.hardMaxHp = double.Parse(PlayerPrefs.GetString("recordKoobHardMaxHp", "0"));
            recordYou.hardMaxHp = double.Parse(PlayerPrefs.GetString("recordYouHardMaxHp", "0"));

            option.skipEffect = PlayerPrefs.GetInt("optionSkipEffect", 0);
            option.bgmVolume = PlayerPrefs.GetInt("optionBgmVolume", 32);
            option.seVolume = PlayerPrefs.GetInt("optionSeVolume", 32);
        }

        /// <summary>
        /// 戦闘スキップ
        /// </summary>
        /// <returns></returns>
        public bool IsBattleSkip()
        {
            return option.skipEffect == 1;
        }
    }

    /// <summary>
    /// 保存しないデータ
    /// </summary>
    public class TemporaryData
    {
        /// <summary>選択中プレイヤー</summary>
        public Constant.PlayerID selectPlayer;

        /// <summary>選択ステージ</summary>
        public int selectStage;

        /// <summary>難易度</summary>
        public Constant.Difficulty difficulty;

        public TemporaryData()
        {
            selectPlayer = Constant.PlayerID.Exa;
            selectStage = 1;
            difficulty = Constant.Difficulty.Normal;
        }
    }
}
