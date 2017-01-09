//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using System;


namespace EasyEditor
{
	/// <summary>
	/// Start rendering <c>InspecterItemRenderer</c> in a horizontal layout.
	/// </summary>
	[System.Serializable]
	[AttributeUsageAttribute(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
	public class BeginHorizontalAttribute : System.Attribute {
	}
}