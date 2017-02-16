// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using AnimationOrTween;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("NGUI Tools")]
	[Tooltip("Encode a color into a string for label coloring tags")]
	public class NguiTextEncodeColor : FsmStateAction
	{
		[RequiredField]
        [Tooltip("The Color")]
        public FsmColor color;
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The string representation  result of that color")]
		public FsmString colorString;
		
		private Color _lastColor;
		
		public bool everyFrame;
		
		public override void Reset()
		{
			color = null;
			colorString = null;
			
			everyFrame = false;
		}
		
		public override void OnEnter()
		{
			_lastColor=color.Value;
			colorString.Value = NGUIText.EncodeColor(_lastColor);
			
			if (!everyFrame)
			{
				Finish();
			}

		}
		public override void OnUpdate()
		{
			if (_lastColor!=color.Value)
			{
				_lastColor=color.Value;
				colorString.Value = NGUIText.EncodeColor(_lastColor);
			}
		}
	}
}