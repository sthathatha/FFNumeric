using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterScript : MonoBehaviour
{
    /// <summary>�̗�</summary>
    protected double hpNum;
    /// <summary>�V�[���h</summary>
    protected double spNum;

    /// <summary>�ő�̗�</summary>
    protected double maxHpNum;
    /// <summary>SP��������</summary>
    protected double spUpRate;

    /// <summary>HP�\��Text</summary>
    protected TMP_Text hpText;
    /// <summary>SP�\��Text</summary>
    protected TMP_Text spText;

    /// <summary>Animator Controller</summary>
    protected Animator modelAnim;

    /// <summary>�˒�����</summary>
    protected int attackRange;

    /// <summary>�U����</summary>
    protected int attackCount;

    /// <summary>�����s�^�[��</summary>
    protected int powUpInvalidTurn = 0;

    /// <summary>�����s�^�[��</summary>
    protected int attackInvalidCount = 0;

    /// <summary>�X�L�������^�[��</summary>
    protected int skillKeepTurn;

    /// <summary>
    /// �R���X�g���N�^
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
    /// �����o�[�ϐ��擾
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

    /// <summary>�U���͈�</summary>
    /// <returns></returns>
    public int GetAttackRange() { return attackRange; }
    /// <summary>�U���͈͑���</summary>
    public void IncreaseAttackRange() { attackRange++; }

    public void AddAttackInvalidCount(int _cnt) { attackInvalidCount += _cnt; }

    public void AddPowUpInvalidTurn(int _turn) { powUpInvalidTurn += _turn; }

    /// <summary>�U����</summary>
    /// <returns></returns>
    public int GetAttackCount() { return attackCount; }
    /// <summary>�U���񐔑���</summary>
    public void IncreaseAttackCount() { attackCount++; }

    /// <summary>�X�L���L��</summary>
    /// <param name="_keepTurn"></param>
    public void SetSkillValid(int _keepTurn) { skillKeepTurn = _keepTurn; }

    /// <summary>�X�L���L����</summary>
    /// <returns></returns>
    public bool IsSkillValid() { return skillKeepTurn > 0; }

    /// <summary>
    /// HP���ő�l�ɃN�����v
    /// </summary>
    protected void ClampHp()
    {
        if (hpNum > Constant.HP_LIMIT) { hpNum = Constant.HP_LIMIT; }
        if (maxHpNum > Constant.HP_LIMIT) { maxHpNum = Constant.HP_LIMIT; }
    }

    /// <summary>
    /// �e�X�N���v�g�擾
    /// </summary>
    /// <returns></returns>
    public FieldCellScript GetCellScript()
    {
        return gameObject.transform.parent.GetComponent<FieldCellScript>();
    }

    /// <summary>
    /// HP�𐬒�
    /// </summary>
    /// <param name="_add"></param>
    virtual public void AddHp(double _add)
    {
        if (powUpInvalidTurn > 0)
        {
            // �����s��
            powUpInvalidTurn--;
            return;
        }

        maxHpNum += _add;
        hpNum += _add;

        ClampHp();

        updateHp();
    }

    /// <summary>
    /// SP�𐬒�
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
    /// SP�������A�b�v
    /// </summary>
    /// <param name="_increase"></param>
    public void IncreaseSpRate(double _increase)
    {
        spUpRate += _increase;

        // �����Ǝv�����ǖ����ꑝ���������SP�I�[�o�[�t���[�̉\�������邽��e+300��e+305�̕��ȓ����炢�K��
        if (spUpRate > 10000.0) { spUpRate = 10000.0; }
    }

    /// <summary>
    /// HP������
    /// </summary>
    /// <param name="_hp"></param>
    public void InitHp(double _hp)
    {
        maxHpNum = _hp;
        hpNum = _hp;
        updateHp();
    }

    /// <summary>
    /// �L�����N�^�[Sprite�ɐF�ݒ�
    /// </summary>
    /// <param name="_col"></param>
    public void SetCharacterColor(Color _col)
    {
        transform.Find("model").GetComponent<SpriteRenderer>().color = _col;
    }

    /// <summary>
    /// HP�\���X�V
    /// </summary>
    public void updateHp()
    {
        hpNum = System.Math.Floor(hpNum);

        if (hpText) { hpText.SetText(Util.ToHpSpViewString(hpNum)); }
    }

    /// <summary>
    /// HP�F�ύX
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
    /// SP�\���X�V
    /// </summary>
    public void updateSp()
    {
        spNum = System.Math.Floor(spNum);

        if (spText) { spText.SetText(spNum <= 0 ? "" : Util.ToHpSpViewString(spNum)); }
    }

    /// <summary>
    /// �U���̒l�擾
    /// </summary>
    /// <returns></returns>
    virtual public double GetAttackNum()
    {
        if (attackInvalidCount > 0)
        {
            // �����s��
            attackInvalidCount--;
            return 0;
        }

        return hpNum;
    }

    /// <summary>
    /// �_���[�W���󂯂�
    /// </summary>
    /// <param name="_dmg"></param>
    virtual public void Damage(double _dmg)
    {
        // �V�[���h�D��
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

            // �V�[���h��0�ɂȂ��̓I�[�o�[��������  �͋�������̂ł��
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
    /// �U���A�j���[�V�����Đ�
    /// </summary>
    public void AnimateAttack()
    {
        modelAnim.SetTrigger("attackTrigger");
    }

    /// <summary>
    /// ���S�A�j���[�V�����Đ�
    /// </summary>
    public void AnimateDeathAndDestroy()
    {
        modelAnim.SetTrigger("deathTrigger");
        Destroy(gameObject, 0.4f);
    }

    /// <summary>
    /// �E�Ɍ�����
    /// </summary>
    /// <param name="_right"></param>
    public void SetCharacterRight(bool _right)
    {
        gameObject.transform.Find("model").GetComponent<SpriteRenderer>().flipX = _right;
    }

    /// <summary>
    /// �^�[���J�n
    /// </summary>
    virtual public void TurnStart()
    {

    }

    /// <summary>
    /// �^�[���I��
    /// </summary>
    virtual public void TurnEnd()
    {
        if (IsSkillValid())
        {
            skillKeepTurn--;
        }
    }
}
