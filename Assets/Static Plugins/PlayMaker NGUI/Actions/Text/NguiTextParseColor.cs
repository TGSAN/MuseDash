// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using AnimationOrTween;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("NGUI Tools")]
	[Tooltip("Parse a color string into color variable")]
	public class NguiTextParseColor : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The string representation of that color")]
        public  FsmString colorString;
		
		[RequiredField]
		[Tooltip("The Color result")]
		[UIHint(UIHint.Variable)]
		public FsmColor color;
		
		private string _lastColor;
		
		public bool everyFrame;
		
		public override void Reset()
		{
			color = null;
			colorString = null;
			
			everyFrame = false;
		}
		
		public override void OnEnter()
		{

			_lastColor=colorString.Value;
			color.Value = NGUIText.ParseColor(_lastColor,0);
			
			if (!everyFrame)
			{
				Finish();
			}

		}
		public override void OnUpdate()
		{
			if (_lastColor!=colorString.Value)
			{
				_lastColor=colorString.Value;
				color.Value = NGUIText.ParseColor(_lastColor,0);
			}
		}
	}
}