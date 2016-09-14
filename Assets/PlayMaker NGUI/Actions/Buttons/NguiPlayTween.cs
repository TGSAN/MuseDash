// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using AnimationOrTween;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("NGUI")]
	[Tooltip("Play a NGUI Tween. It is the same as the NGUI component 'Button Tween', only without the need for a NGUI button")]
	public class NguiPlayTween : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(UITweener))]
        [Tooltip("The GameObject on which there is one or more NGUI tween.")]
        public FsmOwnerDefault tweenTarget;
		
		[Tooltip("If there are multiple tweens, you can choose which ones get activated by changing their group. Default is 0")]
		public FsmInt tweenGroup;

		[Tooltip("Direction to tween in.")]
		public Direction playDirection;

		[Tooltip("Whether the tween will be reset to the start or end when activated. If not, it will continue from where it currently is.")]
		public FsmBool resetOnPlay;
		
		[Tooltip("Whether the tween will be reset to the start if it's disabled when activated.")]
		public FsmBool resetIfDisabled;

		[Tooltip("What to do if the tweenTarget game object is currently disabled.")]
		public EnableCondition ifDisabledOnPlay;

		[Tooltip("What to do with the tweenTarget after the tween finishes.")]
		public DisableCondition disableWhenFinished;

		[Tooltip("Whether the tweens on the child game objects will be considered.")]
		public FsmBool includeChildren;
		
		[Tooltip("Event to trigger when the tween finishes.")]
		public FsmEvent tweeningFinishedEvent;

		UITweener[] mTweens;
		//bool mStarted = false;
		//bool mHighlighted = false;
		GameObject _tweenTarget;
		
		//void Start () { mStarted = true; if (tweenTarget == null) tweenTarget = gameObject; }

		//void OnEnable () { if (mStarted && mHighlighted) OnHover(UICamera.IsHighlighted(gameObject)); }
		
		private int mActive;
		
		public override void Reset()
		{
			tweenTarget = null;
			tweenGroup = null;
			resetOnPlay = false;
			resetIfDisabled = false;
			ifDisabledOnPlay = EnableCondition.DoNothing;
			disableWhenFinished = DisableCondition.DoNotDisable;
			includeChildren = false;
			tweeningFinishedEvent = null;
		}
		
		public override void OnEnter()
		{
			_tweenTarget = Fsm.GetOwnerDefaultTarget(tweenTarget);
			if (_tweenTarget==null)
			{
				LogWarning("no gameObject target to tween");
				return;
			}
			
			PlayTweeners();
			
			if (disableWhenFinished == DisableCondition.DoNotDisable && tweeningFinishedEvent==null)
			{
				Finish();
			}
			
		}

	
	
		public override void OnUpdate ()
		{

			if (disableWhenFinished != DisableCondition.DoNotDisable && mTweens != null)
			{
				bool isFinished = true;
				bool properDirection = true;
	
				for (int i = 0, imax = mTweens.Length; i < imax; ++i)
				{
					UITweener tw = mTweens[i];
					if (tw.tweenGroup != tweenGroup.Value) continue;
	
					if (tw.enabled)
					{
						isFinished = false;
						break;
					}
					else if ((int)tw.direction != (int)disableWhenFinished)
					{
						properDirection = false;
					}
				}
	
				if (isFinished)
				{
					if (properDirection) NGUITools.SetActive(_tweenTarget, false);
					mTweens = null;
				}
			}
		}
	
		/// <summary>
		/// Activate the tweeners.
		/// </summary>
		public void PlayTweeners()
		{

			if (!NGUITools.GetActive(_tweenTarget))
			{
				// If the object is disabled, don't do anything
				if (ifDisabledOnPlay != EnableCondition.EnableThenPlay) return;
	
				// Enable the game object before tweening it
				NGUITools.SetActive(_tweenTarget, true);
			}
	
			// Gather the tweening components
			mTweens = includeChildren.Value ? _tweenTarget.GetComponentsInChildren<UITweener>() : _tweenTarget.GetComponents<UITweener>();
	
			if (mTweens.Length == 0)
			{
				// No tweeners found -- should we disable the object?
				if (disableWhenFinished != DisableCondition.DoNotDisable) NGUITools.SetActive(_tweenTarget, false);
			}
			else
			{
				bool activated = false;
				bool forward = playDirection == Direction.Forward;
				
				
				// Run through all located tween components
				for (int i = 0, imax = mTweens.Length; i < imax; ++i)
				{
					UITweener tw = mTweens[i];
	
					// If the tweener's group matches, we can work with it
					if (tw.tweenGroup == tweenGroup.Value)
					{
						// Ensure that the game objects are enabled
						if (!activated && !NGUITools.GetActive(_tweenTarget))
						{
							activated = true;
							NGUITools.SetActive(_tweenTarget, true);
						}
	
						++mActive;
	
						// Toggle or activate the tween component
						if (playDirection == Direction.Toggle)
						{
							tw.Toggle();
						}
						else
						{
							if (resetOnPlay.Value || (resetIfDisabled.Value && !tw.enabled)) tw.ResetToBeginning();
							tw.Play(forward);
						}
	
						// Listen for tween finished messages
						EventDelegate.Add(tw.onFinished, OnFinished, true);
					}
				}
			}
		}
		
		/// <summary>
		/// Callback triggered when each tween executed by this script finishes.
		/// </summary>
	
		void OnFinished ()
		{
			if (--mActive == 0)
			{
				Fsm.Event(tweeningFinishedEvent);
				Finish();
			}
		}
			
					
	}
}