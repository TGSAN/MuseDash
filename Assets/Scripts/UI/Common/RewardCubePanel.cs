// 宝箱开启的动画

using UnityEngine;
using System.Collections;

public class RewardCubePanel : MonoBehaviour {


	public Show3DCube m_show3DCube;
	public UITexture m_tex;
	public GameObject m_Mask;
	public GameObject m_RewardPanel;
	int index;

	bool show=false;

	void OnEnable()
	{
	//	CommonPanel.GetInstance().SetBlurSub(this.GetComponent<UIPanel>());
	}

	public void Show(int _index,UIPerfabsManage.CallBackFun _CallBack)
	{
		CommonPanel.GetInstance().SetBlurSub(this.GetComponent<UIPanel>());
		m_RewardPanel.SetActive(false);
		this.gameObject.SetActive(true);
		m_tex.gameObject.SetActive(true);
		index=_index;
		m_show3DCube.PlayCubeInAniMation(_index,_CallBack);
		m_Mask.SetActive(true);
		show=true;

	}
	// Use this for initialization
	void Start () {
	
	}
	// Update is called once per frame
	void Update () {
	
	}
	public void ShowUnlocked()
	{
		m_show3DCube.ShowLocked(index);
	}
	public void ClickScreen()
	{
		if(show)
		{
			m_show3DCube.ShowOpenBox();
			show=false;
		}

	}
//	IEnumerator ClostMask()
}
