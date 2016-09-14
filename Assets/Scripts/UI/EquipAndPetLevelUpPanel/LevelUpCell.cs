using UnityEngine;
using System.Collections;
using FormulaBase;
public class LevelUpCell : MonoBehaviour {

	FormulaHost m_Host;
	public GameObject[] m_ArrQuality=new GameObject[4];
	public UISprite m_ItemIcon;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// 设置品质
	/// </summary>
	/// <param name="_Quality">Quality.</param>
	void SetQuality(int _Quality)
	{
		ShowParticle(_Quality);
	}

	/// <summary>
	/// 设置粒子
	/// </summary>
	/// <param name="_Quality">Quality.</param>
	public void ShowParticle(int _Quality)
	{
		for(int i=0,max=m_ArrQuality.Length;i<max;i++)
		{
			if(_Quality==i)
			{
				m_ArrQuality[i].SetActive(true);
			}
			else 
			{
				m_ArrQuality[i].SetActive(false);
			}
		}
	}
	public void SetData(FormulaHost _host)
	{
		m_Host=_host;
		m_ItemIcon.spriteName=m_Host.GetDynamicIntByKey("ID").ToString();
		string type=m_Host.GetFileName();

		//m_Scale.PlayForward();
		switch(type)
		{
		case "Equip":	m_Host.Result(FormulaKeys.FORMULA_19)	;break;
		case "Pet":	m_Host.Result(FormulaKeys.FORMULA_91)	;break;
		}
		SetQuality(m_Host.GetDynamicIntByKey(SignKeys.QUALITY));
	}
}
