using FormulaBase;
using GameLogic;
using System;
using System.Collections;
using Assets.Scripts.Tools.Managers;
using UnityEngine;

public class AirEnergyBottleController : BaseEnemyObjectController
{
    private bool isBeAttacked = false;
    private static GameObject recoveryEffect = null;

    private void Start()
    {
        if (recoveryEffect == null)
        {
            int recoverItemIdx = 4000;
            string path = "Prefabs/skill/Skill_hp";
            recoveryEffect = StageBattleComponent.Instance.AddObjWithControllerInit(ref path, recoverItemIdx);
            recoveryEffect.SetActive(false);
        }
    }

    public override void OnControllerStart()
    {
        base.OnControllerStart();

        //set bottle size flag here
        FormulaHost host = BattleEnemyManager.Instance.Enemies[this.idx];
        if (host != null)
        {
            host.SetDynamicData("BOTTLE_TYPE", 2);
        }
    }

    private void RecoveryEffects()
    {
        var uid = BattleEnemyManager.Instance.GetNodeUidByIdx(this.idx);
        var recoverValue = 0;
        var noteConfig = ConfigManager.instance["notedata"];
        for (int i = 0; i < noteConfig.Count; i++)
        {
            var note = noteConfig[i];
            if (note["uid"].ToString() == uid)
            {
                recoverValue = (int)note["value"];
                break;
            }
        }

        BattleRoleAttributeComponent.Instance.AddHp(recoverValue);

        BattleEnemyManager.Instance.SetPlayResult(this.idx, GameMusic.PERFECT);
        BattleRoleAttributeComponent.Instance.FireSkill(SkillComponent.ON_EAT_ITEM);

        recoveryEffect.SetActive(true);

        SpineActionController.Play(ACTION_KEYS.COMEIN, recoveryEffect);
        SpineActionController.Play(ACTION_KEYS.COMEOUT, this.gameObject);

        string audioName = BattleEnemyManager.Instance.GetNodeAudioByIdx(this.idx);
        AudioManager.Instance.PlayGirlHitByName(audioName);
    }

    //	private IEnumerator WaitForEatEnergy(float waitTime){
    //		yield return new WaitForSeconds(waitTime);
    //		RecoveryEffects();
    //	}

    public override void OnControllerAttacked(int result, bool isDeaded)
    {
        if (!isBeAttacked)
        {
            //Debug.Log (" air energy item be attacked! ");
            this.RecoveryEffects();
            SpineActionController.Play(ACTION_KEYS.COMEOUT3, this.gameObject);
            Animator ani = this.gameObject.GetComponent<Animator>();
            if (ani != null)
            {
                ani.speed = 0;
            }

            isBeAttacked = true;
            TaskStageTarget.Instance.AddEnergyItemCount(1);
        }

        //		ArrayList musicData = StageBattleComponent.Instance.GetMusicData ();
        //		MusicData md = (MusicData)musicData [this.idx];
        //		// if is air unit, only jump can pass;
        //		// if is not air unit, only jump not pass.
        //		if (md.nodeData.isAirunits) {
        //			if (!GameGlobal.gGameTouchPlay.IsJump ()) {
        //				return;
        //			}
        //
        //			BattleEnemyManager.Instance.SetPlayResult(this.idx, (uint)result);
        //			var time = GameGlobal.JUMP_WHOLE_TIME * md.GetAttackrangeRate();
        //			StartCoroutine(WaitForEatEnergy(time));
        //		}

        //		string audioName = StageBattleComponent.Instance.GetNodeAudioByIdx (this.idx);
        //		AudioManager.Instance.PlayGirlHitByName (audioName);

        //		GameGlobal.gGameMusicScene.ShowLongPressMask (false);

        return;
    }
}