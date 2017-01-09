//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using System;

namespace EasyEditor
{
    /// <summary>
    /// Specify the type of field an <c>InspectorItemRender</c> can render.
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Class, AllowMultiple = true)]
    public class RenderTypeAttribute : System.Attribute
    {
        /// <summary>
        /// Which type of field can be rendered by the class the attribute is associated to.
        /// </summary>
        public Type type;

        public RenderTypeAttribute(Type aType)
        {
            this.type = aType;
        }
    }
}