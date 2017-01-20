//----------------------------------------------------------------------------
// TransitionAnimation 场景切换核心程序    Coder:haaaqi
//----------------------------------------------------------------------------
// 说明：不需要挂载到任何对象上。
//----------------------------------------------------------------------------
// 下一步优化：1.将切换场景时的遮罩对象修改为预设，方便加载各种Loading图或载入动画。
//            2.增加对当前场景和目标场景的出场入场动画控制。
//----------------------------------------------------------------------------

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TransitionAnimation : MonoBehaviour {

	private static GameObject m_canvas;
	private GameObject m_overlay;


	// 创建一个新的的画布，并阻止其在载入新场景时被自动销毁（可能是因为原有画布和其内部的对象会随场景销毁一起销毁）。
	private void Awake () {
		m_canvas = new GameObject("TransitionCanvas");// 创建一个供场景转换使用的名为TransitionCanvas的对象。
		var canvas = m_canvas.AddComponent<Canvas>();// 为其添加一个Canvas组件。
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;// 设置其渲染模式。
		DontDestroyOnLoad(m_canvas);// 此API作为用阻止对象在场景切换中被自动销毁。
	}

	// 创建Transition对象，进行场景切换行为。
	public static void LoadScene (string targetScene, float duration, float fadeDelay, Color fadeColor) {
		var fade = new GameObject("Transition");// 创建一个名为Transition的GameObject。
		fade.AddComponent<TransitionAnimation>();// 为其添加名为Transition的脚本。
		fade.GetComponent<TransitionAnimation>().StartFade(targetScene, duration, fadeDelay, fadeColor);// 执行脚本中的StartFade方法。
		fade.transform.SetParent(m_canvas.transform, false);// 创建位置在m_canvas之下。
		fade.transform.SetAsLastSibling();// Move the transform to the end of the local transform list.
		DontDestroyOnLoad(fade);// 此API作为用阻止对象在场景切换中被自动销毁。
	}

	// 
	private void StartFade (string targetScene, float duration, float fadeDelay, Color fadeColor) {
		StartCoroutine(RunFade(targetScene, duration, fadeDelay, fadeColor));
	}


	// 场景转换方法。
	private IEnumerator RunFade (string targetScene, float duration, float fadeDelay, Color fadeColor) {
		
		// 创建转场遮罩。
		// ------------------------------------------------------------------------------------------------------------------------
		var bgTex = new Texture2D(1, 1);
		bgTex.SetPixel(0, 0, fadeColor);
		bgTex.Apply();

		m_overlay = new GameObject();// 创建一个新的GameObject。
		var image = m_overlay.AddComponent<Image>();// 为其添加Image组件。
		var rect = new Rect(0, 0, bgTex.width, bgTex.height);
		var sprite = Sprite.Create(bgTex, rect, new Vector2(0.5f, 0.5f), 1);// 创建一个Sprite。
		image.material.mainTexture = bgTex;// 将创建好的纹理赋给image组件里材质的主贴图。
		image.sprite = sprite;// 将Sprite赋给Image组件里的sprite属性。
		var newColor = image.color;
		image.color = newColor;
		image.canvasRenderer.SetAlpha(0.0f);// 将image的初始Alpha设为0。

		m_overlay.transform.localScale = new Vector3(1, 1, 1);// 将遮罩1比1显示。
		m_overlay.GetComponent<RectTransform>().sizeDelta = m_canvas.GetComponent<RectTransform>().sizeDelta;// 设置遮罩的屏幕适配。
		m_overlay.transform.SetParent(m_canvas.transform, false);// 指定层级位置。
		m_overlay.transform.SetAsFirstSibling();// Move the transform to the end of the local transform list.

		// 场景淡出行为。
		// ------------------------------------------------------------------------------------------------------------------------
		yield return new WaitForSeconds(fadeDelay);
		var time = 0.0f;
		var halfDuration = duration / 2.0f;
		while (time < halfDuration) {
			time += Time.deltaTime;
			image.canvasRenderer.SetAlpha(Mathf.InverseLerp(0, 1, time / halfDuration));
			yield return new WaitForEndOfFrame();
		}

//		image.canvasRenderer.SetAlpha(1.0f);// 将image的Alpha显示为1。
		yield return new WaitForEndOfFrame();

		SceneManager.LoadScene (targetScene);

		// 场景淡入行为。
		// ------------------------------------------------------------------------------------------------------------------------
		time = 0.0f;
		while (time < halfDuration) {
			time += Time.deltaTime;
			image.canvasRenderer.SetAlpha(Mathf.InverseLerp(1, 0, time / halfDuration));
			yield return new WaitForEndOfFrame();
		}

		image.canvasRenderer.SetAlpha(0.0f);
		yield return new WaitForEndOfFrame();

		Destroy(m_canvas);
	}
}