//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using System;

namespace EasyEditor
{
	/// <summary>
    /// Selectable attribute allows to make the items of a list selectable. An item selected is rendered just below the list.
    /// You need to click in a very specific area of the list to select an element : in the left border that contains the element
    /// drag handles.
	/// </summary>
	[Serializable]
	[AttributeUsageAttribute(AttributeTargets.Field, AllowMultiple = false)]
	public class SelectableAttribute : Attribute
	{
	}
}
