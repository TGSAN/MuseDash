using UnityEngine;
using System.Collections;
using FormulaBase;
using System.Collections.Generic;
public class ResoursePanel : MonoBehaviour {

	public List<GameObject> m_list=new List<GameObject>();
	// Use this for initialization
	void Start () {
	//	Debug.Log(StoreManageComponent.Instance.GetImageName(4210));
//		for(int i=0;i<m_list.Count;i++)
//		{
		m_list[0].GetComponent<StoreSaleBox>().SetStoreBox(4010);
		m_list[1].GetComponent<StoreSaleBox>().SetStoreBox(4020);
		m_list[2].GetComponent<StoreSaleBox>().SetStoreBox(4110);
		m_list[3].GetComponent<StoreSaleBox>().SetStoreBox(4120);
		m_list[4].GetComponent<StoreSaleBox>().SetStoreBox(4210);
		m_list[5].GetComponent<StoreSaleBox>().SetStoreBox(4220);
		//}

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnEnable()
	{
		m_PlayTween.Play(true);

	}

	public UIPlayTween m_PlayTween;
	public void ClickButton()
	{
		
	}
}
