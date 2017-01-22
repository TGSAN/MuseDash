//-----------------------------------------------------------------------------------------------
// PopupAnimation 弹窗行为集    Coder:haaaqi
//-----------------------------------------------------------------------------------------------
// 作用：实现各种弹窗行为的核心程序。
//-----------------------------------------------------------------------------------------------
// 使用方法：挂载到作为弹窗的对象身上，配合PopupOpener挂将到功能按钮上来使用。
//-----------------------------------------------------------------------------------------------
// 下一步优化：1.在该游戏对象其下创建一个全屏按钮（不含图象，不显示文字），显示在最底层，在Onclick事件里指定父级对
//象，调用其PopupAnimation脚本中的Close方法。这样就不用每个一弹窗对象都要手动创建一个点击空白区域返回的按钮了。
//-----------------------------------------------------------------------------------------------

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupAnimation : MonoBehaviour
{
	public Color backgroundColor = new Color(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 0.6f);

	public float speed = 0.4f; // 弹窗的速度（时长）。
	public float distance = -100; // 弹窗的位置长度。

	public Ease moveIn;
	public Ease moveOut;

	// 当动画师想要自己调节动画曲线时，将Ease变量注释掉换成下面两行。
//	public AnimationCurve moveIn;
//	public AnimationCurve fadeIn;

	private GameObject m_background;
	private GameObject m_CancelButton;

	// 面板弹出行为。
	public void Open () 
	{
		var bgTex = new Texture2D(1, 1);
		bgTex.SetPixel(0, 0, backgroundColor);
		bgTex.Apply();

//		//创建一个变暗的背景。
//		m_background = new GameObject("PopupBackground");
//		var image = m_background.AddComponent<Image>();
//		var rect = new Rect(0, 0, bgTex.width, bgTex.height);
//		var sprite = Sprite.Create(bgTex, rect, new Vector2(0.5f, 0.5f), 1);
//		image.material.mainTexture = bgTex;
//		image.sprite = sprite;
//		var newColor = image.color;
//		image.color = newColor;
//		image.canvasRenderer.SetAlpha(0.0f);
//		image.CrossFadeAlpha(1.0f, 0.2f, false);
//		//决定变暗背景的层级位置。
//		var canvas = GameObject.Find("Canvas");
//		m_background.transform.localScale = new Vector3(1, 1, 1);
//		m_background.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
//		m_background.transform.SetParent(canvas.transform, false);
//		m_background.transform.SetSiblingIndex(transform.GetSiblingIndex());

		//尝试将创建一个按钮,让玩家点击空白区可以退出弹窗。
		m_CancelButton = new GameObject("BtnCancel");
		var button = m_CancelButton.AddComponent<Button>()as Button;
		var image = m_CancelButton.AddComponent<Image> ();
		var rect = new Rect(0, 0, bgTex.width, bgTex.height);
		var sprite = Sprite.Create(bgTex, rect, new Vector2(0.5f, 0.5f), 1);
		image.material.mainTexture = bgTex;
		image.sprite = sprite;
		var newColor = image.color;
		image.color = newColor;
		image.color = backgroundColor;
		image.canvasRenderer.SetAlpha(0.0f);
		image.CrossFadeAlpha(1.0f, 0.1f, false);
		button.onClick.AddListener(Close);
		//决定上面创建 的BtnCancel的的层级位置。
		var parent = GameObject.Find("UIManager");
		m_CancelButton.transform.localScale = new Vector3(1, 1, 1);
		m_CancelButton.GetComponent<RectTransform>().sizeDelta = parent.GetComponent<RectTransform>().sizeDelta;
		m_CancelButton.transform.SetParent(parent.transform, false);
		m_CancelButton.transform.SetSiblingIndex(transform.GetSiblingIndex());

		//窗口弹出的行为逻辑。
		var boards = gameObject.GetComponentsInChildren<Image>();
		var texts = gameObject.GetComponentsInChildren<Text>();
		Sequence popupSeq = DOTween.Sequence();
		popupSeq.Insert (0.15f, transform.DOLocalMoveY (distance, speed).From().SetEase(moveIn));
		foreach (var board in boards) 
		{
			popupSeq.Insert (0.15f, board.DOFade (0, 0.1f).From());
		}
		foreach (var text in texts) 
		{
			popupSeq.Insert (0.15f, text.DOFade (0, 0.1f).From());
		}

		//		var bgm = GameObject.Find ("Bgm");
		//		bgm.GetComponent<AudioSource> ().pitch = 0.5f;
	}


	// 如果当前Popup状态为激活，则将其关闭。
	public void Close () 
	{
		Sequence popupSeq = DOTween.Sequence();
		var boards = gameObject.GetComponentsInChildren<Image>();
		var texts = gameObject.GetComponentsInChildren<Text>();
//		popupSeq.Insert (0.1f, transform.DOLocalMoveY (distance, speed).SetEase (moveOut));
		foreach (var board in boards) 
		{
			popupSeq.Insert (0, board.DOFade (0, 0.05f));
		}
		foreach (var text in texts) 
		{
			popupSeq.Insert (0, text.DOFade (0, 0.05f));
		}
		RemoveBackground();
		StartCoroutine(RunPopupDestroy());
	}


	// 关闭弹窗后一定时间后将其摧毁。
	private IEnumerator RunPopupDestroy () 
	{
		yield return new WaitForSeconds(0.25f);
		Destroy(m_CancelButton);
		Destroy(gameObject);
	}


	// 关闭弹窗时将背景淡出。
	private void RemoveBackground()
	{
		var image = m_CancelButton.GetComponent<Image>();
		if (image != null)
			image.DOFade (0, 0.2f).SetDelay (0.05f);
//			image.CrossFadeAlpha(0.0f, 0.2f, false);

//		var bgm = GameObject.Find ("Bgm");
//		bgm.GetComponent<AudioSource> ().pitch = 1;
	}
}