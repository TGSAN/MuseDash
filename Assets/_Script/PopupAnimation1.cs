//问题1：想让GameObject在游戏启动时出现在指定的Position（怎么获取基于RectTransform的坐标）上并保持透明（怎样获取UGUI的Image组件中的Color属性？）。
//问题2：用onClick事件控制DOTween动画时，若在动画没有播放完时再次触发了OnClick事件，则会将GameObject当前的状态视为动画的起/终点，导致UI偏移，这种情况要怎么办？
//      DOTween的Shortcuts不能手动设定动画的起始位置吗?
//问题3：DOTween除Transform以外的属性动画可以通过不Copy脚本的方式被子物体继承吗？假如一个弹窗上有多个Image图标和文字，有没有对多个对象的透明度进行统一管理的方式？


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupAnimation1 : MonoBehaviour {

	public Button button;
	public float speed = 0.2f;
	public float distance = -200;

	public Image image;

	void Start () {
		



		Sequence popup = DOTween.Sequence();

		button.onClick.AddListener (() => {
			gameObject.SetActive(true);
			popup.Append (transform.DOMoveY (distance, speed).From());
			popup.Insert (0,image.DOFade (0, speed).From());

		});
	}
}