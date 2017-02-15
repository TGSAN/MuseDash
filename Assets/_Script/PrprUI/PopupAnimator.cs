//-----------------------------------------------------------------------------------------------
// PopupAnimator 弹窗触发器    Coder:haaaqi
//-----------------------------------------------------------------------------------------------
// 作用：触发一个对象的弹窗和关闭行为，具体的弹窗行为逻辑在PopupStates脚本内。
//-----------------------------------------------------------------------------------------------
// 使用方法：将此脚本挂载到需要的BUTTON上，并在此脚本组件内挂载弹窗对象的预设。
// 在本BUTTON上增加一个On Click事件，将BUTTON自身拖入对象框，选择执行本脚本中的自定义方法OpenPopup即可。
// 需要配合脚本PopupStates使用。
//-----------------------------------------------------------------------------------------------
// 下一步优化：兼容多种弹穿风格，增加可选项。
//-----------------------------------------------------------------------------------------------
// 问题1：想让GameObject在游戏启动时出现在指定的Position（怎么获取基于RectTransform的坐标）上并保持透明（怎样获取UGUI的Image组件中的Color属性？）。
// 问题2：用onClick事件控制DOTween动画时，若在动画没有播放完时再次触发了OnClick事件，则会将GameObject当前的状态视为动画的起/终点，导致UI偏移，这种情况要怎么办？
//      DOTween的Shortcuts不能手动设定动画的起始位置吗?
// 问题3：DOTween除Transform以外的属性动画可以通过不Copy脚本的方式被子物体继承吗？假如一个弹窗上有多个Image图标和文字，有没有对多个对象的透明度进行统一管理的方式？
//-----------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupAnimator : MonoBehaviour {

	public GameObject popupPrefab; // 弹窗对象预设。
	protected Canvas m_canvas;

	protected void Start () {
		m_canvas = GameObject.Find("UIManager").GetComponent<Canvas>();
	}
		
	public virtual void OpenPopup()
	{
		var popup = Instantiate(popupPrefab) as GameObject; //将预设实例化。
		popup.SetActive(true); // 且设置为Active为True状态。

		// 创建到m_canvas以下的层级。
		popup.transform.SetParent(m_canvas.transform, false);

		// 获取预设的PopupStates组件中名为Open的方法来完成弹窗事件。
		popup.GetComponent<PopupAnimation>().Open();
	}
}