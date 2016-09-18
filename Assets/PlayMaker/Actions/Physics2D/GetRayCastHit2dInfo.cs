﻿// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Gets info on the last 2d Raycast or LineCast and store in variables.")]
	public class GetRayCastHit2dInfo : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the GameObject hit by the last Raycast and store it in a variable.")]
		public FsmGameObject gameObjectHit;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the world position of the ray hit point and store it in a variable.")]
		[Title("Hit Point")]
		public FsmVector2 point;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the normal at the hit point and store it in a variable.")]
		public FsmVector3 normal;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
		public FsmFloat distance;
		
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
		
		public override void Reset()
		{
			gameObjectHit = null;
			point = null;
			normal = null;
			distance = null;
			everyFrame = false;
		}
		
		public override void OnEnter()
		{
			StoreRaycastInfo();
			
			if (!everyFrame)
			{
				Finish();
			}
		}
		
		public override void OnUpdate()
		{
			StoreRaycastInfo();
		}

        private void StoreRaycastInfo()
        {
            var _info = Fsm.GetLastRaycastHit2DInfo(Fsm);
            if (_info.collider != null)
            {
                gameObjectHit.Value = _info.collider.gameObject;
                point.Value = _info.point;
                normal.Value = _info.normal;
                distance.Value = _info.fraction;
            }
        }
	}
}