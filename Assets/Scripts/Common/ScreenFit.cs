using System.Collections;
using UnityEngine;

/// <summary>
/// Screen fit.
/// 通过设置摄像机size来使画面尺寸适应不同分辨率
/// </summary>
public class ScreenFit {
	public const int SCREEN_WIDTH = 1334;
	public const int SCREEN_HEIGH = 750;
	public const float UNITY_PIX_UNIT = 0.01f;

	public static void CameraFit(Camera camera) {
		if (camera == null) {
			return;
		}

		float _srate = SCREEN_HEIGH / (float)SCREEN_WIDTH;
		float _wrate = Screen.height / (float)Screen.width;
		/*
		// 方法1,设置摄像机size或者fieldOfView,但无法解决超出屏幕区域显示的问题
		if (camera.orthographic) {
			float _sizerate = ((_wrate - _srate));
			camera.orthographicSize = PixelSizeAdjustment ();
			Debug.Log ("Screen fit orthographic : " + Screen.height + " / " + Screen.width + " (" + camera.orthographicSize + ")");
		} else {
			camera.fieldOfView *= 1 + (_wrate - _srate);
			Debug.Log ("Screen fit perspective : " + Screen.height + " / " + Screen.width + " (" + camera.fieldOfView + ")");
		}
		*/

		// 方法2,固定width, 动态设置Viewport Rect
		float h = Screen.width * _srate;
		float y = h * (_wrate - _srate);
		camera.pixelRect = new Rect (0, y, Screen.width, h);
		Debug.Log ("Screen fit : " + camera.pixelRect.width + " / " + camera.pixelRect.height);
	}

	/*
	private static float PixelSizeAdjustment() {
		Vector2 screen = NGUITools.screenSize;
		float aspect = screen.x / screen.y;
		//float initialAspect = (float)SCREEN_WIDTH / SCREEN_HEIGH;
		float activeHeight = Mathf.RoundToInt (SCREEN_WIDTH / aspect);
		float scaleSize = (2f / activeHeight);

		return UNITY_PIX_UNIT / scaleSize;
	}
	*/
}