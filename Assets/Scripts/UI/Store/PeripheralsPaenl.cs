using UnityEngine;
using System.Collections;

public class PeripheralsPaenl : MonoBehaviour {

	public UISprite m_Sprite;
	string []Names={"store_peripherals_1","store_peripherals_2","store_peripherals_3","store_peripherals_4"};
	int index=0;

	public GameObject RightButton;
	public GameObject LeftButton;
	public void OnEnable()
	{
//		index=0;
//		m_Sprite.spriteName=Names[index];
//		LeftButton.gameObject.SetActive(false);
//		RightButton.gameObject.SetActive(true);
	}
	public void ClickRightButton()
	{

//		if(index+1>3)
//			return;
//		index++;
//		if(index==3)
//		{
//			RightButton.gameObject.SetActive(false);
//		}
//		if(index==1)
//		{
//			LeftButton.gameObject.SetActive(true);
//		}
//		m_Sprite.spriteName=Names[index];

	}
	public void ClickLeftButton()
	{
//		if(index-1<0)
//			return;
//		index--;
//
//		if(index==0)
//		{
//			LeftButton.gameObject.SetActive(false);
//
//		}
//		if(index==2)
//		{
//			RightButton.gameObject.SetActive(true);
//		}
//		m_Sprite.spriteName=Names[index];
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
