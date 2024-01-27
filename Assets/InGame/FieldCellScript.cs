using UnityEngine;

public class FieldCellScript : MonoBehaviour
{
    /// <summary>ゲーム内座標</summary>
    private Vector2Int location;

    /// <summary>キャラクター状態</summary>
    private Constant.CharacterType characterType;

    /// <summary>マス表示オブジェクト</summary>
    private GameObject fieldCell = null;
    /// <summary>キャラクター</summary>
    private GameObject characterObj = null;

    /// <summary>空の状態で残る時間</summary>
    private int emptyTime = 0;

    /// <summary>
    /// 位置設定
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetLocation(int x, int y)
    {
        location = new Vector2Int(x, y);
        this.gameObject.transform.position = Util.GetBasePosition(x, y);
    }

    /// <summary>位置取得</summary>
    /// <returns></returns>
    public Vector2Int GetLocation()
    {
        return location;
    }

    /// <summary>
    /// カーソルオブジェクト作成
    /// </summary>
    /// <param name="resource"></param>
    public void CreateCursor(GameObject resource)
    {
        fieldCell = Instantiate(resource);
        fieldCell.transform.SetParent(this.gameObject.transform);
        fieldCell.transform.localPosition = new Vector3(0, 0, 0);
        fieldCell.GetComponent<SpriteRenderer>().color = Color.black;
    }

    /// <summary>
    /// カーソル色セット
    /// </summary>
    /// <param name="col"></param>
    public void SetCellColor(Color col)
    {
        fieldCell.GetComponent<SpriteRenderer>().color = col;
    }

    /// <summary>
    /// キャラクター作成
    /// </summary>
    /// <param name="type"></param>
    /// <param name="prefabResource"></param>
    /// <param name="animResource"></param>
    public void CreateCharacter(Constant.CharacterType type, GameObject prefabResource, RuntimeAnimatorController animResource = null)
    {
        characterType = type;

        characterObj = Instantiate(prefabResource);
        characterObj.transform.SetParent(this.gameObject.transform);
        characterObj.transform.localPosition = Constant.CHARACTER_CELL_OFFSET;

        // アニメーションある場合設定
        if (animResource)
        {
            var anim = Instantiate(animResource);

            characterObj.transform.Find("model").GetComponent<Animator>().runtimeAnimatorController = anim;
        }

        characterObj.GetComponent<CharacterScript>().InitMember();
    }

    /// <summary>
    /// キャラクタースクリプト取得
    /// </summary>
    /// <returns></returns>
    public CharacterScript GetCharacterScript()
    {
        return characterObj?.GetComponent<CharacterScript>();
    }

    /// <summary>
    /// キャラクターGameObject取得
    /// </summary>
    /// <returns></returns>
    public GameObject GetCharacterObject()
    {
        return characterObj;
    }

    /// <summary>
    /// キャラクター状態取得
    /// </summary>
    /// <returns></returns>
    public Constant.CharacterType GetCharacterType()
    {
        return characterType;
    }

    /// <summary>
    /// キャラクターGameObjectを削除
    /// </summary>
    public void RemoveCharacterObj()
    {
        if (characterObj != null)
        {
            characterObj.transform.SetParent(null, false);
            characterObj = null;

            characterType = Constant.CharacterType.Empty;
            emptyTime = Constant.ENDLESS_ENEMY_POP_TURN;
        }
    }

    /// <summary>
    /// キャラクターGameObjectを設定
    /// </summary>
    /// <param name="_obj"></param>
    /// <param name="_characterType"></param>
    public void SetCharacterObj(GameObject _obj, Constant.CharacterType _characterType)
    {
        characterObj = _obj;
        characterObj.transform.SetParent(gameObject.transform, false);

        characterType = _characterType;
        if (_characterType != Constant.CharacterType.Empty)
        {
            // 死亡時に入れられるので、セット時には0としないとTurnEndが不具合を起こす
            emptyTime = 0;
        }
    }

    /// <summary>
    /// キャラクター死亡
    /// </summary>
    public void DeathCharacterObj()
    {
        if (characterObj != null)
        {
            characterObj.transform.SetParent(null, true);
            GetCharacterScript().AnimateDeathAndDestroy();
            characterObj = null;

            characterType = Constant.CharacterType.Empty;
            emptyTime = Constant.ENDLESS_ENEMY_POP_TURN;
        }
    }

    /// <summary>
    /// キャラクターHP初期化
    /// </summary>
    /// <param name="hp"></param>
    public void InitCharacterHp(double hp)
    {
        if (!characterObj) { return; }

        var chrScr = characterObj.GetComponent<CharacterScript>();
        chrScr.InitHp(hp);

        if (characterType == Constant.CharacterType.Enemy)
        {
            float red = (float)(System.Math.Log10(hp) / 300);
            int rand1;
            float green;
            float blue;
            if (red > 0.5)
            {
                // 充分強い場合は赤になる場合がある
                rand1 = Random.Range(0, 3);
            }
            else
            {
                rand1 = Random.Range(0, 2);
            }

            switch (rand1)
            {
                case 0:
                    green = 1.0f;
                    blue = Random.Range(0f, 1f);
                    break;
                case 1:
                    green = Random.Range(0f, 1f);
                    blue = 1.0f;
                    break;
                default:
                    green = Random.Range(0f, 1f) * (1 - red / 2);
                    blue = Random.Range(0f, 1f) * (1 - red / 2);
                    break;
            }

            chrScr.SetCharacterColor(new Color(red, green, blue));
        }
        else if (characterType == Constant.CharacterType.Boss)
        {
            if (Global.GetTemporaryData().selectStage != Constant.LAST_STAGE)
            {
                chrScr.SetCharacterColor(new Color(0.2f, 0.2f, 0.2f));
            }
        }
    }

    /// <summary>
    /// ターン開始
    /// </summary>
    public void TurnStart()
    {
        var scr = GetCharacterScript();
        if (scr)
        {
            scr.TurnStart();
        }
    }

    /// <summary>
    /// ターン終了
    /// </summary>
    public void TurnEnd()
    {
        if (emptyTime > 0)
        {
            emptyTime--;
        }

        var scr = GetCharacterScript();
        if (scr)
        {
            scr.TurnEnd();
        }

        if (characterType == Constant.CharacterType.Enemy)
        {
            // ターン終了時ザコ敵成長
            var addHp = scr.GetHp() * Constant.ENEMY_TURN_POWUP_RATE;
            scr.AddHp(addHp);


        }
    }

    /// <summary>
    /// 空状態の維持中
    /// </summary>
    /// <returns></returns>
    public bool IsEmptyTime()
    {
        return emptyTime > 0;
    }

    /// <summary>
    /// プレイヤーHPと比べて敵HPの色を変更
    /// </summary>
    /// <param name="_playerScript"></param>
    public void UpdateHPColor(CharacterScript _playerScript)
    {
        if (characterType != Constant.CharacterType.Enemy && characterType != Constant.CharacterType.Boss)
        {
            return;
        }

        // 色表示の倍率
        var weakRate = 0.6;
        var aRate1 = 1.0;
        var aRate2 = 1.5;
        var bRate1 = 1.9;
        var bRate2 = 2.1;
        var cRate1 = 3.5;
        var cRate2 = 4.4;
        var strongRate = 8.0;

        // 攻撃回数分を考慮
        if (_playerScript.GetAttackCount() > 1)
        {
            var rateP = _playerScript.GetAttackCount() - 1.0;
            weakRate += rateP;
            aRate1 += rateP;
            aRate2 += rateP;
            bRate1 += rateP;
            bRate2 += rateP;
            cRate1 += rateP;
            cRate2 += rateP;
            strongRate += rateP;
        }

        var scr = GetCharacterScript();
        var hp = scr.GetHp();
        var pHp = _playerScript.GetHp();
        var pSp = _playerScript.GetSp();
        // シールド減衰分を考慮
        if (pSp > 0)
        {
            var guardCount = pSp / hp;
            hp -= pHp * guardCount;
        }

        var rate = hp / pHp;

        var weakCol = new Color(0.5f, 1f, 0.5f);
        var aCol = Color.white;
        var bCol = new Color(1f, 0.2f, 1f);
        var cCol = new Color(1f, 0.2f, 0.2f);
        var strongCol = new Color(0.4f, 0.4f, 0.4f);
        Color col;
        if (rate <= weakRate)
        {
            col = weakCol;
        }
        else if (rate < aRate1)
        {
            col = calcBetweenColor((float)((rate - weakRate) / (aRate1 - weakRate)), weakCol, aCol);
        }
        else if (rate <= aRate2)
        {
            col = aCol;
        }
        else if (rate < bRate1)
        {
            col = calcBetweenColor((float)((rate - aRate2) / (bRate1 - aRate2)), aCol, bCol);
        }
        else if (rate <= bRate2)
        {
            col = bCol;
        }
        else if (rate < cRate1)
        {
            col = calcBetweenColor((float)((rate - bRate2) / (cRate1 - bRate2)), bCol, cCol);
        }
        else if (rate <= cRate2)
        {
            col = cCol;
        }
        else if (rate < strongRate)
        {
            col = calcBetweenColor((float)((rate - cRate2) / (strongRate - cRate2)), cCol, strongCol);
        }
        else
        {
            col = strongCol;
        }

        scr.SetHpColor(col);
    }

    /// <summary>
    /// 色補間
    /// </summary>
    /// <param name="rate">0～1</param>
    /// <param name="col1"></param>
    /// <param name="col2"></param>
    /// <returns></returns>
    private Color calcBetweenColor(float _rate, Color _col1, Color _col2)
    {
        var r = Util.CalcBetweenFloat(_rate, _col1.r, _col2.r);
        var g = Util.CalcBetweenFloat(_rate, _col1.g, _col2.g);
        var b = Util.CalcBetweenFloat(_rate, _col1.b, _col2.b);

        return new Color(r, g, b);
    }
}
