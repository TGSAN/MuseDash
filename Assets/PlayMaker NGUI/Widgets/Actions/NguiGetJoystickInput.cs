// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("NGUI")]
	[Tooltip("Gets the input values from a UIJoystick")]
	//[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1012")]
	public class NguiGetJoystickInput : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(UIJoystick))]
        [Tooltip("The GameObject featuring the UIJoystick component.")]
        public FsmOwnerDefault gameObject;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("The Horyzontal and Vertical Input values")]
		public FsmVector2 input;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("The pad angle")]
		public FsmFloat angle;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("The pad input plus the  angle in the z value of the vector3. Use this for network synching, saves bandwidth")]
		public FsmVector3 inputAndAngle;
		
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
		
		UIJoystick _joystick;
		
		public override void Reset()
		{
			gameObject = null;
			input = null;
			angle = null;
			inputAndAngle = null;
			everyFrame = true;
		}

		public override void OnEnter()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null)
			{
				return;
			}

			_joystick = go.GetComponent<UIJoystick>();

			if (_joystick == null)
			{
				return;
			}
			
			doGetPadInputs();
			
			if (!everyFrame)
				Finish();		
		}

		public override void OnUpdate()
		{
			doGetPadInputs();
		}
		
		void doGetPadInputs()
		{
			if (!input.IsNone)
			{
				input.Value = _joystick.padPosition;
			}
			if (!angle.IsNone)
			{
				angle.Value = _joystick.padAngle;
			}
			if (!inputAndAngle.IsNone)
			{
				inputAndAngle.Value = _joystick.padPositionAndAngle;
			}
		}
		
	}
}

