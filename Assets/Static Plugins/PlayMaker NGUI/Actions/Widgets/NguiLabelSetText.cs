// (c) Copyright HutongGames, LLC 2010-2014. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("NGUI")]
	[Tooltip("Sets the text of a Ngui Label")]
	public class NguiLabelSetText : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(UILabel))]
        [Tooltip("The GameObject on which there is a UILabel")]
        public FsmOwnerDefault gameObject;
		
		[Tooltip("The label")]
		public FsmString text;
		
		[Tooltip("Repeat every frame while the state is active. Useful to change the text over time")]
		public bool everyFrame;
		
		
		private UILabel _label;
		
		
		public override void Reset()
		{
			gameObject = null;
			text =null;
		}
		
		public override void OnEnter()
		{
			GameObject _go  = Fsm.GetOwnerDefaultTarget(gameObject);
			if (_go==null)
			{
				LogWarning("no gameObject");
				Finish();
				return;
			}
			
			_label = _go.GetComponent<UILabel>();
			if (_label == null)
			{
				LogWarning("no UILabel");
				Finish();
				return;
			}
			
			SetText();
			
			if(!everyFrame)
			{
				Finish();
			}
		}

	
	
		public override void OnUpdate ()
		{
			SetText();
		}
		
		void SetText()
		{
			_label.text = text.Value;
		}
			
	}
}