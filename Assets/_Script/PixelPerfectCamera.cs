using UnityEngine;
using System.Collections;

public class PixelPerfectCamera : MonoBehaviour {

	//Unity场景中单元格像内像素容量
	public static float PixelToUnit = 100f;

	//比例值
	public static float scale = 1f;

	//默认分辨率
	public Vector2 NativeResolution = new Vector2 (1136, 640);

	void Awake()
	{
		//创建摄像机变量并获取实例
		var camera = GetComponent<Camera> ();

		//判断是否是正交摄像机
		if (camera.orthographic) 
		{
		 	//计算比例
			scale=Screen. width/NativeResolution.x;

			PixelToUnit *= scale;

			camera.orthographicSize = (Screen.width / 2) / PixelToUnit;

		}
	}
}