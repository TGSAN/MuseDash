using UnityEngine;
using System.Collections;
using FormulaBase;
public class EnSureSalePanel : MonoBehaviour {

	public UILabel m_GetMoney;
	public UILabel m_warningLabel;
	public UILabel m_SaleLabel;
	public Callback m_OKCallBack;
	public Callback m_CancelCallBack;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	/// <summary>
	/// Shows the panel.
	/// </summary>
	/// <param name="Money">显示的钱.</param>
	/// <param name="_OKCallBack">确定的回调.</param>
	/// <param name="_CancelButton">退出的回调.</param>
	/// <param name="_BlueQulity">是否包含高级物品.</param>
	/// <param name="_brul">是否在毛玻璃上面</param>
	public void ShowPanel(int Money,Callback _OKCallBack,Callback _CancelButton,bool _BlueQulity=false,bool  _brul=true)
	{
		
		m_OKCallBack=_OKCallBack;
		m_CancelCallBack=_CancelButton;
		if(Money<0)
		{
			return ;
		}
		m_GetMoney.text=Money.ToString();
		if(_BlueQulity)
		{
			m_warningLabel.text="警告:内含高级稀有物品";
		}
		else 
		{
			m_warningLabel.text="此乃贵重物品骚年请三思!";
		}

		if(ItemManageComponent.Instance.GetChosedItem.Count>2)
		{
			m_SaleLabel.text="出售介个可以得到:";
		}
		else 
		{
			m_SaleLabel.text="此次出售可以得到:";
		}
		//设置数据 进场动画
		CommonPanel.GetInstance().SetPanelBlur(this.gameObject.GetComponent<UIPanel>(),true);
		this.gameObject.SetActive(true);
	}
	/// <summary>
	/// 关闭界面
	/// </summary>
	public void ClosePanel()
	{
		FinishCloseAniMation();
	}
	/// <summary>
	/// 完成关闭界面动画
	/// </summary>
	public void FinishCloseAniMation()
	{
		if(m_CancelCallBack!=null)
		{
			m_CancelCallBack();
		}
	}

	/// <summary>
	/// 一般不调用 直接调用回调函数  不同情况太多  并不是每次都关闭
	/// </summary>
	public void CloseEnSureSalePanel(bool _brue=false)
	{
		if(!this.gameObject.activeSelf)
			return ;
		this.gameObject.SetActive(false);
		if(!_brue)
		{
			CommonPanel.GetInstance().CloseBlur(null);
		}
	
	}
	/// <summary>
	/// 点击否Button
	/// </summary>
	public void ClickNoButton()
	{
		//CommonPanel.GetInstance().CloseBlur(null);
		ClosePanel();
	}
	/// <summary>
	/// 点击确定按钮
	/// </summary>
	public void ClickYesButton()
	{
		this.gameObject.SetActive(false);
		m_OKCallBack();
	}
}
