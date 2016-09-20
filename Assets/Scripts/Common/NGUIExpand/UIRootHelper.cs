using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FormulaBase;

/// <summary>
/// User interface root helper.
/// 单个UI perfab的根节点分析器
/// 通过工具自动挂载，负责管理旗下所有子节点
/// </summary>
public class UIRootHelper : MonoBehaviour {
	public const string ON_CLICK_METHOD = "OnClickCommon";
	public const string ON_ANI_FINISHED_METHOD = "OnAnimationFinishCommon";
	public const string ON_SLIDER_METHOD = "OnSliderCommon";
	private const string DO_METHOD = "Do";

	public const BindingFlags B_FLAGS = BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod; 

	private static System.Reflection.Assembly assembly = System.Reflection.Assembly.Load("Assembly-CSharp");

	/// <summary>
	/// The name of the object to module.
	/// 
	/// onClick统一入口，然后会分配到对应配置模块
	/// onClick will all go into OnClickCommon and sperate by objectToModuleName
	/// </summary>
	private Dictionary<GameObject, Type> objectToModuleName = null;

	/// <summary>
	/// The name of the object to ani finished.
	/// 
	/// onFinish统一入口，然后会分配到对应配置模块
	/// Tween onFinish will all go into OnAnimationFinishCommon and sperate by objectToAniFinishedName
	/// </summary>
	private Dictionary<GameObject, Type> objectToAniFinishedName = null;

	/// <summary>
	/// The name of the object to on slide.
	/// 
	/// onSlider统一入口，然后会分配到对应配置模块
	/// Slider onSlider will all go into OnSliderCommon and sperate by objectToOnSliderName
	/// </summary>
	private Dictionary<GameObject, Type> objectToOnSliderName = null;

	[SerializeField]
	/// <summary>
	/// 是否单例界面
	/// 单例界面里面的UIPhaseHelper可自动关联host作为数据源
	/// 非单例界面则需手动调用SetLabelHost进行host关联
	/// 例如物品图标之类的动态元素只能作为非单例界面
	/// </summary>
	public bool isSingleInstanceUI;
	/// <summary>
	/// The is show on active.
	/// 是否加载即显示
	/// </summary>
	public bool isShowOnLoaded;

	/// <summary>
	/// The widgets.
	/// 被分析后带有UIPhaseHelper的子对象
	/// </summary>
	public List<UIPhaseHelper> widgets;

	// Use this for initialization
	void Start () {
		this.objectToModuleName = new Dictionary<GameObject, Type> ();
		this.objectToAniFinishedName = new Dictionary<GameObject, Type> ();
		this.objectToOnSliderName = new Dictionary<GameObject, Type> ();
		foreach (UIPhaseHelper _s in this.widgets) {
			if (_s == null ||  _s.gameObject == null) {
				continue;
			}

			Type tpe = this.GetModule(_s.onClickModuleName); // assembly.GetType (_moduledName);
			Type tpeAni = this.GetModule(_s.onTweenAniFinishedModuleName);
			Type tpeSlider = this.GetModule (_s.onSliderModuleName);
			//MethodInfo method = tpe.GetMethod (DO_METHOD);
			//tpe.InvokeMember(_s.moduleName);

			this.objectToModuleName [_s.gameObject] = tpe;
			this.objectToAniFinishedName [_s.gameObject] = tpeAni;
			this.objectToOnSliderName [_s.gameObject] = tpeSlider;

			_s.Init ();
		}

		if (UISceneHelper.Instance != null) {
			UISceneHelper.Instance.RegDymWidget (this.gameObject.name, this);
		}
	}

	void OnEnable() {
		if (UISceneHelper.Instance != null) {
			UISceneHelper.Instance.RegDymWidget (this.gameObject.name, this);
		}
	}

	public Type GetModule(string moduleName) {
		string _moduleName = this.gameObject.name + "." + moduleName;
		//Debug.Log ("Init ui module " + _moduledName);
		return assembly.GetType (_moduleName);
	}

	public int RegWidget(UIPhaseHelper uph) {
		if (uph.gameObject == null) {
			return -1;
		}

		for (int i = 0; i < this.widgets.Count; i++) {
			UIPhaseHelper _s = this.widgets [i];
			if (_s == null || _s.gameObject == null) {
				continue;
			}

			if (_s.gameObject == uph.gameObject) {
				this.widgets [i] = _s;
				return i;
			}
		}

		this.widgets.Add (uph);
		return this.widgets.Count - 1;
	}

	/// <summary>
	/// Sets the label host.
	/// 
	/// 设置影响label text的host
	/// </summary>
	/// <param name="host">Host.</param>
	public void SetLabelHost(FormulaHost host) {
		if (this.widgets == null) {
			return;
		}

		foreach (UIPhaseHelper uph in this.widgets) {
			if (uph == null) {
				continue;
			}

			uph.SetLabelHost (host);
		}
	}

	/// <summary>
	/// Sets the active host.
	/// 
	/// 设置影响active的host
	/// </summary>
	/// <param name="host">Host.</param>
	public void SetActiveHost(FormulaHost host) {
		if (this.widgets == null) {
			return;
		}

		foreach (UIPhaseHelper uph in this.widgets) {
			if (uph == null) {
				continue;
			}

			uph.SetActiveHost (host);
		}
	}

	/// <summary>
	/// Sets the slider host.
	/// 
	/// 设置影响slider的host
	/// </summary>
	/// <param name="host">Host.</param>
	public void SetSliderHost(FormulaHost host) {
		if (this.widgets == null) {
			return;
		}

		foreach (UIPhaseHelper uph in this.widgets) {
			if (uph == null) {
				continue;
			}

			uph.SetSliderHost (host);
		}
	}

	public void AutoSetLabelHostForTable() {
		if (this.widgets == null) {
			return;
		}

		foreach (UIPhaseHelper uph in this.widgets) {
			if (uph == null) {
				continue;
			}

			uph.AutoSetLabelHostForTable ();
		}
	}

	/// <summary>
	/// Raises the click common event.
	/// </summary>
	/// <param name="clickObject">Click object.</param>
	public void OnClickCommon(GameObject clickObject) {
		if (this.objectToModuleName == null) {
			return;
		}

		if (!this.objectToModuleName.ContainsKey (clickObject)) {
			return;
		}

		Type _tpe = this.objectToModuleName [clickObject];
		if (_tpe == null) {
			return;
		}

		_tpe.InvokeMember (DO_METHOD, B_FLAGS, null, null, new object[]{clickObject});
	}
	
	/// <summary>
	/// Raises the animation finish common event.
	/// </summary>
	/// <param name="aniObject">Animation object.</param>
	public void OnAnimationFinishCommon(GameObject aniObject) {
		if (this.objectToAniFinishedName == null) {
			return;
		}
		
		if (!this.objectToAniFinishedName.ContainsKey (aniObject)) {
			return;
		}
		
		Type _tpe = this.objectToAniFinishedName [aniObject];
		if (_tpe == null) {
			return;
		}

		_tpe.InvokeMember (DO_METHOD, B_FLAGS, null, null, new object[]{aniObject});
	}

	/// <summary>
	/// Raises the slider common event.
	/// </summary>
	/// <param name="sliderObject">Slider object.</param>
	public void OnSliderCommon(GameObject sliderObject) {
		if (this.objectToOnSliderName == null) {
			return;
		}

		if (!this.objectToOnSliderName.ContainsKey (sliderObject)) {
			return;
		}

		Type _tpe = this.objectToOnSliderName [sliderObject];
		if (_tpe == null) {
			return;
		}

		_tpe.InvokeMember (DO_METHOD, B_FLAGS, null, null, new object[]{sliderObject});
	}

	public void OnNotifyDynamicDataChange(FormulaHost host, string key, object value) {
		if (this.widgets == null) {
			return;
		}

		for (int i = 0; i < this.widgets.Count; i++) {
			UIPhaseHelper _s = this.widgets [i];
			if (_s == null || _s.gameObject == null) {
				continue;
			}

			_s.OnNotifyDynamicDataChange (host, key, value);
		}
	}
}
