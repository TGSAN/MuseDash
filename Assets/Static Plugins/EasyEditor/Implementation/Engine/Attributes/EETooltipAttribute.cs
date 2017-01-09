//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//	

using System;

namespace EasyEditor
{
	/// <summary>
	/// EETooltip attribute is used to replace UnityEngine.TooltipAttribute where it is not supported.
	/// ButtonRenderer uses it for example since UnityEngine.TooltipAttribute cannot be placed above a function.
	/// </summary>
	[Serializable]
	[AttributeUsageAttribute(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
    public class EETooltipAttribute : Attribute
	{
		/// <summary>
		/// Text content of the tooltip.
		/// </summary>
		public string tooltip = "";

		public EETooltipAttribute(string tooltip)
		{
			this.tooltip = tooltip;
		}
	}
}