using UnityEngine;
using System.Collections;

namespace EasyEditor
{
    public class ImageExample : MonoBehaviour {

        [Inspector(group = "Preview Texture")]

        [Image]
        public string previewTexture = "";
        [Comment("Drag the texture here to change the preview")]
        public Texture2D myTexture;


        [Inspector(group = "Position Image")]

        [Image(alignement = ImageAlignement.Left, size = 40f)]
        public string positionImage1 = "Assets/EasyEditor/Examples/icon.png";

        [Image(alignement = ImageAlignement.Right, size = 30f)]
        public string positionImage2 = "Assets/EasyEditor/Examples/icon.png";
    }
}