using UnityEngine;
using System.Collections;
using GameLogic;
using FormulaBase;
using System.Collections.Generic;

public class NormalEnemyController : BaseEnemyObjectController
{
    public override void OnAttackDestory()
    {
        if (!this.IsEmptyNode())
        {
            return;
        }

        GameObject.Destroy(this.gameObject);
    }

    public override void OnControllerStart()
    {
        base.OnControllerStart();
    }

    public override void OnControllerAttacked(int result, bool isDeaded)
    {
        base.OnControllerAttacked(result, isDeaded);
    }

    public override bool OnControllerMiss(int idx)
    {
        if (GameKernel.Instance.IsOnFeverState())
        {
            return false;
        }

        if (this.IsShieldDefence())
        {
            return false;
        }

        bool isShowMiss = base.OnControllerMiss(idx);
        if (isShowMiss)
        {
            GirlManager.Instance.BeAttackEffect();
        }

        return isShowMiss;
    }

    /// <summary>
    /// Determines whether this instance is shield defence.
    ///
    /// 护盾 一次消耗一个护盾抵消一次伤害
    /// </summary>
    /// <returns><c>true</c> if this instance is shield defence; otherwise, <c>false</c>.</returns>
    private bool IsShieldDefence()
    {
        FormulaHost battleRole = BattleRoleAttributeComponent.Instance.GetBattleRole();
        if (battleRole == null)
        {
            return false;
        }

        List<FormulaHost> skObjShield = (List<FormulaHost>)battleRole.GetDynamicObjByKey(SkillComponent.SIGN_KEY_SKILL_SHIELD);
        if (skObjShield == null || skObjShield.Count <= 0)
        {
            return false;
        }

        FormulaHost skObj = skObjShield[0];
        SkillComponent.Instance.RemoveSkill(skObj);
        skObjShield.RemoveAt(0);
        return true;
    }
}