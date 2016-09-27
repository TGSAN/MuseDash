//using UnityEngine;
//using System.Collections;
//using UnityStandardAssets.ImageEffects;
//public class ShowEquipPanel : MonoBehaviour {
//	public static EquipShowStyel g_EquipShowStyel=EquipShowStyel.NONE;
//
//	public GameObject[] m_arrGameObject=new GameObject[3];
//	public GameObject t_temp;
//	public GameObject m_sprite;
//
//	public CommonPanel m_CommonPanel;
//	public GameObject m_UpGrade;
//	public GameObject m_Evolution;
//	// Use this for initialization
//
//	public GameObject m_MainMenuPanel;
//
//	public  void ShowEquip(string _icon)
//	{
//
//		t_temp.SetActive(true);
//		m_sprite.SetActive(true);
//
//		switch(g_EquipShowStyel)
//		{
//		case EquipShowStyel.EQUIPSHOW_EQUIP:ShowAndHidStyel(0);break;
//		case EquipShowStyel.EQUIPSHOW_DISCHARGE:ShowAndHidStyel(1);break;
//		case EquipShowStyel.EQUIPSHOW_EQUIPSALE:ShowAndHidStyel(2);break;
//		case EquipShowStyel.NONE:;break;
//
//		}
//		m_Evolution.SetActive(false);
//		m_UpGrade.SetActive(false);
//		//m_sprite
//
//	}
//	public void ShowAndHidStyel(int _index)
//	{
//		for(int i=0;i<3;i++)
//		{
//			if(i==_index)
//			{
//				m_arrGameObject[i].SetActive(true);
//			}
//			else
//			{
//				m_arrGameObject[i].SetActive(false);
//			}
//		}
//	}
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
//	public void ClickSellButton()
//	{
//		m_CommonPanel.ShowOkBox();
//	}
//	public void ClickUpgrade()
//	{
//		m_arrGameObject[1].SetActive(false);
//		m_arrGameObject[2].SetActive(false);
//		m_UpGrade.SetActive(true);
//	}
//	public void ClickEvolution()
//	{
//		m_arrGameObject[1].SetActive(false);
//		m_arrGameObject[2].SetActive(false);
//		m_sprite.SetActive(false);
//		m_arrGameObject[2].SetActive(false);
//		m_Evolution.SetActive(true);
//	}
//	public void ClickDisCharge()
//	{
//		m_CommonPanel.ShowOkBox();
//	}
//	public void ClickEquipButton()
//	{
//		m_CommonPanel.ShowOkBox("Equiped Success",CloseEquipShow);
//	}
//
//	public void CloseEquipShow()
//	{
//		//CharactorPanel.m_EquipCellState=EquipCellState.EQUIP_BAG;
//		this.gameObject.SetActive(false);
//
//	}
//	public void UpGradeYes()
//	{
//		m_CommonPanel.ShowOkBox();
//	}
//	public void UpGradeNo()
//	{
//		m_CommonPanel.ShowOkBox();
//	}
//	public void EvolutionYes()
//	{
//		m_CommonPanel.ShowOkBox();
//	}
//	public void EvolutionNo()
//	{
//		m_CommonPanel.ShowOkBox();
//	}
//	public void AddItemButton()
//	{
//		m_CommonPanel.ShowOkBox();
//	}
//
//	public BlurOptimized m_Temp;
//	public GameObject CommonPanel;
//	public void OnEnable()
//	{
//		Debug.LogWarning("zhege niao wanyi"+this.gameObject.name);
//		Debug.LogWarning("zhege niao wanyi"+this.gameObject.name);
//		Debug.LogWarning("zhege niao wanyi"+this.gameObject.name);
//		Debug.LogWarning("zhege niao wanyi"+this.gameObject.name);
//		Debug.LogWarning("zhege niao wanyi"+this.gameObject.name);
//		Time.timeScale=1f;
//		//Debug.Log("kai shi mao bo li");
//		m_MainMenuPanel.layer=10;
//		this.gameObject.layer=10;
//		m_Temp.enabled=true;
//
//		CommonPanel.layer=10;
//
//		t_blurtime=Time.time;
//		InvokeRepeating("ToBlur",0.01f,0.1f);
//
//	}
//	float t_blurtime=0;
//	public float DurTime=1.2f;
//	public void ToBlur()
//	{
//		//t_blurtime+=Time.deltaTime;
//
//		m_Temp.blurSize=Mathf.Lerp(0,2.9f,(Time.time-t_blurtime)/DurTime);
//	//	m_Temp.blurIterations=(int)Mathf.Lerp(0,2,(Time.time-t_blurtime)/2);
//		if((Time.time-t_blurtime)>DurTime)
//		{
//			CancelInvoke("ToBlur");
//		}
//
//	}
//	public void AwayBlur()
//	{
//
//		m_Temp.blurSize=Mathf.Lerp(5,0f,(Time.time-t_blurtime)/DurTime);
//		//	m_Temp.blurIterations=(int)Mathf.Lerp(0,2,(Time.time-t_blurtime)/2);
//		if((Time.time-t_blurtime)>DurTime)
//		{
//			m_Temp.enabled=false;
//			CancelInvoke("AwayBlur");
//		}
////		m_Temp.blurSize;
////		m_Temp.blurIterations;
//	}
//	public void OnDisable()
//	{
//		t_blurtime=Time.time;
//		InvokeRepeating("AwayBlur",0.01f,0.1f);
//		//CommonPanel.layer=10;
//		//Debug.Log("guan bi mao bo li");
//	}
//}
