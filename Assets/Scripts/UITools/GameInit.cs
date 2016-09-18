//EDIT ：SF 游戏初始化类
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FormulaBase;
using GameLogic;

public class GameInit : MonoBehaviour {
	static bool once=true ;
	// Use this for initialization
	void OnEnable () {
		//	Debug.Log("path:"+Application.persistentDataPath+"Game Init");
		if (once) {
			//UI管理器误删
			this.Init ();
			once = false;
		}

		CommonPanel.GetInstance ().SetMask (false);
	}

	void OnDisable()
	{
		CancelInvoke ();
	}
	
//	IEnumerable OnApplicationQuit()
//	{
//	
//	//	PlayerAccountData temp
//		//SaveBuildingData(AccountData.g_Instan.m_PlayerAccountData);
//		yield return  new WaitForSeconds(1f);
//		Debug.Log("SaveData");
//		Application.Quit();
//	}

	// Update is called once per frame
	void Update () {
		TimeWork.g_Instace.CheckTime ();		//时间检测
	}



	// Use this for initialization
	void Start () {
		TimerHostController thc = this.gameObject.GetComponent<TimerHostController> ();
		thc.enabled = true;
		// 成功登陆次数计数，顺便可以初始化账号
		AccountManagerComponent.Instance.AddLoginCount (1);
		// 定时恢复体力
		AccountPhysicsManagerComponent.Instance.AutoPhysicalRecover ();
	}

	public void Init() {
		#region 角色
		RoleManageComponent.Instance.InitRole ();
		//ChoseHeroManager.Get ().InitLoadModle ();
		#endregion
		Debug.Log ("初始化");
		FormulaBase.AccountManagerComponent.Instance.Init ();
		FormulaBase.AccountPhysicsManagerComponent.Instance.Init ();
		FormulaBase.BagManageComponent.Instance.Init ();
		FormulaBase.EquipManageComponent.Instance.Init ();//初始化装s备
		FormulaBase.materialManageComponent.Instance.Init ();	//初始化材料
		FormulaBase.PetManageComponent.Instance.Init ();//初始化宠物
		FormulaBase.ItemManageComponent.Instance.Init ();//背包初始化 放在所有道具之后
	}
}