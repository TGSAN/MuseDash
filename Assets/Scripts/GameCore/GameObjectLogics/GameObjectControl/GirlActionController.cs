using UnityEngine;
using System.Collections;
using GameLogic;
using FormulaBase;
using System.Collections.Generic;

public class GirlActionController : BaseSpineObjectController
{
    //private static int ATTACKER_INDEX = 0;
    //private static string[] ACTION_BY_INDEX = new string[]{ACTION_KEYS.MELEE1, ACTION_KEYS.RUN, ACTION_KEYS.RUN};
    //private int actionIndex = 0;

    // Use this for initialization
    //void Start () {
    //}

    public static GirlActionController instance
    {
        get; private set;
    }

    public string curAnimName
    {
        get
        {
            return gameObject.GetComponent<SpineActionController>().GetCurrentActionName();
        }
    }

    public override void OnControllerStart()
    {
        SpineActionController.SetSynchroObjectsActive(this.gameObject, true);
        SpineActionController.Play(ACTION_KEYS.COMEIN, this.gameObject);
        SpineMountController smc = this.gameObject.GetComponent<SpineMountController>();
        if (smc != null)
        {
            smc.OnControllerStart();
        }

        this.StartPhysicDetect();
        this.InitSkills();
    }

    public override bool ControllerMissCheck(int idx, decimal currentTick)
    {
        return true;
    }

    public override void OnControllerAttacked(int result, bool isDeaded)
    {
    }

    public override bool OnControllerMiss(int idx)
    {
        return true;
    }

    public override void Init()
    {
        instance = this;
        this.gameObject.SetActive(false);
        SpineActionController.SetSynchroObjectsActive(this.gameObject, false);
    }

    public override void SetIdx(int idx)
    {
        this.idx = idx;
        //this.actionIndex = idx;
    }

    /*
	public int GetActionIndex() {
		return this.actionIndex;
	}
*/

    // ----------------------------- Actions -----------------------------
    public void StartPhysicDetect()
    {
        var spineMountController = this.gameObject.GetComponent<SpineMountController>();
        if (spineMountController == null || spineMountController.GetMountObjects() == null)
        {
            return;
        }

        foreach (var detectScript in spineMountController.GetMountObjects())
        {
            if (detectScript == null)
            {
                continue;
            }

            var detect = detectScript.GetComponent<GirlCollisionDetectNodeController>();
            if (detect == null)
            {
                continue;
            }

            detect.EnableDetect();
        }
    }

    public void StopPhysicDetect()
    {
        var spineMountController = this.gameObject.GetComponent<SpineMountController>();
        if (spineMountController == null)
        {
            return;
        }

        foreach (var detectScript in spineMountController.GetMountObjects())
        {
            if (detectScript == null || spineMountController.GetMountObjects() == null)
            {
                continue;
            }

            var detect = detectScript.GetComponent<GirlCollisionDetectNodeController>();
            if (detect == null)
            {
                continue;
            }

            detect.DisableDetect();
        }
    }

    public void Attack(string actKey, uint result)
    {
        /*
		this.actionIndex -= 1;
		if (this.actionIndex < ATTACKER_INDEX) {
			this.actionIndex = ACTION_BY_INDEX.Length - 1;
		}
*/
        this.__Attack(actKey, result);
    }

    public void BackToNormalRun()
    {
        var curAnimationName = gameObject.GetComponent<SpineActionController>().GetCurrentAnimationName();
        if (curAnimationName == "press")
        {
            SpineActionController.Play("char_run", this.gameObject);
        }
    }

    public void AttackQuick(string actKey, uint result, float tick = 0)
    {
        //if (this.actionIndex != ATTACKER_INDEX) {
        //	return;
        //}

        string _atkName = null;
        if (result == GameMusic.COOL)
        {
            _atkName = (actKey != null && actKey.Length > 2) ? actKey : ACTION_KEYS.ATTACK_COOL;
        }
        else if (result == GameMusic.GREAT)
        {
            _atkName = (actKey != null && actKey.Length > 2) ? actKey : ACTION_KEYS.ATTACK_GREAT;
        }
        else
        {
            _atkName = (actKey != null && actKey.Length > 2) ? actKey : ACTION_KEYS.ATTACK_PERFECT;
        }
        _atkName = JumpAttack(_atkName, result);
        SpineActionController.Play(_atkName, this.gameObject, tick);
    }

    private string JumpAttack(string atkName, uint result)
    {
        if (!GirlManager.Instance.IsJumpingAction() || result == GameMusic.NONE || atkName == ACTION_KEYS.JUMP) return atkName;
        if (atkName == ACTION_KEYS.HURT || atkName == ACTION_KEYS.JUMP_HURT)
        {
            GirlManager.Instance.SetJumpingAction(false);
            return atkName;
        }
        var md = StageBattleComponent.Instance.neareastMusicData;
        var isAirNode = md.nodeData.type == GameGlobal.NODE_TYPE_AIR_BEAT;
        var isLongPress = md.isLongPressStart || md.isLongPress || md.isLongPressEnd;
        if (!isAirNode)
        {
            GirlManager.Instance.SetJumpingAction(false);
            atkName = ACTION_KEYS.JUMP_DOWN_ATTACK;
        }
        else
        {
            GirlManager.Instance.SetJumpingAction(true);
            atkName = ACTION_KEYS.JUMP_ATTACK;
        }

        if (isLongPress)
        {
            GirlManager.Instance.SetJumpingAction(false);
            atkName = ACTION_KEYS.JUMP_DOWN_PRESS;
        }
        return atkName;
    }

    /*
    public void ResetAttacker() {
        if (this.actionIndex != ATTACKER_INDEX) {
            return;
        }

        SpineActionController.Play (ACTION_KEYS.RUN, this.gameObject);
    }
    */

    private void InitSkills()
    {
        // Auto use skills.
        FormulaHost battleRole = BattleRoleAttributeComponent.Instance.GetBattleRole();
        if (battleRole == null)
        {
            return;
        }

        List<FormulaHost> skills = (List<FormulaHost>)battleRole.GetDynamicObjByKey(SkillComponent.SIGN_KEY_SKILLS);
        if (skills == null)
        {
            return;
        }

        for (int i = 0; i < skills.Count; i++)
        {
            FormulaHost skillObject = skills[i];
            if (skillObject == null)
            {
                continue;
            }

            this.UseSkill(skillObject);
        }
    }

    public void UseSkill(FormulaHost skillObject)
    {
        if (skillObject == null)
        {
            return;
        }

        SkillComponent.Instance.FireSkill(skillObject, SkillComponent.ON_START);
        float duration = skillObject.GetDynamicDataByKey(SkillComponent.SIGN_KEY_COUNTDOWN);
        if (duration > 0)
        {
            this.StartCoroutine(this.EndSkill(duration, skillObject));
        }
    }

    private IEnumerator EndSkill(float second, FormulaHost skillObject)
    {
        yield return new WaitForSeconds(second);

        if (skillObject != null)
        {
            SkillComponent.Instance.FireSkill(skillObject, SkillComponent.ON_TIME_UP);
        }
    }

    // ----------------------------- Callbacks -----------------------------
    private void __Attack(string actKey, uint result)
    {
        //string __actKey = ACTION_BY_INDEX [this.actionIndex];
        // When this girl is attacking.
        //if (this.actionIndex == ATTACKER_INDEX) {
        //	__actKey = (actKey != null && actKey.Length > 2) ? actKey : ACTION_KEYS.MELEE1;
        //}

        // string _actKey = (actKey != null && actKey.Length > 2) ? actKey : ACTION_KEYS.ATTACK_MISS;
        SpineActionController.Play(actKey, this.gameObject);
    }
}