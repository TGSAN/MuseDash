//瞎写的时间类  自己能瞎玩了 
//先同意忽略时间 有时间再补
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// 3函数 添加 删除 更新		简单粗暴  忽略时间
///	
/// </summary>
public class TimeWork : System.Object {

	static TimeWork m_Instance=null;

	public static TimeWork g_Instace
	{
		get
		{
			if(m_Instance==null)
				m_Instance=new TimeWork();
			return m_Instance;
		}
	}

	TimeWork()
	{
		//SubRealTime
	}

	public class TimeStract
	{
		public float RepeatTime;									//重复时间间隔
		public float FinishTime;									//总时间
		public UIPerfabsManage.CallBackFun m_RepeatFun=null;		//重复函数
		public UIPerfabsManage.CallBackFun m_FinishFun=null;		//完结函数
		public float BeginTime;										//开始的时间
		public bool IgnoreTimeScal=true;							//是否忽略时间缩放
	}
	public Dictionary<string,TimeStract> m_dicTime=new Dictionary<string, TimeStract>();

	public void AddTime(string _key,float _RepeatTime,float _allTime,UIPerfabsManage.CallBackFun _RepeatFun=null,UIPerfabsManage.CallBackFun _FinishFun=null,bool _IgnoreTimeScal=true)
	{

		TimeStract temp=new TimeStract();
		temp.FinishTime=_allTime;
		temp.RepeatTime=_RepeatTime;
		temp.IgnoreTimeScal=_IgnoreTimeScal;
		temp.m_RepeatFun=_RepeatFun;
		temp.m_FinishFun=_FinishFun;
		temp.BeginTime=RealTime.time;
		m_dicTime[_key]=temp;
	}

	public void KillTime(string _key)
	{
		if(m_dicTime.ContainsKey(_key))
		{
			m_dicTime.Remove(_key);
		}
		else 
		{
			Debug.LogWarning("Kill the wrong Time!!!");
		}
	}

	public void CheckTime()
	{
		if(m_dicTime.Count==0)
			return ;
		TimeStract[] tempList=new TimeStract[m_dicTime.Count];
//		if(tempList.Count!=0)
//			tempList.Clear();
		//[] abRefs = new AssetBundleRef[dictAssetBundleRefs.Count];
		int i=0;
		foreach(KeyValuePair<string , TimeStract> temp in m_dicTime)
		{

			m_dicTime.TryGetValue(temp.Key, out tempList[i]);
			i++;
		}
		for(int j=0;j<tempList.Length;j++)
		{
			float PasTime=RealTime.time-tempList[j].BeginTime;		//经过时间

			if(PasTime>=tempList[j].FinishTime&&tempList[j].m_FinishFun!=null)
			{
				tempList[j].m_FinishFun();
				continue;
			}
			if(PasTime>tempList[j].RepeatTime)
			{
				tempList[j].m_RepeatFun();
				tempList[j].RepeatTime+=tempList[j].RepeatTime;
			}
		}
	}
}
