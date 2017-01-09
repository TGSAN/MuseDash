//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using System;

namespace EasyEditor
{
    /// <summary>
    /// Constants to easily set default settings of EasyEditor.
    /// </summary>
    public static class Settings
    {
        public enum RenderPosition
        {
            Above,
            Below
        }

        /// <summary>
        /// The indentation in foldable groups and inline serializable class.
        /// </summary>
        public const int indentation = 1;
        /// <summary>
        /// Should groups be foldable by default or not.
        /// </summary>
        public const bool foldable = false;
        /// <summary>
        /// Should comments be render on top or below ui elements.
        /// </summary>
        public const RenderPosition commentRenderPosition = RenderPosition.Below;
        /// <summary>
        /// Should messages be render on top or below ui elements.
        /// </summary>
        public const RenderPosition messageRenderPosition = RenderPosition.Below;
        /// <summary>
        /// Should inline object be unfolded or not when display for the first time in editor
        /// </summary>
        public const bool inlineUnfolded = false;
    }
}