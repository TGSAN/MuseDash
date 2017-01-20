using UnityEngine;
using System.Collections;

[AddComponentMenu("Image Effects/Blur Behind Viewport")]
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class BlurBehindViewport : MonoBehaviour {

    void OnPreRender()
    {
        BlurBehind.SetViewport();
    }

    void OnPostRender()
    {
        BlurBehind.ResetViewport();
    }
}
