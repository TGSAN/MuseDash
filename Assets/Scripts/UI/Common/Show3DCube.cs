using UnityEngine;
using System.Collections;

public class Show3DCube : MonoBehaviour {



	public GameObject Cube1=null;
	public GameObject Cube2=null;
	public GameObject Cube3=null;
	public GameObject Cube4=null;

//	public void SetCubeOpenFun(UIPerfabsManage.CallBackFun _CallBack)
//	{
//		
//		HideAll();
//	}
//
	int Quality=0;

	/// <summary>
	/// 显示宝箱开启动画
	/// </summary>
	public void ShowOpenBox()
	{
		HideAll();
		switch(Quality)
		{
		case 1:
			Cube1.SetActive(true);
			Cube1.GetComponent<Animator>().Play("cube_1_open");

			break;
		case 2:
			Cube2.SetActive(true);
			Cube2.GetComponent<Animator>().Play("cube_2_open");
		
			break;
		case 3:
			Cube3.SetActive(true);
			Cube3.GetComponent<Animator>().Play("cube_3_open");
		
			break;
		case 4:
			Cube4.SetActive(true);
			Cube4.GetComponent<Animator>().Play("cube_4_open");

			break;
		}
	}

	/// <summary>
	/// 显示宝箱的进场动画
	/// </summary>
	/// <param name="_index">Index.</param>
	/// <param name="_CallBack">Call back.</param>
	public void PlayCubeInAniMation(int _index,UIPerfabsManage.CallBackFun _CallBack)
	{
		
		Debug.Log("PlayAni");
		HideAll();
		//Cube1.GetComponent<Animator>().Play()
		switch(_index)
		{
		case 1:
			Cube1.SetActive(true);
			Cube1.GetComponent<Animator>().Play("cube_1_in");
			Cube1.GetComponent<CubeAnimtor>().SetFUn(_CallBack,temFun);
			break;
		case 2:
			Cube2.SetActive(true);
			Cube2.GetComponent<Animator>().Play("cube_2_in");
			Cube2.GetComponent<CubeAnimtor>().SetFUn(_CallBack,temFun);
			break;
		case 3:
			Cube3.SetActive(true);

			Cube3.GetComponent<Animator>().Play("cube_3_in");
			Cube3.GetComponent<CubeAnimtor>().SetFUn(_CallBack,temFun);
			break;
		case 4:
			Cube4.SetActive(true);
			Cube4.GetComponent<Animator>().Play("cube_4_in");
			Cube4.GetComponent<CubeAnimtor>().SetFUn(_CallBack,temFun);
			break;
		}
		Quality=_index;
	}
	/// <summary>
	/// 宝箱进场动画播放完成（开始播放宝箱抖动动画）
	/// </summary>
	void temFun()
	{
		Debug.Log("Shaker");
		ShowLocked(Quality);
	}
	/// <summary>
	/// 显示宝箱的待机状态
	/// </summary>
	/// <param name="_index">Index.</param>
	public void ShowIdel(int _index)
	{
		HideAll();
		switch(_index)
		{
		case 1:
			Cube1.SetActive(true);
			Cube1.GetComponent<Animator>().Play("cube_1_stand");
			break;
		case 2:
			Cube2.SetActive(true);
			Cube2.GetComponent<Animator>().Play("cube_2_stand");
			break;
		case 3:
			Cube3.SetActive(true);
			Cube3.GetComponent<Animator>().Play("cube_3_stand");
			break;
		case 4:
			Cube4.SetActive(true);
			Cube4.GetComponent<Animator>().Play("cube_4_stand");

			break;
		}
	}
	/// <summary>
	/// 显示宝箱的正在解锁状态（抖动）
	/// </summary>
	/// <param name="_index">Index.</param>
	public void ShowLocked(int _index)
	{
		HideAll();
		switch(_index)
		{
		case 1:
			Cube1.SetActive(true);
			Cube1.GetComponent<Animator>().Play("cube_1_unlocked");
			break;
		case 2:
			Cube2.SetActive(true);
			Cube2.GetComponent<Animator>().Play("cube_2_unlocked");
			break;
		case 3:
			Cube3.SetActive(true);
			Cube3.GetComponent<Animator>().Play("cube_3_unlocked");
			break;
		case 4:
			Cube4.SetActive(true);
			Cube4.GetComponent<Animator>().Play("cube_4_unlocked");

			break;
		case -1:
			HideAll();

			break;
		}
	}
	public void HideAll()
	{
		Cube1.SetActive(false);
		Cube2.SetActive(false);	
		Cube3.SetActive(false);	
		Cube4.SetActive(false);
	}
	/// <summary>
	/// 显示装备的正在解锁动画(旋转)
	/// </summary>
	/// <param name="_index">Index.</param>
	public void ShowCube(int _index)//正在解锁
	{
		HideAll();

		switch(_index)
		{
		case 1:
			Cube1.SetActive(true);
			Cube1.GetComponent<Animator>().Play("cube_1_unlocking");
			break;
		case 2:
			Cube2.SetActive(true);
			Cube2.GetComponent<Animator>().Play("cube_2_unlocking");
			break;
		case 3:
			Cube3.SetActive(true);
			Cube3.GetComponent<Animator>().Play("cube_3_unlocking");
			break;
		case 4:
			Cube4.SetActive(true);
			Cube4.GetComponent<Animator>().Play("cube_4_unlocking");

			break;
		case -1:
			HideAll();

			break;
		}
	}

	public void CLickCUbe()
	{
		HideAll();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

//	void PlayBoom()
//	{
//		
//		m_Boom.Play();
//	}

//	void PlayAttractive()
//	{
//		m_Attractive.Play();
//	}
}
