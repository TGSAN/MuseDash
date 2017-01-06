//-----------------------------------------------------------------------------------------------
// PopupAnimation 弹窗行为集    Coder:haaaqi
//-----------------------------------------------------------------------------------------------
// 作用：实现各种弹窗行为的核心程序。
//-----------------------------------------------------------------------------------------------
// 使用方法：挂载到作为弹窗的对象身上，配合PopupOpener挂将到功能按钮上来使用。
//-----------------------------------------------------------------------------------------------
// 下一步优化：1.弹窗后背景音乐声音变小变慢。
//-----------------------------------------------------------------------------------------------

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupAnimation : MonoBehaviour
{
	public Color backgroundColor = new Color(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 0.6f);

	public float speed = 0.2f; // 弹窗的速度（时长）。
	public float distance = -500; // 弹窗的位置长度。

	public Ease moveIn;
	public Ease moveOut;

	// 当动画师想要自己调节动画曲线时，将Ease变量注释掉换成下面两行。
//	public AnimationCurve moveIn;
//	public AnimationCurve fadeIn;

	private GameObject m_background;


	// 面板弹出行为。
	public void Open () {
		AddBackground();
	}


	// 如果当前Popup状态为激活，则将其关闭。
	public void Close () {
		Sequence popupSeq = DOTween.Sequence();
		var board = gameObject.GetComponentInChildren<Image>();
		popupSeq.Append (transform.DOMoveY (distance, speed).SetEase (moveOut));
		popupSeq.Insert (0, board.DOFade (0, speed));
		RemoveBackground();
		StartCoroutine(RunPopupDestroy());
	}


	// 关闭弹窗后一定时间后将其摧毁。
	private IEnumerator RunPopupDestroy () {
		yield return new WaitForSeconds(0.2f);
		Destroy(m_background);
		Destroy(gameObject);
	}

	// 背景变暗。
	private void AddBackground () {
		var bgTex = new Texture2D(1, 1);
		bgTex.SetPixel(0, 0, backgroundColor);
		bgTex.Apply();

		m_background = new GameObject("PopupBackground");
		var image = m_background.AddComponent<Image>();
		var rect = new Rect(0, 0, bgTex.width, bgTex.height);
		var sprite = Sprite.Create(bgTex, rect, new Vector2(0.5f, 0.5f), 1);
		image.material.mainTexture = bgTex;
		image.sprite = sprite;
		var newColor = image.color;
		image.color = newColor;
		image.canvasRenderer.SetAlpha(0.0f);
		image.CrossFadeAlpha(1.0f, 0.4f, false);

		var canvas = GameObject.Find("Canvas");
		m_background.transform.localScale = new Vector3(1, 1, 1);
		m_background.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
		m_background.transform.SetParent(canvas.transform, false);
		m_background.transform.SetSiblingIndex(transform.GetSiblingIndex());

		Sequence popupSeq = DOTween.Sequence();
		var board = gameObject.GetComponentInChildren<Image>();
		popupSeq.Append (transform.DOMoveY (distance, speed).From().SetEase(moveIn));
		popupSeq.Insert (0, board.DOFade (0, speed).From());

		var bgm = GameObject.Find ("Bgm");
		bgm.GetComponent<AudioSource> ().pitch = 0.5f;
	}

	// 关闭弹窗时将背景淡出。
	private void RemoveBackground()
	{
		var image = m_background.GetComponent<Image>();
		if (image != null)
			image.CrossFadeAlpha(0.0f, 0.2f, false);

		var bgm = GameObject.Find ("Bgm");
		bgm.GetComponent<AudioSource> ().pitch = 1;
	}
}