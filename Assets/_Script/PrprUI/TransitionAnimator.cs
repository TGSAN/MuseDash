//-----------------------------------------------------------------------------------------------
// TransitionAnimator 场景切换器    Coder:haaaqi
//-----------------------------------------------------------------------------------------------
// 将此脚本挂载到切换场景的BUTTON上，在脚本组件面板填写场景名称，自由调节参数。
// 在BUTTON上增加一个On Click事件，将BUTTON拖入对象框，选择执行本脚本中的PerformTransition方法即可。
// 注意：需要在菜单File/Build Setting中添加需要切换的场景。
//-----------------------------------------------------------------------------------------------
// 下一步优化：1.自动加载Sences in build中的场景，可以下拉选择。
//            2.在Transition脚本中增加对当前场景和目标场景的出场入场动画控制。
//-----------------------------------------------------------------------------------------------

using UnityEngine;

public class TransitionAnimator : MonoBehaviour
{
	[Space(5)]
	public string targetScene = "<Insert scene name>";
	[Space(5)]
	public float duration = 1.0f;
	[Space(5)]
	public Color fadeColor = Color.black;
	public float fadeDelay = 0;

	public void PerformTransition()
	{
		TransitionAnimation.LoadScene(targetScene, duration, fadeDelay, fadeColor);// 场景切换的核心逻辑在脚本Transition中。
	}
}