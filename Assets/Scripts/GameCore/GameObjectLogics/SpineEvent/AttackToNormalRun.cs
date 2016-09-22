// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using UnityEngine;

namespace GameLogic {
	public class AttackToNormalRun : DoNothing {
		public override void Do (Spine.AnimationState state, int trackIndex, int loopCount) {
			// Air run.
			if (GirlManager.Instance.IsJumpingAction ()) {
				SpineActionController.Play (ACTION_KEYS.STAND_AIR, this.gameObject);
				return;
			}

			// Ground run.
			SpineActionController.Play (ACTION_KEYS.RUN, this.gameObject);
		}
	}
}
