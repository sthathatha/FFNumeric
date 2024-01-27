using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerScript : CharacterScript
{
    public PlayerScript()
    {
        switch (Global.GetTemporaryData().selectPlayer)
        {
            case Constant.PlayerID.Koob:
                attackRange = Constant.SKILL_KOOB_ATTACK_RANGE; break;
            case Constant.PlayerID.Eraps:
                spUpRate = Constant.SKILL_ERAPS_INIT_SP_RATE; break;
            case Constant.PlayerID.Worra:
                attackCount = Constant.SKILL_WORRA_INIT_ATTACK_COUNT;
                attackRange = Constant.SKILL_WORRA_INIT_RANGE;
                break;
            default:
                attackRange = 1;
                attackCount = 1;
                spUpRate = 0;
                break;
        }
    }

    /// <summary>
    /// �U����
    /// </summary>
    /// <returns></returns>
    public override double GetAttackNum()
    {
        var dmg = base.GetAttackNum();

        switch (Global.GetTemporaryData().selectPlayer)
        {
            case Constant.PlayerID.Drows:
                if (IsSkillValid())
                {
                    dmg *= Constant.SKILL_DROWS_ATTACK_RATE;
                }
                break;
            case Constant.PlayerID.You:
                // �I�@�U����UP
                dmg *= Constant.SKILL_YOU_ATTACK_RATE; break;
        }

        return dmg;
    }

    /// <summary>
    /// �_���[�W
    /// </summary>
    /// <param name="_dmg"></param>
    public override void Damage(double _dmg)
    {
        switch (Global.GetTemporaryData().selectPlayer)
        {
            case Constant.PlayerID.You:
                // �I�@�_���[�WDOWN
                _dmg *= Constant.SKILL_YOU_DAMAGE_RATE; break;
        }

        base.Damage(_dmg);
    }

    /// <summary>
    /// HP����
    /// </summary>
    /// <param name="_add"></param>
    public override void AddHp(double _add)
    {
        switch (Global.GetTemporaryData().selectPlayer)
        {
            case Constant.PlayerID.Koob:
                // �N�[�@������UP
                _add *= Constant.SKILL_KOOB_POWUP_RATE; break;
        }

        base.AddHp(_add);

        if (Global.GetTemporaryData().selectPlayer == Constant.PlayerID.Eraps)
        {
            base.AddSp(_add);
        }
    }

    /// <summary>
    /// �^�[���J�n
    /// </summary>
    public override void TurnStart()
    {
        base.TurnStart();

        switch (Global.GetTemporaryData().selectPlayer)
        {
            case Constant.PlayerID.Eraps:
                // �G�� �J�n���V�[���h����
                AddSp(GetHp() * Constant.SKILL_ERAPS_ADD_SP_RATE);
                break;
        }
    }
}
