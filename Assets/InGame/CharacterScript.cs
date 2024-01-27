using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterScript : MonoBehaviour
{
    /// <summary>体力</summary>
    protected double hpNum;
    /// <summary>シールド</summary>
    protected double spNum;

    /// <summary>最大体力</summary>
    protected double maxHpNum;
    /// <summary>SP成長割合</summary>
    protected double spUpRate;

    /// <summary>HP表示Text</summary>
    protected TMP_Text hpText;
    /// <summary>SP表示Text</summary>
    protected TMP_Text spText;

    /// <summary>Animator Controller</summary>
    protected Animator modelAnim;

    /// <summary>射程距離</summary>
    protected int attackRange;

    /// <summary>攻撃回数</summary>
    protected int attackCount;

    /// <summary>成長不可ターン</summary>
    protected int powUpInvalidTurn = 0;

    /// <summary>反撃不可ターン</summary>
    protected int attackInvalidCount = 0;

    /// <summary>スキル持続ターン</summary>
    protected int skillKeepTurn;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public CharacterScript()
    {
        hpNum = 0;
        spNum = 0;
        maxHpNum = 0;
        spUpRate = 0;
        attackRange = 1;
        attackCount = 1;
        skillKeepTurn = 0;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        updateHp();
        updateSp();
    }

    /// <summary>
    /// メンバー変数取得
    /// </summary>
    public void InitMember()
    {
        var canvas = transform.Find("Canvas");

        hpText = canvas.Find("hpText").GetComponent<TMP_Text>();
        spText = canvas.Find("spText").GetComponent<TMP_Text>();
        modelAnim = transform.Find("model").GetComponent<Animator>();
    }

    public double GetHp() { return hpNum; }
    public bool IsAlive() { return hpNum >= 0.5; }
    public double GetMaxHp() { return maxHpNum; }
    public double GetSp() { return spNum; }

    /// <summary>攻撃範囲</summary>
    /// <returns></returns>
    public int GetAttackRange() { return attackRange; }
    /// <summary>攻撃範囲増加</summary>
    public void IncreaseAttackRange() { attackRange++; }

    public void AddAttackInvalidCount(int _cnt) { attackInvalidCount += _cnt; }

    public void AddPowUpInvalidTurn(int _turn) { powUpInvalidTurn += _turn; }

    /// <summary>攻撃回数</summary>
    /// <returns></returns>
    public int GetAttackCount() { return attackCount; }
    /// <summary>攻撃回数増加</summary>
    public void IncreaseAttackCount() { attackCount++; }

    /// <summary>スキル有効</summary>
    /// <param name="_keepTurn"></param>
    public void SetSkillValid(int _keepTurn) { skillKeepTurn = _keepTurn; }

    /// <summary>スキル有効中</summary>
    /// <returns></returns>
    public bool IsSkillValid() { return skillKeepTurn > 0; }

    /// <summary>
    /// HPを最大値にクランプ
    /// </summary>
    protected void ClampHp()
    {
        if (hpNum > Constant.HP_LIMIT) { hpNum = Constant.HP_LIMIT; }
        if (maxHpNum > Constant.HP_LIMIT) { maxHpNum = Constant.HP_LIMIT; }
    }

    /// <summary>
    /// 親スクリプト取得
    /// </summary>
    /// <returns></returns>
    public FieldCellScript GetCellScript()
    {
        return gameObject.transform.parent.GetComponent<FieldCellScript>();
    }

    /// <summary>
    /// HPを成長
    /// </summary>
    /// <param name="_add"></param>
    virtual public void AddHp(double _add)
    {
        if (powUpInvalidTurn > 0)
        {
            // 成長不可
            powUpInvalidTurn--;
            return;
        }

        maxHpNum += _add;
        hpNum += _add;

        ClampHp();

        updateHp();
    }

    /// <summary>
    /// SPを成長
    /// </summary>
    /// <param name="_add"></param>
    public void AddSp(double _add)
    {
        var num = spNum + _add * spUpRate;
        if (num > Constant.SHIELD_LIMIT) { num = Constant.SHIELD_LIMIT; }

        spNum = num;
        updateSp();
    }

    /// <summary>
    /// SP成長率アップ
    /// </summary>
    /// <param name="_increase"></param>
    public void IncreaseSpRate(double _increase)
    {
        spUpRate += _increase;

        // 無いと思うけど万が一増えすぎるとSPオーバーフローの可能性があるためe+300→e+305の幅以内ぐらい適当
        if (spUpRate > 10000.0) { spUpRate = 10000.0; }
    }

    /// <summary>
    /// HP初期化
    /// </summary>
    /// <param name="_hp"></param>
    public void InitHp(double _hp)
    {
        maxHpNum = _hp;
        hpNum = _hp;
        updateHp();
    }

    /// <summary>
    /// キャラクターSpriteに色設定
    /// </summary>
    /// <param name="_col"></param>
    public void SetCharacterColor(Color _col)
    {
        transform.Find("model").GetComponent<SpriteRenderer>().color = _col;
    }

    /// <summary>
    /// HP表示更新
    /// </summary>
    public void updateHp()
    {
        hpNum = System.Math.Floor(hpNum);

        if (hpText) { hpText.SetText(Util.ToHpSpViewString(hpNum)); }
    }

    /// <summary>
    /// HP色変更
    /// </summary>
    /// <param name="_col"></param>
    public void SetHpColor(Color _col)
    {
        if (hpText)
        {
            hpText.color = _col;
        }
    }

    /// <summary>
    /// SP表示更新
    /// </summary>
    public void updateSp()
    {
        spNum = System.Math.Floor(spNum);

        if (spText) { spText.SetText(spNum <= 0 ? "" : Util.ToHpSpViewString(spNum)); }
    }

    /// <summary>
    /// 攻撃の値取得
    /// </summary>
    /// <returns></returns>
    virtual public double GetAttackNum()
    {
        if (attackInvalidCount > 0)
        {
            // 反撃不可
            attackInvalidCount--;
            return 0;
        }

        return hpNum;
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="_dmg"></param>
    virtual public void Damage(double _dmg)
    {
        // シールド優先
        if (spNum > 0)
        {
            if (spNum < _dmg)
            {
                _dmg -= spNum;
                spNum = 0;
            }
            else
            {
                spNum -= _dmg;
                _dmg = 0;
            }

            updateSp();

            // シールドが0になる回はオーバー分が無効  は強すぎるのでやめ
            //return;
        }

        if (hpNum < _dmg)
        {
            hpNum = 0;
        }
        else
        {
            hpNum -= _dmg;
        }
        updateHp();
    }

    /// <summary>
    /// 攻撃アニメーション再生
    /// </summary>
    public void AnimateAttack()
    {
        modelAnim.SetTrigger("attackTrigger");
    }

    /// <summary>
    /// 死亡アニメーション再生
    /// </summary>
    public void AnimateDeathAndDestroy()
    {
        modelAnim.SetTrigger("deathTrigger");
        Destroy(gameObject, 0.4f);
    }

    /// <summary>
    /// 右に向ける
    /// </summary>
    /// <param name="_right"></param>
    public void SetCharacterRight(bool _right)
    {
        gameObject.transform.Find("model").GetComponent<SpriteRenderer>().flipX = _right;
    }

    /// <summary>
    /// ターン開始
    /// </summary>
    virtual public void TurnStart()
    {

    }

    /// <summary>
    /// ターン終了
    /// </summary>
    virtual public void TurnEnd()
    {
        if (IsSkillValid())
        {
            skillKeepTurn--;
        }
    }
}
