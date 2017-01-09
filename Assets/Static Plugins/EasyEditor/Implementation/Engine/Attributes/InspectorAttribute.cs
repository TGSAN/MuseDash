//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using System;

namespace EasyEditor
{
    [Serializable]
	[AttributeUsageAttribute(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
	public class InspectorAttribute : Attribute
    {
        /// <summary>
        /// In which group the UI element belongs to. If not specified the element will be placed in the last group that was specified for an element above it.
        /// </summary>
        public string group = "";

        /// <summary>
        /// The group description which will be displayed just under the group label.
        /// </summary>
        public string groupDescription = "";

        /// <summary>
        /// Flag to display or hide the header of a group of properties.
        /// </summary>
        public bool displayHeader = true;

        /// <summary>
        /// Specify if the group should be foldable or not. Unfoldable by default.
        /// </summary>
        public bool foldable = Settings.foldable;

        /// <summary>
        /// At which position the UI element is rendered inside the group it belongs to. The default value is 100. Thus you don't need to specify every UI element order.
        /// Setting a value inferior to 100 will place the element in front of every element. Setting a value superior to 100 will place the element at the end.
        /// </summary>
        public int order = 100;

        /// <summary>
        /// The type of renderer that should be used to render the field or the method
        /// </summary>
        public string rendererType = null;

        /// <summary>
        /// The identifier to operate some operation to a specific renderer (like hiding it).
        /// </summary>
        public string id = "";

        /// <summary>
        /// Gets the default inspector attribute which correspond to not belonging to any group (rendered first).
        /// </summary>
        /// <value>
        /// The default inspector attribute.
        /// </value>
        public static InspectorAttribute DefaultInspectorAttribute
        {
            get
            {
                return new InspectorAttribute();
            }
        }
    }
}