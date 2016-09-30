using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
public class TextureGetA_RGB : EditorWindow {

	[MenuItem("RHY/TextureTool")]
	static void GetTextureAndRgb_alpha()
	{
		Texture2D _HandleTex = Selection.activeObject as Texture2D;

		if (_HandleTex != null) 
		{
			string path = AssetDatabase.GetAssetPath (_HandleTex.GetInstanceID ());
			path = path.Remove (path.Length - 4);
			Texture2D _TempTex = new Texture2D (_HandleTex.width, _HandleTex.height);
			Texture2D _TempTex2 = new Texture2D (_HandleTex.width, _HandleTex.height);
			for (int i=0; i < _TempTex.width; i++) {
				for (int j=0; j < _TempTex.height; j++) 
				{
					Color Temp = _HandleTex.GetPixel (i, j);
					_TempTex.SetPixel (i, j, new Color(Temp.r,Temp.g,Temp.b,1f));
					_TempTex2.SetPixel (i, j, Color.white * Temp.a);
				}
			}
			_TempTex.Apply ();
			_TempTex2.Apply ();
			byte[] bytes = _TempTex.EncodeToPNG();
			string path1 = path + "RGB.png";
			File.WriteAllBytes(path1, bytes);

			byte[] byte2s = _TempTex2.EncodeToPNG();
			string path2 = path + "Alpha.png";
			File.WriteAllBytes(path2, byte2s);

			AssetDatabase.SaveAssets();
			AssetDatabase.ImportAsset(path1);
			AssetDatabase.ImportAsset(path2);
		}
	}
}
