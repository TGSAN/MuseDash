//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using System;

namespace EasyEditor
{
	/// <summary>
	/// Comment attribute allows to place a comment under an <c>InspecterItemRenderer</c> in the inspector inside an info box.
	/// </summary>
	[Serializable]
	[AttributeUsageAttribute(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
    public class CommentAttribute : Attribute {

		/// <summary>
		/// Text content of the boxed comment label.
		/// </summary>
		public string comment = "";

		public CommentAttribute(string comment)
		{
			this.comment = comment;
		}
	}
}