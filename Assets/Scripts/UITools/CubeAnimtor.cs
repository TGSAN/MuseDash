using UnityEngine;
using System.Collections;

public class CubeAnimtor : MonoBehaviour {



	UIPerfabsManage.CallBackFun m_CallBack1=null;
	UIPerfabsManage.CallBackFun m_CallBack2=null;
	UIPerfabsManage.CallBackFun m_CallBack3=null;

	/// <summary>
	/// 设置开启宝箱的回调函数 两个动画时间点 在动画上
	/// </summary>
	/// <param name="_fun1">Fun1.</param>
	/// <param name="_fun2">Fun2.</param>
	public void SetFUn(UIPerfabsManage.CallBackFun _fun1,UIPerfabsManage.CallBackFun _fun2=null)
	{
		m_CallBack1=_fun1;
		m_CallBack2=_fun2;
	}
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void FunCation1()
	{

		if(m_CallBack1!=null)
		{
			m_CallBack1();

			this.gameObject.SetActive(false);
		}
	}
	public void FunCation2()
	{

		Debug.Log("DO fun2");
		if(m_CallBack2!=null)
		{
			m_CallBack2();
		}
	}
	public void FinishInAni()
	{
		//FunCation2();
	}
}
