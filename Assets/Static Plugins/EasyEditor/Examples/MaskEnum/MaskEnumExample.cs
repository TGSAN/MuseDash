//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

#pragma warning disable 414

using UnityEngine;

namespace EasyEditor
{
    public enum Compatibility
    {
        iOS             = 0x001,
        Android         = 0x002,
        WindowsPhone    = 0x004,
        PS4             = 0x008,
        Wii             = 0x010,
        XBoxOne         = 0x020,
        WindowsDesktop  = 0x040,
        Linux           = 0x080,
        MacOS           = 0x100
    }

    public class MaskEnumExample : MonoBehaviour
    {
        [Image]
        public string easyEditorImage = "Assets/EasyEditor/Examples/icon.png";
        [Space(20f)]

    	[EnumFlag]
        [SerializeField] Compatibility compatibleDevice;

        [Visibility(method = "IsCompatibleWithAConsole")]
        [ReadOnly]
        [SerializeField] string consoleReleaseDate = "2016 02 04";

    	[Inspector]
        public void TestCompatibilityWithMacOS()
        {
            int compatible = (int)compatibleDevice & (int)Compatibility.MacOS;
            Debug.Log((compatible == 0) ? "Not compatible" : "Compatible");
        }

        private bool IsCompatibleWithAConsole()
        {
            return ((int)compatibleDevice & ((int)Compatibility.Wii | (int)Compatibility.XBoxOne | (int)Compatibility.PS4)) != 0;  
        }
    }
}