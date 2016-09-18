// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using AnimationOrTween;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("NGUI Tools")]
	[Tooltip("Add a child to a Ngui Element.")]
	public class NguiToolsAddChild : FsmStateAction
	{
		[RequiredField]
        [Tooltip("The Parent")]
        public FsmOwnerDefault parent;
		
		[RequiredField]
		[Tooltip("The GameObject to add")]
		public FsmGameObject childReference;
		
		
		[UIHint(UIHint.Variable)]
		public FsmGameObject childInstance;
		
		public override void Reset()
		{
			parent = null;
			childReference = null;
			childInstance = null;
		}
		
		public override void OnEnter()
		{
			GameObject _go = Fsm.GetOwnerDefaultTarget(parent);
			
			childInstance.Value = NGUITools.AddChild(_go,childReference.Value);		
			
			
			UITable mTable = NGUITools.FindInParents<UITable>(childInstance.Value);
			if (mTable != null) { 
				mTable.repositionNow = true; 
			}
			
			UIGrid mGrid = NGUITools.FindInParents<UIGrid>(childInstance.Value);
			if (mGrid != null) { 
				mGrid.repositionNow = true; 
			}
				
			
			Finish();

		}

	}
}