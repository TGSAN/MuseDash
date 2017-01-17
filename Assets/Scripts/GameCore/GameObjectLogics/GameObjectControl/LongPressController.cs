using FormulaBase;
using GameLogic;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LongPressController : BaseEnemyObjectController
{
    public static int psidx = -1;

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

        var isPunch = GameGlobal.gGameTouchPlay.IsPunch();
#if !UNITY_EDITOR && !UNITY_EDITOR_OSX && !UNITY_EDITOR_64
        isPunch = GameGlobal.gGameTouchPlay.IsPunch (Input.touchCount);
#endif

        if (!md.isLongPressEnd)
        {
            if (isPunch)
            {
                var result = StageBattleComponent.Instance.GetCurLPSPlayResult();
                BattleEnemyManager.Instance.AddHp(this.idx, -1);
                GameGlobal.gGameTouchPlay.TouchResult(idx, result, GameMusic.TOUCH_ACTION_SIGNLE_PRESS); //(mark) attack node damage to it
                BattleEnemyManager.Instance.SetPlayResult(idx, result);
                GameGlobal.gGameMissPlay.SetMissHardTime(0);

                BattleEnemyManager.Instance.SetLongPressEffect(true);
                //GameObject.Destroy(this.gameObject);
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
                    //GameObject.Destroy(this.gameObject);
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
            GameKernel.Instance.IsUnderLongPress = false;
            GameKernel.Instance.IsLongPressFailed = false;

            var result = StageBattleComponent.Instance.GetCurLPSPlayResult();
            StageTeachComponent.Instance.SetPlayResult(idx, result);
            TaskStageTarget.Instance.AddLongPressFinishCount(1);
            StageTeachComponent.Instance.AddPlayCountRecord(1);

            GirlManager.Instance.UnLockActionProtect();
            GirlManager.Instance.AttacksWithoutExchange(GameMusic.NONE, ACTION_KEYS.RUN);
            BattleEnemyManager.Instance.SetLongPressEffect(false);
        }

        return true;
    }
}