//EDIT ：SF 游戏初始化类
using FormulaBase;
using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    private static bool once = true;

    // Use this for initialization
    private void OnEnable()
    {
        //	Debug.Log("path:"+Application.persistentDataPath+"Game Init");
        if (once)
        {
            //UI管理器误删
            this.Init();
            once = false;
        }

        if (CommonPanel.GetInstance() != null)
        {
            CommonPanel.GetInstance().ResetMask();
        }

        this.StartCoroutine(this.__Init());
    }

    private void OnDisable()
    {
        CancelInvoke();
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
    private void Update()
    {
        TimeWork.g_Instace.CheckTime();     //时间检测
    }

    // Use this for initialization
    private void Start()
    {
        TimerHostController thc = this.gameObject.GetComponent<TimerHostController>();
        thc.enabled = true;
        // 成功登陆次数计数，顺便可以初始化账号
        AccountManagerComponent.Instance.AddLoginCount(1);
    }

    public void Init()
    {
        #region 角色

        RoleManageComponent.Instance.InitRole();
        //ChoseHeroManager.Get ().InitLoadModle ();

        #endregion 角色

        Debug.Log("初始化");
        FormulaBase.AccountManagerComponent.Instance.Init();
        FormulaBase.AccountPhysicsManagerComponent.Instance.Init();
//        FormulaBase.BagManageComponent.Instance.Init();
        FormulaBase.TaskManager.instance.Init();
    }

    private IEnumerator __Init()
    {
        if (UISceneHelper.Instance == null || UISceneHelper.Instance.widgets == null || UISceneHelper.Instance.widgets.Count <= 0)
        {
            yield return 0;
        }

        // 定时恢复体力
        AccountPhysicsManagerComponent.Instance.AutoPhysicalRecover();

        Debug.Log("Show main sceen ui.");
        // 所有数据 对象准备完毕后才展示ui
        UISceneHelper.Instance.Show();
        CommonPanel.GetInstance().SetMask(false);
    }
}