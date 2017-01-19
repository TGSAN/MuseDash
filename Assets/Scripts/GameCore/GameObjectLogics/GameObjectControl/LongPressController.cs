using FormulaBase;
using GameLogic;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LongPressController : BaseEnemyObjectController
{
    public static int psidx = -1;
    public bool isActive = true;
    public bool isEndTouch = false;

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
        if (!isActive) return false;
        var md = StageBattleComponent.Instance.GetMusicDataByIdx(idx);
        GameKernel.Instance.IsUnderLongPress = true;

        var isPunch = GameGlobal.gGameTouchPlay.IsPunch();
#if !UNITY_EDITOR && !UNITY_EDITOR_OSX && !UNITY_EDITOR_64
        isPunch = GameGlobal.gGameTouchPlay.IsPunch (Input.touchCount);
#endif
        if (!md.isLongPressEnd && !isEndTouch)
        {
            if (isPunch)
            {
                var result = StageBattleComponent.Instance.GetCurLPSPlayResult();
                BattleEnemyManager.Instance.AddHp(this.idx, -1);
                GameGlobal.gGameTouchPlay.TouchResult(idx, result, GameMusic.TOUCH_ACTION_SIGNLE_PRESS); //(mark) attack node damage to it
                BattleEnemyManager.Instance.SetPlayResult(idx, result);
                GameGlobal.gGameMissPlay.SetMissHardTime(0);
                BattleEnemyManager.Instance.SetLongPressEffect(true);
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
                    StageBattleComponent.Instance.curLPSIdx = -1;
                }
                else
                {
                    GirlManager.Instance.UnLockActionProtect();
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
        }
        else
        {
            if (isPunch || isEndTouch)
            {
                var result = StageBattleComponent.Instance.GetCurLPSPlayResult();
                GameGlobal.gGameTouchPlay.TouchResult(idx, result, GameMusic.TOUCH_ACTION_SIGNLE_PRESS);
                StageTeachComponent.Instance.SetPlayResult(idx, result);
                TaskStageTarget.Instance.AddLongPressFinishCount(1);
                StageTeachComponent.Instance.AddPlayCountRecord(1);

                GirlManager.Instance.UnLockActionProtect();

                var go = BattleEnemyManager.Instance.GetObj(StageBattleComponent.Instance.curLPSIdx);
                if (go)
                {
                    var sac = go.GetComponent<SpineActionController>();
                    if (sac)
                    {
                        sac.DestroyLongPress();
                    }
                }
                isActive = false;
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
                    StageBattleComponent.Instance.curLPSIdx = -1;
                }
                else
                {
                    GirlManager.Instance.UnLockActionProtect();
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

            BattleEnemyManager.Instance.SetLongPressEffect(false);
            GameKernel.Instance.IsLongPressFailed = false;
            GameKernel.Instance.IsUnderLongPress = false;
        }

        return true;
    }
}