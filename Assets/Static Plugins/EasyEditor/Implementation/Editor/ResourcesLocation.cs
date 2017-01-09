//
// Copyright (c) 2016 Easy Editor
// All Rights Reserved
//
//

using System.Collections;
using System.IO;
using UnityEngine;

namespace EasyEditor
{
    public class ResourcesLocation
    {
        /// <summary>
        /// Location of the root folder EasyEditor. If you move the location of this folder, you need to update the const string ResourcesLocation.
        /// For example if you move it to Asset/ExternalPlugins/EasyEditor, you need to set ResourcesLocation as :
        /// ResourcesLocation = "Asset/ExternalPlugins/EasyEditor/";
        /// </summary>
        public const string _ResourcesPath = "Assets/Static Plugins/EasyEditor/";

        public static string ResourcesPath
        {
            get
            {
                if (!Directory.Exists(_ResourcesPath))
                {
                    Debug.LogWarning("The folder EasyEditor could not be found at the location indicated by ResourcesLocation class. If you moved it in some subfolder, please update the location in ResourcesLocation class.");
                }

                return _ResourcesPath;
            }
        }
    }
}