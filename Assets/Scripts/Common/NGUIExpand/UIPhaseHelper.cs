using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FormulaBase;

/// <summary>
/// User interface phase helper.
/// 带UIRootHelper根节点的UI perfab下属子节点的分析器
/// 用于操作和配置具体的界面控件功能和数据、行为连接
/// </summary>
public class UIPhaseHelper : MonoBehaviour {
	private const string GET_DATA_METHOD = "GetData";

	private string _dymUid = null;

	private UISprite _sprite;
	private UILabel _label;
	private UIButton _button;
	private UITweener _tweener;
	private UISlider _slider;
	private UIScrollView _scorll;
	private List<GameObject> _tableItems;
	private Type _tTableDataFrom;

	private FormulaHost hostActive;
	private FormulaHost hostLabel;
	private FormulaHost hostSlider;

	[SerializeField]
	public GameObject root;
	// 点击响应对象
	public List<GameObject> clickResponseObjects;
	public List<string> clickResponseParentNames;
	public List<bool> clickResponseObjectsIsShow;
	public List<bool> clickResponseParentNamesIsShow;
	public List<string> clickResponseObjectAnimations;
	public List<string> clickResponseParentAnimations;

	public bool isFoldObjs;
	public bool isFoldParentObjs;

	// 是否强制保留
	public bool isForceKeep;

	// 点击后是否隐藏自己
	public bool isClickHideSelf;
	// 点击后是否隐藏父节点
	public bool isClickHideParent;

	public string des;

	// host相关
	public int activeHostKeyId;
	public string activeCondiction;
	public string activeValue;

	public int labelHostKeyId;
	public string labelMatchSign;
	public string labelMatchSelfDefineSign;

	public int sliderHostKeyId;
	public string sliderMatchSign;
	public string sliderMaxSign;

	// 是否需要支持多语言自动翻译
	public bool isNeedTranslate;
	// 各种界面注册方法
	public string onClickModuleName;
	public string onTweenAniFinishedModuleName;
	public string onSliderModuleName;
	public string scrollerTableDataModuleName;

	// Use this for initialization
	void Start () {
		this.Init ();
	}

	void OnDestory() {
		FomulaHostManager.Instance.RemoveNotifyUiHelper (this._dymUid);
	}

	public void Init() {
		if (this._dymUid != null) {
			return;
		}

		UIRootHelper urh = this.root.GetComponent<UIRootHelper> ();
		this._label = this.gameObject.GetComponent<UILabel> ();
		this._button = this.gameObject.GetComponent<UIButton> ();
		this._tweener = this.gameObject.GetComponent<UITweener> ();
		this._slider = this.gameObject.GetComponent<UISlider> ();
		this._sprite = this.gameObject.GetComponent<UISprite> ();
		this.InitTableItems ();
		if (urh != null && this._button != null) {
			EventDelegate _dlg = new EventDelegate ();
			_dlg.Set (urh, UIRootHelper.ON_CLICK_METHOD);
			_dlg.parameters [0] = new EventDelegate.Parameter ();
			_dlg.parameters [0].obj = this.gameObject;

			if (!this._button.onClick.Contains (_dlg)) {
				this._button.onClick.Add (_dlg);
			}
		}

		if (urh != null && this._tweener != null) {
			EventDelegate _dlg = new EventDelegate ();
			_dlg.Set (urh, UIRootHelper.ON_ANI_FINISHED_METHOD);
			_dlg.parameters [0] = new EventDelegate.Parameter ();
			_dlg.parameters [0].obj = this.gameObject;

			if (!this._tweener.onFinished.Contains (_dlg)) {
				this._tweener.onFinished.Add (_dlg);
			}
		}

		if (urh != null && this._slider != null) {
			EventDelegate _dlg = new EventDelegate ();
			_dlg.Set (urh, UIRootHelper.ON_SLIDER_METHOD);
			_dlg.parameters [0] = new EventDelegate.Parameter ();
			_dlg.parameters [0].obj = this.gameObject;

			if (!this._slider.onChange.Contains (_dlg)) {
				this._slider.onChange.Add (_dlg);
			}
		}

		if (this._dymUid == null) {
			this._dymUid = ObjectId.NewObjectId ();
		}

		bool isControledActive = (this.activeValue != null && this.activeValue != string.Empty);
		if (urh != null && urh.isSingleInstanceUI && (this._label != null || this._slider != null || isControledActive)) {
			FomulaHostManager.Instance.SetNotifyUiHelper (this._dymUid, this);
		}

		if (isControledActive) {
			this.gameObject.SetActive (false);
		}
	}

	/// <summary>
	/// Sets the active host.
	/// 
	/// 设置影响active的host
	/// </summary>
	/// <param name="host">Host.</param>
	public void SetActiveHost(FormulaHost host) {
		if (this.activeValue == null || this.activeValue == string.Empty) {
			return;
		}

		if (host.GetHostType () != this.activeHostKeyId) {
			return;
		}

		//Debug.Log ("SetActiveHost " + host.GetFileName ());
		this.hostActive = host;
	}

	/// <summary>
	/// Sets the active host for table.
	/// 
	/// 对于自带Scroller view的控件设置内部元素的active host
	/// 例如背包里面的多个物品
	/// </summary>
	/// <param name="list">List.</param>
	public void SetActiveHostForTable(List<FormulaHost> list) {
		if (this._scorll == null) {
			return;
		}

		if (list == null || list.Count <= 0) {
			return;
		}

		if (this._tableItems == null || this._tableItems.Count <= 0) {
			return;
		}

		for (int i = 0; i < list.Count; i++) {
			if (i >= this._tableItems.Count) {
				return;
			}

			FormulaHost _host = list [i];
			GameObject _tbObj = this._tableItems [i];
			if (_host == null || _tbObj == null) {
				continue;
			}

			UIRootHelper _urh = _tbObj.GetComponent<UIRootHelper> ();
			if (_urh == null) {
				continue;
			}

			_urh.SetActiveHost (_host);
		}
	}

	/// <summary>
	/// Sets the label host.
	/// 
	/// 设置影响label text的host
	/// </summary>
	/// <param name="host">Host.</param>
	public void SetLabelHost(FormulaHost host) {
		if (host.GetHostType () != this.labelHostKeyId) {
			return;
		}

		//Debug.Log ("SetLabelHost " + host.GetFileName () + " / " + this.labelMatchSign);
		this.hostLabel = host;
	}

	/// <summary>
	/// Sets the level host for table.
	/// 
	/// 对于自带Scroller view的控件设置内部元素的label host
	/// 例如背包里面的多个物品
	/// </summary>
	/// <param name="list">List.</param>
	public void SetLabelHostForTable(List<FormulaHost> list) {
		if (this._scorll == null) {
			return;
		}

		if (list == null || list.Count <= 0) {
			return;
		}

		if (this._tableItems == null || this._tableItems.Count <= 0) {
			return;
		}

		for (int i = 0; i < list.Count; i++) {
			if (i >= this._tableItems.Count) {
				return;
			}

			FormulaHost _host = list [i];
			GameObject _tbObj = this._tableItems [i];
			if (_host == null || _tbObj == null) {
				continue;
			}

			UIRootHelper _urh = _tbObj.GetComponent<UIRootHelper> ();
			if (_urh == null) {
				continue;
			}

			_urh.SetLabelHost (_host);
		}
	}

	public void AutoSetLabelHostForTable() {
		if (this._tTableDataFrom == null) {
			return;
		}

		object datas = this._tTableDataFrom.InvokeMember (GET_DATA_METHOD, UIRootHelper.B_FLAGS, null, null, new object[]{ this.gameObject });
		if (datas == null) {
			return;
		}

		List<FormulaHost> listDatas = (List<FormulaHost>)datas;
		this.SetLabelHostForTable (listDatas);
	}

	/// <summary>
	/// Sets the slider host.
	/// 
	/// 设置影响滑动条、进度条的host
	/// </summary>
	/// <param name="host">Host.</param>
	public void SetSliderHost(FormulaHost host) {
		if (host.GetHostType () != this.sliderHostKeyId) {
			return;
		}

		//Debug.Log ("SetSliderHost " + host.GetFileName () + " / " + this.sliderMatchSign);
		this.hostSlider = host;
	}

	/// <summary>
	/// Sets the slider host for table.
	/// 
	/// 对于自带Scroller view的控件设置内部元素的slider host
	/// 例如背包里面的多个物品
	/// </summary>
	/// <param name="list">List.</param>
	public void SetSliderHostForTable(List<FormulaHost> list) {
		if (this._scorll == null) {
			return;
		}

		if (list == null || list.Count <= 0) {
			return;
		}

		if (this._tableItems == null || this._tableItems.Count <= 0) {
			return;
		}

		for (int i = 0; i < list.Count; i++) {
			if (i >= this._tableItems.Count) {
				return;
			}

			FormulaHost _host = list [i];
			GameObject _tbObj = this._tableItems [i];
			if (_host == null || _tbObj == null) {
				continue;
			}

			UIRootHelper _urh = _tbObj.GetComponent<UIRootHelper> ();
			if (_urh == null) {
				continue;
			}

			_urh.SetSliderHost (_host);
		}
	}

	/// <summary>
	/// Raises the notify dynamic data change event.
	/// 
	/// 被注册在内的host当sign value改变时触发
	/// </summary>
	/// <param name="host">Host.</param>
	/// <param name="key">Key.</param>
	/// <param name="value">Value.</param>
	public void OnNotifyDynamicDataChange(FormulaHost host, string key, object value) {
		if (host == null || key == null || value == null) {
			return;
		}

		if (this.hostActive == null && this.hostLabel == null && this.hostSlider == null) {
			return;
		}

		if (host == this.hostActive) {
			this.__NotifyForActive (key, value);
		}

		if (host == this.hostLabel) {
			this.__NotifyForText (key, value);
		}

		if (host == this.hostSlider) {
			this.__NotifyForSlider (key, value);
		}
	}

	private void __NotifyForSlider(string key, object value) {
		if (this._slider == null) {
			return;
		}

		if (this.hostSlider == null) {
			return;
		}

		if (this.sliderMatchSign == this.sliderMaxSign) {
			return;
		}

		if (key == null || (key != this.sliderMatchSign && key != this.sliderMaxSign)) {
			return;
		}

		float _val = this.hostSlider.GetDynamicDataByKey (this.sliderMatchSign);
		float _valmax = this.hostSlider.GetDynamicDataByKey (this.sliderMaxSign);
		float _rate = _val / _valmax;
		this._slider.value = _rate;
		if (this._sprite == null) {
			return;
		}

		this._sprite.fillAmount = _rate;
	}

	/// <summary>
	/// Condictions the active.
	/// 
	/// host数据引起的active变动
	/// </summary>
	/// <param name="key">Key.</param>
	/// <param name="value">Value.</param>
	private void __NotifyForActive(string key, object value) {
		if (this.activeValue == null || this.activeValue == string.Empty) {
			return;
		}

		if (key != this.activeCondiction) {
			return;
		}

		if (value.ToString () != this.activeValue) {
			this.gameObject.SetActive (false);
			return;
		}

		this.gameObject.SetActive (true);
	}

	/// <summary>
	/// Labels the match.
	/// 
	/// host数据引起的label text变动
	/// 可在此加入动画数字变动的支持
	/// </summary>
	/// <param name="key">Key.</param>
	/// <param name="value">Value.</param>
	private void __NotifyForText(string key, object value) {
		if (this.gameObject == null) {
			return;
		}

		if (this._label == null) {
			return;
		}

		bool isNoConfig = (this.labelHostKeyId == 0 && this.labelMatchSign == SignKeys.LEVEL && (this.labelMatchSelfDefineSign == null || this.labelMatchSelfDefineSign == string.Empty));
		if (isNoConfig) {
			return;
		}

		// 优先处理自定义匹配关键字
		// Debug.Log(key + "\t\t\t" + this.labelMatchSelfDefineSign + "  /// ------ /// " + this.labelMatchSign);
		if (this.labelMatchSelfDefineSign != null && this.labelMatchSelfDefineSign != string.Empty) {
			if (key != this.labelMatchSelfDefineSign) {
				return;
			}
		} else {
			if (key != this.labelMatchSign) {
				return;
			}
		}

		this._label.text = value.ToString ();
	}

	/// <summary>
	/// Inits the table items.
	/// 
	/// 如果该控件包含scroller view，则自动加载其内涵的所有独立小对象，
	/// 例如下拉列表的每个小项目
	/// </summary>
	private void InitTableItems() {
		this._scorll = this.gameObject.GetComponent<UIScrollView> ();
		if (this._scorll == null) {
			return;
		}

		if (this.scrollerTableDataModuleName != null && this.scrollerTableDataModuleName != string.Empty) {
			UIRootHelper urh = this.root.GetComponent<UIRootHelper> ();
			this._tTableDataFrom = urh.GetModule (this.scrollerTableDataModuleName);
		}

		Transform _t = this.gameObject.transform.GetChild (0);
		if (_t == null) {
			return;
		}

		if (_t.transform.childCount <= 0) {
			return;
		}

		this._tableItems = new List<GameObject> ();
		for (int i = 0; i < _t.transform.childCount; i++) {
			Transform _tc = _t.transform.GetChild (i);
			if (_tc == null || _tc.gameObject == null) {
				continue;
			}

			this._tableItems.Add (_tc.gameObject);
		}

		// Try to fill data.
		this.AutoSetLabelHostForTable ();
	}
}
