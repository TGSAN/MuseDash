﻿// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Iterate through a list of all colliders detected by a RayCast" +
	         "The colliders iterated are sorted in order of increasing Z coordinate. No iteration will take place if there are no colliders within the area.")]
	public class GetNextRayCast2d: FsmStateAction
	{
		[ActionSection("Setup")]
		
		[Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
		public FsmOwnerDefault fromGameObject;
		
		[Tooltip("Start ray at a vector2 world position. \nOr use Game Object parameter.")]
		public FsmVector2 fromPosition;
		
		[Tooltip("A vector2 direction vector")]
		public FsmVector2 direction;
		
		[Tooltip("Cast the ray in world or local space. Note if no Game Object is specified, the direction is in world space.")]
		public Space space;
		
		[Tooltip("The length of the ray. Set to -1 for infinity.")]
		public FsmFloat distance;
		
		[Tooltip("Only include objects with a Z coordinate (depth) greater than this value. leave to none for no effect")]
		public FsmInt minDepth;
		
		[Tooltip("Only include objects with a Z coordinate (depth) less than this value. leave to none")]
		public FsmInt maxDepth;
		
		[ActionSection("Filter")] 
		
		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;
		
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		[ActionSection("Result")] 
		
		[Tooltip("Store the number of colliders found for this overlap.")]
		[UIHint(UIHint.Variable)]
		public FsmInt collidersCount;


		[UIHint(UIHint.Variable)]
		[Tooltip("Store the next collider in a GameObject variable.")]
		public FsmGameObject storeNextCollider;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the 2d position of the next ray hit point and store it in a variable.")]
		public FsmVector2 storeNextHitPoint;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the 2d normal at the next hit point and store it in a variable.")]
		public FsmVector2 storeNextHitNormal;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the distance along the ray to the next hit point and store it in a variable.")]
		public FsmFloat storeNextHitDistance;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the fraction along the ray to the hit point and store it in a variable. If the ray's direction vector is normalised then this value is simply the distance between the origin and the hit point. If the direction is not normalised then this distance is expressed as a 'fraction' (which could be greater than 1) of the vector's magnitude.")]
		public FsmFloat storeNextHitFraction;
		
		[Tooltip("Event to send to get the next collider.")]
		public FsmEvent loopEvent;
		
		[Tooltip("Event to send when there are no more colliders to iterate.")]
		public FsmEvent finishedEvent;
		
		
		private RaycastHit2D[] hits;
		
		private int colliderCount;
		
		// increment an index as we loop through children
		private int nextColliderIndex;
		
		public override void Reset()
		{
			fromGameObject = null;
			fromPosition = new FsmVector2 { UseVariable = true };

			direction = new FsmVector2 { UseVariable = true };

			space = Space.Self;

			minDepth = new FsmInt { UseVariable = true };
			maxDepth = new FsmInt { UseVariable = true };
			
			layerMask = new FsmInt[0];
			invertMask = false;
			
			collidersCount = null;
			storeNextCollider = null;
			storeNextHitPoint = null;
			storeNextHitNormal = null;
			storeNextHitDistance = null;
			storeNextHitFraction = null;
			
			loopEvent = null;
			finishedEvent = null;
		}
		
		public override void OnEnter()
		{
			if (hits == null)
			{
				hits = GetRayCastAll();
				colliderCount = hits.Length;
				collidersCount.Value = colliderCount;
			}
			
			DoGetNextCollider();
			
			Finish();
			
		}

	    private void DoGetNextCollider()
		{
			
			// no more colliders?
			// check first to avoid errors.
			
			if (nextColliderIndex >= colliderCount)
			{
				hits = new RaycastHit2D[0];
				nextColliderIndex = 0;
				Fsm.Event(finishedEvent);
				return;
			}
			
			// get next collider
            Fsm.RecordLastRaycastHit2DInfo(Fsm, hits[nextColliderIndex]);
			storeNextCollider.Value = hits[nextColliderIndex].collider.gameObject;
			storeNextHitPoint.Value = hits[nextColliderIndex].point;
			storeNextHitNormal.Value = hits[nextColliderIndex].normal;
			storeNextHitDistance.Value = hits[nextColliderIndex].distance;
			storeNextHitFraction.Value = hits[nextColliderIndex].fraction;
			
			// no more colliders?
			// check a second time to avoid process lock and possible infinite loop if the action is called again.
			// Practically, this enabled calling again this state and it will start again iterating from the first child.
			
			if (nextColliderIndex >= colliderCount)
			{
				hits = new RaycastHit2D[0];
				nextColliderIndex = 0;
				Fsm.Event(finishedEvent);
				return;
			}
			
			// iterate the next collider
			nextColliderIndex++;
			
			if (loopEvent != null)
			{
				Fsm.Event(loopEvent);
			}
		}


	    private RaycastHit2D[] GetRayCastAll()
		{
			if (Math.Abs(distance.Value) < Mathf.Epsilon)
			{
				return new RaycastHit2D[0];
			}

			var go = Fsm.GetOwnerDefaultTarget(fromGameObject);

			var originPos = fromPosition.Value;
			
			if (go!=null)
			{
				originPos.x += go.transform.position.x;
				originPos.y += go.transform.position.y;
			}
			
			var rayLength = Mathf.Infinity;
			if (distance.Value > 0 )
			{
				rayLength = distance.Value;
			}
			
			var dirVector2 = direction.Value.normalized; // normalized to get the proper distance later using fraction from the rayCastHitinfo.
			
			if(go != null && space == Space.Self)
			{
				
				var dirVector = go.transform.TransformDirection(new Vector3(direction.Value.x,direction.Value.y,0f));
				dirVector2.x = dirVector.x;
				dirVector2.y = dirVector.y;
			}
			
			if (minDepth.IsNone && maxDepth.IsNone)
			{
				return Physics2D.RaycastAll(originPos,dirVector2,rayLength,ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value));
			}else{
				var _minDepth = minDepth.IsNone? Mathf.NegativeInfinity : minDepth.Value;
				var _maxDepth = maxDepth.IsNone? Mathf.Infinity : maxDepth.Value;
				return Physics2D.RaycastAll(originPos,dirVector2,rayLength,ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value),_minDepth,_maxDepth);
			}


		}
		
	}
}

