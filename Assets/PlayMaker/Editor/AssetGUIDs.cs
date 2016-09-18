using UnityEditor;
using UnityEngine;

namespace HutongGames.PlayMakerEditor
{
    /// <summary>
    /// Collect Playmaker Asset GUIDs as static strings
    /// Used instead of asset paths (user can move files)
    /// Also can be used to check for installation problems
    /// </summary>
    public class AssetGUIDs
    {
        public static string PlayMakerDll = "e743331561ef77147ae48cda9bcb8209";
        public static string PlayMakerEditorDll = "336aa50a81ce85b47b50a7b6adf85a76";
        public static string ConditionalExpressionDll = "d4efecccbe1d6134f99fa8da66d82942";
        public static string ConditionalExpressionEditorDll = "3588691a691f1074eb5388783b2d2f5d";
        public static string PlayMakerMetroDll = "fd7aabeb995f6a64aa68d02891fc2294";
        public static string PlayMakerWebGLDll = "9754d4abda502c6458053d5ed8e4fc5a";
        public static string PlayMakerWP8Dll = "de72a6d2da64d114d95e3c5a01cfaec5";
        public static string PlayMakerUnitypackage1784 = "dd583cbbf618ba54983cdf396b28e49b";
        public static string PlayMakerUnitypackage180 = "f982487afa4f0444ea11e90a9d05b94e";
        public static string PlayMakerUnitypackage181 = "0921e97db908b2f4e8e407e68a2ed27c";
        public static string PlayMakerUnitypackage182 = "cd593cc3ded027746bf4658e85cb9fb9";

        public static string GetFullAssetPath(string assetGUID)
        {
            var path = AssetDatabase.GUIDToAssetPath(assetGUID);
            if (!string.IsNullOrEmpty(path))
            {
                // strip Assets from asset path since it's in dataPath
                path = Application.dataPath + path.Substring(6); 
            }
            return path;
        }
    }
}

