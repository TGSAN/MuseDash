using DYUnityLib;
using FormulaBase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameLogic
{
    public class GameTouchPlay
    {
        public const uint COMBO_SHOW_MIN = 5;
        public const uint COMBO_EFFECT_PLAY_MIN = 15;
        private const uint COMBO_RECOVER_MIN = 30;
        private const int COMBO_RECOVER_HP = 1;
        private const decimal MUTIL_TOUCH_ENABLE_INTERVAL = 0.05m;

        private readonly int WIDTH_MID = Screen.width / 2;

        private const decimal PRESS_HARD_TIME = 0.15m;

        // touch play about(cd)
        private decimal pressHardTime = -1m;

        // touch slide about
        private decimal beginTouchTick = -1m;

        private float touchTempY = 0;
        private float touchDisY = 0;
        private uint touchPhase = 0;

        /// <summary>
        /// 0 : none
        /// 1 : pumch
        /// 2 : jump
        /// </summary>
        private uint pressState = GameGlobal.PRESS_STATE_NONE;

        public void Init()
        {
            this.pressHardTime = -1m;

            gTrigger.UnRegEvent(GameGlobal.MUSIC_TOUCH_EVENT);
            // Touch event for play
            EventTrigger eTouch = gTrigger.RegEvent(GameGlobal.MUSIC_TOUCH_EVENT);
            GameGlobal.gTouch.AddCustomEvent(eTouch);
            eTouch.Trigger += TouchTrigger;
        }

        /// <summary>
        /// 左跳右打
        /// Sets the right screen.
        /// true : jump
        /// false : pumch
        /// </summary>
        /// <param name="value">If set to <c>true</c> value.</param>
        private void SetPressStateByScreen(uint forcePressState)
        {
            if (forcePressState != GameGlobal.PRESS_STATE_NONE)
            {
                this.SetPressState(forcePressState);
                return;
            }

            if (Input.GetKey(GameGlobal.KC_JUMP) || Input.GetKey(GameGlobal.KC_PUMCH))
            {
                if (Input.GetKey(GameGlobal.KC_JUMP))
                {
                    this.SetPressState(GameGlobal.PRESS_STATE_JUMP);
                }

                if (Input.GetKey(GameGlobal.KC_PUMCH))
                {
                    this.SetPressState(GameGlobal.PRESS_STATE_PUMCH);
                }
            }
            else
            {
                float tx = GameGlobal.gTouch.GetTouchX();
                bool isRightScreen = tx > WIDTH_MID;
                if (isRightScreen)
                {
                    this.SetPressState(GameGlobal.PRESS_STATE_PUMCH);
                }
                else
                {
                    this.SetPressState(GameGlobal.PRESS_STATE_JUMP);
                }
            }
        }

        private void SetPressState(uint state)
        {
            this.pressState = state;
            if (this.IsPunch())
            {
                this.pressHardTime = PRESS_HARD_TIME;
                //GirlManager.Instance.SetJumpingAction (false);
                return;
            }

            if (this.IsJump())
            {
                // jump about.
                //GirlManager.Instance.SetJumpingAction (true);
                TaskStageTarget.Instance.AddJumpCount(1);
                BattleRoleAttributeComponent.Instance.FireSkill(GameMusic.JUMPOVER);
            }
        }

        /// <summary>
        /// 左跳右打
        /// </summary>
        /// <returns><c>true</c> if this instance is right screen; otherwise, <c>false</c>.</returns>
        public bool IsPunch()
        {
            return this.pressState == GameGlobal.PRESS_STATE_PUMCH;
        }

        public bool IsPunch(int touchCount)
        {
            if (touchCount <= 0)
            {
                return false;
            }

            float tx = GameGlobal.gTouch.GetTouchX();
            bool isRightScreen = tx > WIDTH_MID;
            return isRightScreen;
        }

        public bool IsJump()
        {
            return this.pressState == GameGlobal.PRESS_STATE_JUMP;
        }

        public uint GetPressState()
        {
            return this.pressState;
        }

        public bool IsPressing()
        {
            return Input.anyKey;
        }

        public void SetPressHardTime(decimal t)
        {
            this.pressHardTime = t;
        }

        public void TimeStep()
        {
            if (this.pressHardTime > 0)
            {
                this.pressHardTime -= oTimer.dInterval;
            }
        }

        /// <summary>
        /// Touchs the trigger.
        ///
        /// 游戏逻辑的触摸总入口
        /// 触碰、键盘、鼠标操作检测函数
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="triggerId">Trigger identifier.</param>
        /// <param name="args">Arguments.</param>
        public void TouchTrigger(object sender, uint triggerId, params object[] args)
        {
            if (BattleRoleAttributeComponent.Instance.IsDead())
            {
                return;
            }

            if (StageBattleComponent.Instance.IsEndSettlement())
            {
                return;
            }

            if (GirlManager.Instance == null || GirlManager.Instance.isCommingOut)
            {
                return;
            }

            ArrayList musicData = StageBattleComponent.Instance.GetMusicData();
            if (musicData == null || musicData.Count <= 0)
            {
                return;
            }

            //int touchCount = GameGlobal.gTouch.GetTouchCount ();
            //if (touchCount > 1) {
            //	this.MutilTouchPhaser ();
            //	return;
            //}

            GameGlobal.gTouch.AssignTouchPosition();
            this.touchPhase = (uint)args[0];
            if (this.touchPhase == gTrigger.DYUL_EVENT_TOUCH_ENDED)
            {
                this.EndTouchPhaser();
                return;
            }

            if (this.touchPhase == gTrigger.DYUL_EVENT_TOUCH_BEGAN)
            {
                this.BeginTouchPhaser();
                return;
            }

            if (this.touchPhase == gTrigger.DYUL_EVENT_TOUCH_MOVE)
            {
                this.MoveTouchPhaser();
                return;
            }
        }

        private void EndTouchPhaser()
        {
            this.touchDisY = 0;
            this.touchTempY = 0;
            this.beginTouchTick = -1m;

            this.SetPressState(GameGlobal.PRESS_STATE_NONE);
        }

        private void BeginTouchPhaser()
        {
            uint _tempState = GameGlobal.PRESS_STATE_NONE;
            if (Input.GetKey(GameGlobal.KC_JUMP) || Input.GetKey(GameGlobal.KC_PUMCH))
            {
                if (Input.GetKey(GameGlobal.KC_JUMP))
                {
                    _tempState = GameGlobal.PRESS_STATE_JUMP;
                }

                if (Input.GetKey(GameGlobal.KC_PUMCH))
                {
                    _tempState = GameGlobal.PRESS_STATE_PUMCH;
                }
            }
            else
            {
                float tx = GameGlobal.gTouch.GetTouchX();
                bool isRightScreen = tx > WIDTH_MID;
                if (isRightScreen)
                {
                    _tempState = GameGlobal.PRESS_STATE_PUMCH;
                }
                else
                {
                    _tempState = GameGlobal.PRESS_STATE_JUMP;
                }
            }

            if (_tempState == GameGlobal.PRESS_STATE_PUMCH && this.pressHardTime > 0)
            {
                return;
            }

            if (_tempState == GameGlobal.PRESS_STATE_JUMP && GirlManager.Instance.IsJumpingAction())
            {
                return;
            }

            this.touchDisY = 0;
            this.touchTempY = 0;
            this.beginTouchTick = GameGlobal.gGameMusic.GetMusicPassTick();

            this.TouchActionResult(GameMusic.TOUCH_ACTION_SIGNLE_PRESS);
        }

        private void MoveTouchPhaser()
        {
        }

        private void MutilTouchPhaser()
        {
            // If press interval over MUTIL_TOUCH_ENABLE_INTERVAL,
            // Mutil touch fail, do as single touch.
            decimal touchTick = GameGlobal.gGameMusic.GetMusicPassTick();
            decimal interval = touchTick - this.beginTouchTick;

            // Quick double touch.
            if (interval < MUTIL_TOUCH_ENABLE_INTERVAL)
            {
                // use pet skill
                //if (ArmActionController.Instance != null) {
                //	ArmActionController.Instance.UseSkill ();
                //}

                return;
            }
        }

        // ----------------------------- Touch result about -----------------------------
        /// <summary>
        /// Touchs the action result.
        ///
        /// Main function of touch play.
        /// 打击结果处理主函数
        /// 该模块下面的函数均在这个函数的处理流程中被调用
        /// </summary>
        /// <param name="actionType">Action type.</param>
        /// <param name="forcePressState">Force press state.</param>
        // This is the main function of touch result phase.
        public void TouchActionResult(uint actionType, uint forcePressState = GameGlobal.PRESS_STATE_NONE)
        {
            if (actionType == GameMusic.TOUCH_ACTION_NONE)
            {
                return;
            }

            //1, Touch screen/press key punch or jump.
            this.SetPressStateByScreen(forcePressState);
            //2, Get current play time to choose current hit node.
            decimal passedTick = GameGlobal.gGameMusic.GetMusicPassTick();

            if (CommonPanel.GetInstance().showDebug)
            {
                var perfectTime = (StageBattleComponent.Instance.GetPerfectIdxByTick(passedTick)) * 0.01m;
                CommonPanel.GetInstance().DebugTxt((perfectTime - passedTick).ToString());
            }
            List<TimeNodeOrder> tnos = StageBattleComponent.Instance.GetTimeNodeByTick(passedTick);
            if (tnos == null || tnos.Count <= 0)
            {
                if (this.touchPhase == gTrigger.DYUL_EVENT_TOUCH_BEGAN)
                {
                    AttacksController.Instance.ShowAttack(AttacksController.FAIL_PLAY_IDX1, GameMusic.NONE, actionType);
                }
                StageBattleComponent.Instance.curLPSIdx = -1;
                return;
            }

            for (int i = 0; i < tnos.Count; i++)
            {
                TimeNodeOrder tno = tnos[i];
                int _idx = tno.idx;
                if (!this.IsPlayEmpty(_idx, actionType))
                {               //(mark) clean
                    return;
                }

                if (BattleEnemyManager.Instance.GetPlayResult(_idx) > GameMusic.NONE)
                {
                    continue;
                }

                //3, Phase play result(punch or jump ok, cool/great/perfect) of current hit node.
                uint resultCode = tno.result;
                if (tno.mustJump != this.IsJump())
                {
                    resultCode = GameMusic.NONE;
                }

                if (tno.enableJump && this.IsJump())
                {
                    resultCode = GameMusic.JUMPOVER;
                }
                // Jump beat check
                MusicData md = StageBattleComponent.Instance.GetMusicDataByIdx(_idx);
                if (md.nodeData.type == GameGlobal.NODE_TYPE_AIR_BEAT)
                {
                    if (!GirlManager.Instance.IsJumpingAction())
                    {
                        resultCode = GameMusic.NONE;
                    }
                }

                //4, Touch succeed, do touch result.
                this.TouchResult(_idx, resultCode, actionType); //(mark)  show result by id

                if (GameGlobal.IS_DEBUG)
                {
                    Debug.Log(_idx + " play result is " + resultCode);
                }

                if (md.isLongPressStart)
                {
                    StageBattleComponent.Instance.curLPSIdx = _idx;
                }
                break;
            }
        }

        public void TouchResult(int idx, uint resultCode, uint actionType)
        {
            this.pressHardTime = -1m;
            if (resultCode > GameMusic.MISS && resultCode < GameMusic.JUMPOVER)
            {
                BattleEnemyManager.Instance.SetPlayResult(idx, resultCode);
            }

            if (resultCode == GameMusic.NONE)
            {
                AttacksController.Instance.ShowAttack(AttacksController.FAIL_PLAY_IDX2, GameMusic.NONE, actionType);
                return;
            }

            BattleRoleAttributeComponent.Instance.AttackScore(idx, (int)resultCode, (int)actionType);
            // show action with continue attack check
            this.ShowPlayResult(idx, actionType, resultCode);
            BattleRoleAttributeComponent.Instance.AfterAttackScore(idx, (int)resultCode, (int)actionType);

            MusicData md = StageBattleComponent.Instance.GetMusicDataByIdx(idx);
            if (md.nodeData.addCombo)
            {
                this.PlayComboPhaser(resultCode, md.nodeData.isShowPlayEffect);
            }

            // Jump beat pause
            if (md.nodeData.type == GameGlobal.NODE_TYPE_AIR_BEAT)
            {
                if (GirlManager.Instance.IsJumpingAction())
                {
                    GirlManager.Instance.JumpBeatPause();
                }
            }
        }

        private bool IsPlayEmpty(int idx, uint actionType)
        {
            if (idx < 0)
            {
                //if (this.touchPhase == gTrigger.DYUL_EVENT_TOUCH_BEGAN) {
                //	AttacksController.Instance.ShowAttack (AttacksController.FAIL_PLAY_IDX1, GameMusic.NONE, actionType);
                //}

                return false;
            }

            ArrayList musicData = StageBattleComponent.Instance.GetMusicData();
            if (idx >= musicData.Count)
            {
                return false;
            }

            return true;
        }

        private void ShowPlayResult(int idx, uint actionType, uint resultCode)
        {
            AttacksController.Instance.ShowAttack(idx, resultCode, actionType);
            // break cube
            AttacksController.Instance.OnShowAttack(idx, resultCode);
        }

        private void PlayComboPhaser(uint resultCode, bool isShowEffect = true)
        {
            if (StageTeachComponent.Instance.IsTeachingStage())
            {
                return;
            }

            // jump over is no count
            if (resultCode == GameMusic.JUMPOVER)
            {
                return;
            }

            // combo cancle when COOL or less
            //if (resultCode < GameMusic.GREAT) {
            //	StageBattleComponent.Instance.SetCombo (0);
            //	CharPanel.Instance.StopCombo ();
            //	return;
            //}

            int combo = StageBattleComponent.Instance.GetCombo() + 1;
            // combo add, show after more than COMBO_SHOW_MIN.
            StageBattleComponent.Instance.SetCombo(combo);

            if (!isShowEffect)
            {
                return;
            }

            if (combo < COMBO_SHOW_MIN)
            {
                EffectManager.Instance.StopCombo();
            }
            else
            {
                if (combo % COMBO_EFFECT_PLAY_MIN == 0)
                {
                    EffectManager.Instance.ShowCombo(combo);
                }
                else
                {
                    EffectManager.Instance.ShowCombo(combo, false);
                }
            }
        }
    }
}