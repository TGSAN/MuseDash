using FormulaBase;
using GameLogic;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LongPressController : BaseEnemyObjectController
{
    public static int psidx = -1;

    public void SetLength(float length)
    {
    }

    public override void Init()
    {
        base.Init();
        if (psidx < 0)
        {
            psidx = this.idx;
        }
    }

    public override void OnControllerStart()
    {
        GameKernel.Instance.IsUnderLongPress = true;
        base.OnControllerStart();
        ArrayList mds = StageBattleComponent.Instance.GetMusicData();
        if (mds == null)
        {
            return;
        }
        SpineActionController.SetSkeletonOrder(mds.Count - this.idx, this.gameObject);
    }

    public override bool OnControllerMiss(int idx)
    {
        var md = StageBattleComponent.Instance.GetMusicDataByIdx(idx);

        GameKernel.Instance.IsUnderLongPress = true;
#if !UNITY_EDITOR && !UNITY_EDITOR_OSX && !UNITY_EDITOR_64
		if (GameGlobal.gGameTouchPlay.IsPunch (Input.touchCount)) {
#else
        if (GameGlobal.gGameTouchPlay.IsPunch())
        {
#endif
            var result = StageBattleComponent.Instance.GetCurLPSPlayResult();
            BattleEnemyManager.Instance.AddHp(this.idx, -1);
            GameGlobal.gGameTouchPlay.TouchResult(idx, result, GameMusic.TOUCH_ACTION_SIGNLE_PRESS); //(mark) attack node damage to it
            BattleEnemyManager.Instance.SetPlayResult(idx, result);
            GameGlobal.gGameMissPlay.SetMissHardTime(0);
            AttacksController.Instance.ShowPressGirl(true);
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            if (!GameKernel.Instance.IsOnFeverState())
            {
                GirlManager.Instance.UnLockActionProtect();
                base.OnControllerMiss(idx);
                GameGlobal.gGameMissPlay.SetMissHardTime(0);
                GameKernel.Instance.IsLongPressFailed = true;
                GirlManager.Instance.BeAttackEffect();
                AttacksController.Instance.BeAttacked();
            }
            else
            {
                GirlManager.Instance.UnLockActionProtect();
                GameObject.Destroy(this.gameObject);
                foreach (var girl in GirlManager.Instance.Girls)
                {
                    if (girl != null)
                    {
                        if (SpineActionController.CurrentAnimationName(girl) != "run")
                        {
                            SpineActionController.Play(ACTION_KEYS.RUN, girl);
                        }
                    }
                }
            }
        }
        if (md.isLongPressEnd)
        {
            GameKernel.Instance.IsUnderLongPress = false;

            if (GameKernel.Instance.IsLongPressFailed)
            {
                StageTeachComponent.Instance.SetPlayResult(idx, GameMusic.MISS);
            }
            else
            {
                var result = StageBattleComponent.Instance.GetCurLPSPlayResult();
                StageTeachComponent.Instance.SetPlayResult(idx, result);
                TaskStageTarget.Instance.AddLongPressFinishCount(1);
                StageTeachComponent.Instance.AddPlayCountRecord(1);
            }

            GameKernel.Instance.IsLongPressFailed = false;

            AttacksController.Instance.ShowPressGirl(false);
            GirlManager.Instance.UnLockActionProtect();
            GirlManager.Instance.AttacksWithoutExchange(GameMusic.NONE, ACTION_KEYS.RUN);

            return false;
        }
        return true;
    }
}