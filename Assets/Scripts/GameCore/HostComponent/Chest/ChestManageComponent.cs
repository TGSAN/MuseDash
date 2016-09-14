 ///自定义模块，可定制模块具体行为
/// 包含存放到背包中的箱子和在开启栏的箱子
using System;

using System.Collections.Generic;
using System.Collections;


namespace FormulaBase {
	public class ChestManageComponent : CustomComponentBase {
		private static ChestManageComponent instance = null;
		private const int HOST_IDX = 9;
		public static ChestManageComponent Instance {
			get {
				if(instance == null) {
					instance = new ChestManageComponent();
				}
			return instance;
			}
		}
		List<FormulaHost> m_LsitEquipedChest=new List<FormulaHost>();//开启栏的箱子
		public List<FormulaHost> GetChestList
		{
			get{
				return m_LsitEquipedChest;
			}
			set 
			{
				m_LsitEquipedChest=value;
			}
		}
		List<FormulaHost> m_ListTimeDownChest=new List<FormulaHost>();//完成的箱子
		public List<FormulaHost> GetTimeDownChest
		{
			get
			{
				return m_ListTimeDownChest;
			}
			set
			{
				m_ListTimeDownChest=value;
			}
		}
		public void ReomoveChest(FormulaHost _host)
		{

			int Type=(int)_host.GetDynamicDataByKey(SignKeys.CHESTQUEUE);
			if(Type==0)//判断是不是在开启栏的宝箱
			{
				ItemManageComponent.Instance.GetChestList.Remove(_host);			  //删除本地数据
				_host.Delete(new HttpResponseDelegate(UseChestCallBack));
				CommonPanel.GetInstance().ShowWaittingPanel();
			}
			else 
			{
				OpenChestInGrid(_host);
			}
		}
		public void UseChestCallBack(bool _success)
		{
			if(_success)
			{
				CommonPanel.GetInstance().ShowWaittingPanel(false);
				Messenger.Broadcast(LevelPrepaerPanel.BraodCast_RestChestEmpty);
				Messenger.Broadcast(bagPanel2.BroadcastBagItem);
			}
			else 
			{
				CommonPanel.GetInstance().ShowText("Conncet fail");
				NGUIDebug.Log("Conncet fail");
			}
		//	GetChestList[1].SetRealTimeCountDown(200);
			//GetChestList[1].tim;
		}
		/// <summary>
		/// 添加开启的箱子
		/// </summary>
		public void AddOpenningChest(FormulaHost _host)
		{
			m_LsitEquipedChest.Add(_host);
		}
		/// <summary>
		/// 获取拥有的箱子个数
		/// </summary>
		public int GetOwnedChestNumber()
		{
			return GetChestList.Count+GetTimeDownChest.Count;
		}
		/// <summary>
		/// 获取宝箱开启的所有时间
		/// </summary>
		/// <returns>The chest all time.</returns>
		/// <param name="_host">Host.</param>
		public string GetChestAllTime(FormulaHost _host)
		{
			int time=0;
			if(_host.GetDynamicIntByKey(SignKeys.CHESTQUEUE)==1)
			{
				time=_host.GetRealTimeCountDownNow();
				if(time<=0)
					time=0;
			}
			else 
			{
				time=(int)_host.GetDynamicIntByKey(SignKeys.CHESTREMAINING_TIME);
			}
			return IntTimetoStirng(time);
		}
		public string GetBagCellTime(FormulaHost _host)
		{
			int alltime=(int)_host.Result(FormulaKeys.FORMULA_94);
			int h=alltime/3600;
			int m=alltime%3600/60;
			int s=alltime%3600%60;
			if(h!=0)
			{
				return string.Format("{0}小时",h);
			}
			else if(m!=0)
			{
				return string.Format("{0}分",m);
			}
			else  if(s!=0)
			{
				return string.Format("{0}秒",s);
			}
			return "";
		}
		/// <summary>
		/// 整形转字符串
		/// </summary>
		/// <returns>The timeto stirng.</returns>
		/// <param name="_time">Time.</param>
		string IntTimetoStirng(int _time)
		{
			if(_time<=0)
			{
				return "";
			}
			int h=_time/3600;
			int m=_time%3600/60;
			int s=_time%3600%60;
			if(h>0)
			{
				return string.Format("{0}小时{1}分",h,m);
			}
			else if(m>0)
			{
				return string.Format("{0}分{1}秒",m,s);
			}
			else if(s>0)
			{
				return string.Format("{0}秒",s);
			}
			else 
			{
				return "";
			}
		}
		public string GetAllChestTime()
		{

			int allTime=0;
			for(int i=0,max=m_LsitEquipedChest.Count;i<max;i++)
			{
				if(m_LsitEquipedChest[i].GetDynamicIntByKey(SignKeys.CHESTQUEUE)==1)
				{
					allTime+=m_LsitEquipedChest[i].GetRealTimeCountDownNow();
				}
				else 
				{
					allTime+=m_LsitEquipedChest[i].GetDynamicIntByKey(SignKeys.CHESTREMAINING_TIME);
				}
			}
			return IntTimetoStirng(allTime);
		}
//		/// <summary>
//		/// 队列箱子完成
//		/// </summary>
//		/// <param name="_host">Host.</param>
//		public void QueueFinish(FormulaHost _host)
//		{
//
//			for(int i=0,max=m_LsitEquipedChest.Count;i<max;i++)
//			{
//				if(m_LsitEquipedChest[i].GetDynamicIntByKey(SignKeys.CHESTQUEUE)==2)
//				{
//					m_LsitEquipedChest[i].SetDynamicData(SignKeys.CHESTREMAINING_TIME,(int)m_LsitEquipedChest[i].Result(FormulaKeys.FORMULA_94));
//					m_LsitEquipedChest[i].SetRealTimeCountDown((int)m_LsitEquipedChest[i].Result(FormulaKeys.FORMULA_94));
//				//	Debugger.Log("Time-->>>>>>>>>>>>>>>>>>>",_host.GetRealTimeCountDownNow());
//				}
//				m_LsitEquipedChest[i].SetDynamicData(SignKeys.CHESTQUEUE,m_LsitEquipedChest[i].GetDynamicIntByKey(SignKeys.CHESTQUEUE)-1);
//			}
//			_host.SetDynamicData(SignKeys.CHESTQUEUE,m_LsitEquipedChest.Count);
//			_host.SetDynamicData(SignKeys.CHESTREMAINING_TIME,0);
//
//			//FormulaHost temp=null;
//
//			FormulaHost.SaveList(m_LsitEquipedChest,new HttpEndResponseDelegate(QueueFinishCallBack));
//			CommonPanel.GetInstance().ShowWaittingPanel(true);
//		}
		public FormulaHost GetQueue1()
		{
			for(int i=0,max=m_LsitEquipedChest.Count;i<max;i++)
			{
				if(m_LsitEquipedChest[i].GetDynamicIntByKey(SignKeys.CHESTQUEUE)==1)
					return m_LsitEquipedChest[i];
			}
			//Debugger.LogError("放回错误的Host");
			return null;
		}
		public FormulaHost GetTaggetQueue(int _Queue)
		{
			for(int i=0,max=m_LsitEquipedChest.Count;i<max;i++)
			{
				if(m_LsitEquipedChest[i].GetDynamicIntByKey(SignKeys.CHESTQUEUE)==_Queue)
					return m_LsitEquipedChest[i];
			}
			//Debugger.LogError("放回错误的Host");
			return null;
			
		}

//		public void QueueFinishCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response)
//		{
//			CommonPanel.GetInstance().ShowWaittingPanel(false);
//			Messenger.Broadcast(LevelPrepaerPanel.BraodCast_RestChestEmpty);
//			Messenger.Broadcast(bagPanel2.BroadcastBagItem);
//			Debugger.Log("交换队列的反馈");
//		}
		/// <summary>
		/// 获取剩余时间的百分比 用与剩余时间的显示
		/// </summary>
		/// <returns>The time percentage.</returns>
		/// <param name="_host">Host.</param>
		public float GetTimePercentage(FormulaHost _host)
		{
			int RemindTime=(int)_host.GetDynamicDataByKey(SignKeys.CHESTREMAINING_TIME);
			int AllTime= (int)_host.Result(FormulaKeys.FORMULA_94);
			float temp=(RemindTime*1f)/AllTime;
			return temp;
		}
		/// <summary>
		/// 获取开启宝箱需要的钻石
		/// </summary>
		/// <returns>The open chest money.</returns>
		public int GetOpenChestMoney(FormulaHost _host)
		{//
			int  _Retime=0;
			if(_host.GetDynamicIntByKey(SignKeys.CHESTQUEUE)==1)
			{
				_Retime=_host.GetRealTimeCountDownNow();
			}
			else 
			{
				_Retime=(int)_host.Result(FormulaKeys.FORMULA_94);
			}
			int timecut=(int)SundryManageComponent.Instance.GetVaule(11);
			int Money=_Retime/(timecut*60);
			if(_Retime%(timecut*60)!=0)
			{
				Money++;
			}
			return Money;
		}
		public  FormulaHost CreateItem(int idx) {
			FormulaHost host = FomulaHostManager.Instance.CreateHost (HOST_IDX);
			if (host != null) {
				host.SetDynamicData("ID",idx);
				host.SetDynamicData(SignKeys.CHESTREMAINING_TIME,(int)host.Result(FormulaKeys.FORMULA_94));
				ItemManageComponent.Instance.AddItem(host);
			}
			return host;
		}
		public void CreateItem(List<int> _listIndex)
		{
			List<FormulaHost> TempListItem=new List<FormulaHost>();
			for(int i=0;i<_listIndex.Count;i++)
			{
				
				FormulaHost host = FomulaHostManager.Instance.CreateHost (HostKeys.HOST_9);
				if (host != null) {
					host.SetDynamicData("ID",_listIndex[i]);
					host.SetDynamicData(SignKeys.CHESTREMAINING_TIME,(int)host.Result(FormulaKeys.FORMULA_94));
				}
				TempListItem.Add(host);
			}
			ItemManageComponent.Instance.AddItemList(TempListItem);
		}
		/// <summary>
		/// 获取最新的箱子
		/// </summary>
		/// <returns>The latest chest.</returns>
		public FormulaHost GetLatestChest()
		{
			FormulaHost temp=null;
			for(int i=0,max=ItemManageComponent.Instance.GetChestList.Count;i<max;i++)
			{
				if(i==0)
				{
					temp=ItemManageComponent.Instance.GetChestList[i];
				}
				else 
				{
					if(ItemManageComponent.Instance.GetChestList[i].GetDynamicIntByKey(SignKeys.BAGINID)<temp.GetDynamicIntByKey(SignKeys.BAGINID))
					{
						temp=ItemManageComponent.Instance.GetChestList[i];
					}
				}
			}
			return temp;
		}

		/// <summary>
		/// 背包到栏位
		/// </summary>
		public bool  ChestBagToGrid()
		{

			int MaxGrid=AccountManagerComponent.Instance.GetChestGirdNumber();			//玩家宝箱栏位
			int ChestInGrid=GetOwnedChestNumber();		

			//栏位上的箱子
			if(ChestInGrid>=MaxGrid||ItemManageComponent.Instance.GetChestList.Count==0)//栏位满 背包没宝箱
			{
				Debugger.Log("不添加宝箱");
				return false ;
			}
			FormulaHost temp=GetLatestChest();
			if(temp==null)
			{
				Debugger.Log("不添加宝箱");
				return false;
			}
				
			temp.SetDynamicData(SignKeys.CHESTQUEUE,GetChestList.Count+1);
			if(GetChestList.Count==0)
			{
				temp.SetRealTimeCountDown((int)temp.Result(FormulaKeys.FORMULA_94));
			}
			Messenger.Broadcast<int>(LevelPrepaerPanel.BraodCast_ChestMissAni,10);
			ItemManageComponent.Instance.GetChestList.Remove(temp);
			GetChestList.Add(temp);
		
			temp.Save(new HttpResponseDelegate(ChestBagTiGridCallBack));
			CommonPanel.GetInstance().ShowWaittingPanel();
			return true;
		}
		public void ChestBagTiGridCallBack(bool _success)
		{
			if(_success)
			{
				CommonPanel.GetInstance().ShowWaittingPanel(false);
				//Messenger.Broadcast(LevelPrepaerPanel.BraodCast_RestChestEmpty);
			}
			else 
			{
				CommonPanel.GetInstance().ShowText("Connect is fail");
			}
			
		}
		public bool MoveFinishedToEnd(FormulaHost _host)
		{
			Debugger.Log("移动箱子到最后");
			int MaxGrid=AccountManagerComponent.Instance.GetChestGirdNumber();			//玩家宝箱栏位
			int ChestInGrid=GetOwnedChestNumber();										//栏位上的箱子
			if(MaxGrid==ChestInGrid&&m_LsitEquipedChest.Count==0)			            //不移动 宝箱栏位为宝箱个数
			{
				return false;
			}
			Messenger.Broadcast<int>(LevelPrepaerPanel.BraodCast_ChestMissAni,10);
			List<FormulaHost> tempList=new List<FormulaHost>();

			for(int i=0,max=GetChestList.Count;i<max;i++)
			{
				int oldQueue=GetChestList[i].GetDynamicIntByKey(SignKeys.CHESTQUEUE);
				GetChestList[i].SetDynamicData(SignKeys.CHESTQUEUE,oldQueue-1);
				if(oldQueue==2)
				{
					GetChestList[i].SetRealTimeCountDown((int)GetChestList[i].Result(FormulaKeys.FORMULA_94));

				}
				tempList.Add(GetChestList[i]);
			}

			_host.SetDynamicData(SignKeys.CHESTQUEUE,MaxGrid-GetTimeDownChest.Count);
			_host.SetDynamicData(SignKeys.CHESTREMAINING_TIME,0);

			GetChestList.Remove(_host);
			GetTimeDownChest.Add(_host);

//			//+++++++++++
//			Messenger.Broadcast<int>(LevelPrepaerPanel.BraodCast_ChestMissAni,10);
			FormulaHost.SaveList(tempList,MoveFinishedToEndCallBack);
		//	_host.Save(new HttpResponseDelegate(MoveFinishedToEndCallBack));
			CommonPanel.GetInstance().ShowWaittingPanel();
			return true;
		}
		public void MoveFinishedToEndCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response)
		{
			CommonPanel.GetInstance().ShowWaittingPanel(false);
			//Messenger.Broadcast(LevelPrepaerPanel.BraodCast_RestChestEmpty);
			//Messenger.Broadcast(LevelPrepaerPanel.BraodCast_ChestAppearAni);
		}
		/// <summary>
		/// 点击开启特定的宝箱
		/// </summary>
		public void OpenChestInGrid(FormulaHost _host)
		{
			CommonPanel.GetInstance().ShowWaittingPanel();
			int ChestQueue=_host.GetDynamicIntByKey(SignKeys.CHESTQUEUE);

			if(_host.GetDynamicIntByKey(SignKeys.CHESTREMAINING_TIME)==0)
			{
				Debugger.Log("打开宝箱栏上时间到的宝箱");
				for(int i=0,max=GetTimeDownChest.Count;i<max;i++)
				{
					int iChestQueue=GetTimeDownChest[i].GetDynamicIntByKey(SignKeys.CHESTQUEUE);
					if(iChestQueue<ChestQueue)
					{
						GetTimeDownChest[i].SetDynamicData(SignKeys.CHESTQUEUE,iChestQueue+1);
					}
		
				}

				Messenger.Broadcast<int>(LevelPrepaerPanel.BraodCast_ChestMissAni,10);
				GetTimeDownChest.Remove(_host);
//				////++++++++
//				Messenger.Broadcast<int>(LevelPrepaerPanel.BraodCast_ChestMissAni,10);
				ChestManageComponent.Instance.ChestBagToGrid();
				if(GetTimeDownChest.Count==0)
				{
					_host.Delete(new HttpResponseDelegate(DeleteOneChestInGrid));
				}
				else 
				{
					_host.Delete();
					FormulaHost.SaveList(GetTimeDownChest,new HttpEndResponseDelegate(OpenChestCallBack));
				}

			}
			else 
			{
				
//				Debugger.Log("打开宝箱栏上时间未到的宝箱");
				for(int i=0,max=GetChestList.Count;i<max;i++)
				{
					int iChestQueue=GetChestList[i].GetDynamicIntByKey(SignKeys.CHESTQUEUE);
					if(iChestQueue>ChestQueue)
					{
						GetChestList[i].SetDynamicData(SignKeys.CHESTQUEUE,iChestQueue-1);
					}
					if(iChestQueue==2)
					{
						GetChestList[i].SetRealTimeCountDown((int)GetChestList[i].Result(FormulaKeys.FORMULA_94));
					}
				}
				Messenger.Broadcast<int>(LevelPrepaerPanel.BraodCast_ChestMissAni,10);
				GetChestList.Remove(_host);
				////++++++++

				ChestManageComponent.Instance.ChestBagToGrid();
				if(GetChestList.Count==0)
				{
					_host.Delete(new HttpResponseDelegate(DeleteOneChestInGrid));
				}
				else 
				{
					_host.Delete();
					FormulaHost.SaveList(GetChestList,new HttpEndResponseDelegate(OpenChestCallBack));
				}

			}
		}

		/// <summary>
		/// 宝箱栏位只有一个宝箱的情况
		/// </summary>
		public void DeleteOneChestInGrid(bool _success)
		{
			if(_success)
			{
				CommonPanel.GetInstance().ShowWaittingPanel(false);
			//	ChestManageComponent.Instance.ChestBagToGrid();
				//Messenger.Broadcast<int>(LevelPrepaerPanel.BraodCast_ChestMissAni,10);
			//	Messenger.Broadcast(LevelPrepaerPanel.BraodCast_RestChestEmpty);
			}
			else 
			{
				CommonPanel.GetInstance().ShowText("Connect is fail");
			}
			
		}
		public void OpenChestCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response)
		{
			CommonPanel.GetInstance().ShowWaittingPanel(false);
			//ChestManageComponent.Instance.ChestBagToGrid();
		
		//	Messenger.Broadcast(LevelPrepaerPanel.BraodCast_RestChestEmpty);
		}

		/// <summary>
		/// 改变宝箱的位置
		/// </summary>
		public void AddGridMoveChest()
		{
			if(GetTimeDownChest.Count==0)
				return ;
			for(int i=0,max=GetTimeDownChest.Count;i<max;i++)
			{
				int oldQueue=GetTimeDownChest[i].GetDynamicIntByKey(SignKeys.CHESTQUEUE);
				GetTimeDownChest[i].SetDynamicData(SignKeys.CHESTQUEUE,oldQueue+1);
			}

			FormulaHost.SaveList(GetTimeDownChest,new HttpEndResponseDelegate(AddGridMoveChestCallBack));


			CommonPanel.GetInstance().ShowWaittingPanel();

		}
		public void AddGridMoveChestCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response)
		{
			CommonPanel.GetInstance().ShowWaittingPanel(false);
	//		Messenger.Broadcast(LevelPrepaerPanel.BraodCast_RestChestEmpty);

		}

		/// <summary>
		/// 改变宝箱的位置
		/// </summary>
		public void SaleChestInGird(FormulaHost _host)
		{
			CommonPanel.GetInstance().ShowWaittingPanel();
			int ChestQueue=_host.GetDynamicIntByKey(SignKeys.CHESTQUEUE);
			Debugger.Log("-------------------------------->>>>加钱(宝箱卖出的钱)");
//			if(_host.GetDynamicIntByKey(SignKeys.CHESTREMAINING_TIME)==0)
//			{
//				//Debugger.Log("出售宝箱栏上时间到的宝箱");
//				for(int i=0,max=GetTimeDownChest.Count;i<max;i++)
//				{
//					int iChestQueue=GetTimeDownChest[i].GetDynamicIntByKey(SignKeys.CHESTQUEUE);
//					if(iChestQueue<ChestQueue)
//					{
//						GetTimeDownChest[i].SetDynamicData(SignKeys.CHESTQUEUE,iChestQueue+1);
//					}
//
//				}
//				GetTimeDownChest.Remove(_host);
//				if(GetTimeDownChest.Count==0)
//				{
//					_host.Delete(new HttpResponseDelegate(DeleteOneChestInGrid));
//				}
//				else 
//				{
//					_host.Delete();
//					FormulaHost.SaveList(GetTimeDownChest,new HttpEndResponseDelegate(OpenChestCallBack));
//				}
//
//			}
//			else 
//			{
				//				Debugger.Log("出售宝箱栏上时间未到的宝箱");
				for(int i=0,max=GetChestList.Count;i<max;i++)
				{
					int iChestQueue=GetChestList[i].GetDynamicIntByKey(SignKeys.CHESTQUEUE);
					if(iChestQueue>ChestQueue)
					{
						GetChestList[i].SetDynamicData(SignKeys.CHESTQUEUE,iChestQueue-1);
					}
					if(iChestQueue==2)
					{
						GetChestList[i].SetRealTimeCountDown((int)GetChestList[i].Result(FormulaKeys.FORMULA_94));
					}
				}
				Messenger.Broadcast<int>(LevelPrepaerPanel.BraodCast_ChestMissAni,10);
				GetChestList.Remove(_host);
				ChestManageComponent.Instance.ChestBagToGrid();
				if(GetChestList.Count==0)
				{
					_host.Delete(new HttpResponseDelegate(SaleOneChestInGrid));
				}
				else 
				{
					_host.Delete();
					FormulaHost.SaveList(GetChestList,new HttpEndResponseDelegate(SaleChestInGirdCallBack));
				}

		//	}

		}

		public void SaleOneChestInGrid(bool _success)
		{
			if(_success)
			{
				CommonPanel.GetInstance().ShowWaittingPanel(false);
				//ChestManageComponent.Instance.ChestBagToGrid();
		
			//	Messenger.Broadcast(LevelPrepaerPanel.BraodCast_RestChestEmpty);
				Messenger.Broadcast(MainMenuPanel.BroadcastChangeMoney);
			}
			else 
			{
				CommonPanel.GetInstance().ShowText("Connect is fail");
			}

		}
		public void SaleChestInGirdCallBack(cn.bmob.response.EndPointCallbackData<Hashtable> response)
		{
			CommonPanel.GetInstance().ShowWaittingPanel(false);
			//ChestManageComponent.Instance.ChestBagToGrid();
			//Messenger.Broadcast<int>(LevelPrepaerPanel.BraodCast_ChestMissAni,10);
			//Messenger.Broadcast(LevelPrepaerPanel.BraodCast_RestChestEmpty);
			Messenger.Broadcast(MainMenuPanel.BroadcastChangeMoney);

		}
//		/// <summary>
//		/// 检测宝箱数据
//		/// </summary>
//		public void CheckChest()
//		{
//			if(GetTimeDownChest.Count==0)
//				return ;
//
//
//			for(int i=0,max=GetTimeDownChest.Count;i<max;i++)
//			{
//				FormulaHost tempHost=GetTaggetQueue(i+1);
//				if(tempHost.r)
//			}
//
//
//
//		}
	}


}