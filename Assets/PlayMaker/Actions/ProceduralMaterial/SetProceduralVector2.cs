// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Substance")]
	[Tooltip("Set a named Vector2 property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
	public class SetProceduralVector2 : FsmStateAction
	{
		[RequiredField]
        [Tooltip("The Substance Material.")]
		public FsmMaterial substanceMaterial;

		[RequiredField]
        [Tooltip("The named vector property in the material.")]
		public FsmString vector2Property;

		[RequiredField]
        [Tooltip("The Vector3 value to set the property to.\nNOTE: Use Set Procedural Vector2 for Vector3 values.")]
		public FsmVector2 vector2Value;

		[Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
		public bool everyFrame;

		public override void Reset()
		{
			substanceMaterial = null;
		    vector2Property = null;
		    vector2Value = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetProceduralVector();

			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetProceduralVector();
		}

	    private void DoSetProceduralVector()
        {
#if !(UNITY_IPHONE || UNITY_IOS || UNITY_ANDROID || UNITY_NACL || UNITY_FLASH || UNITY_PS3 || UNITY_PS4 || UNITY_XBOXONE || UNITY_BLACKBERRY || UNITY_METRO || UNITY_WP8 || UNITY_WIIU || UNITY_PSM || UNITY_WEBGL)

            var substance = substanceMaterial.Value as ProceduralMaterial;
			if (substance == null)
			{
				LogError("The Material is not a Substance Material!");
				return;
			}

			substance.SetProceduralVector(vector2Property.Value, vector2Value.Value);
#endif
        }
	}
}