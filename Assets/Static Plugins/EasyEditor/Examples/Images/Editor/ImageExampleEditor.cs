using UnityEditor;
using UnityEngine;
using System.Collections;
using EasyEditor;

namespace EasyEditor
{
    [Groups("Preview Texture", "Position Image")]
	[CustomEditor(typeof(ImageExample))]
	public class ImageExampleEditor : EasyEditorBase
	{
        Texture2D oldTexture2D = null;

	    [Inspector(rendererType = "CustomRenderer")]
        void UpdateImage()
        {
            Texture2D currentTexture2D = ((ImageExample)target).myTexture;
            if (currentTexture2D != oldTexture2D)
            {
                if(currentTexture2D != null)
                {
                    ((ImageExample)target).previewTexture = AssetDatabase.GetAssetPath(currentTexture2D);
                }
                else
                {
                    ((ImageExample)target).previewTexture = "";
                }
            }

            oldTexture2D = currentTexture2D;
        }
	}
}