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
using Spine;

namespace GameLogic {
	public class FrontRenderObject : DoNothing {
		public override void Do (TrackEntry entry) {
			MeshRenderer render = this.gameObject.GetComponent<MeshRenderer> ();
			if (render == null) {
				return;
			}
			
			render.sortingOrder = GameGlobal.LIMITE_INT;
		}
	}
}
