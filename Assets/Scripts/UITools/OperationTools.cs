using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using UnityEngine.Internal;
public class OperationTools 
{

	static bool Sound=false;
	static OperationTools m_Instan=null;
	public static OperationTools g_Instan
	{
		get
		{
			if(m_Instan==null)
			{
				m_Instan=new OperationTools();
				m_Instan.Init();
			}
			return m_Instan;
		}
	}
	void Init()
	{
		for(int i=0;i<20;i++)
		{
			m_listNumber[i]=new NumberAni();
			m_listNumber[i].Empty=true;
		}
	}
	#region //算了 不想写类 心里累

	#endregion

	/// <summary>
	/// 更改字符串中某字节
	/// </summary>
	/// <returns>The char.</returns>
	/// <param name="str">String.</param>
	/// <param name="index">Index.</param>
	/// <param name="_Random">是否随意此字符</param>
	static string ReplaceChar(string str, int index,bool _Random=false)
	{
		char[] carr = str.ToCharArray();
		int temp=(int)str[index];
		temp++;

		if(_Random)
		{
			temp=Random.Range(48,58);
		}
		else
		{
			if(temp>=58)
				temp=48;
		}

		carr[index]=(char)temp;
		return new string(carr);
	}
	#region 数值滚动的工具
	public delegate void CallBackFun();
	CallBackFun m_CallBack=null;
//
//	//public delegate void CallBackFun();
//	CallBackFun m_CallBackFinish=null;
	//简单粗暴的类  添加Number 进链表 重置开始时间
	//1.for(AddNumber)
	//2.SetBeginTime();

	float AniBeginTime=0;
	float AniEndTime=0;
	float Timeinterval=0.1f;



	float AniBeginTime2=0;
	float AniEndTime2=0;
	float Timeinterval2=0.02f;

	bool PlayNumberAnimation=false;
	public struct NumberAni
	{
		public string NowVaule;
		public string DesVaule;
		public UILabel Label;
		public int Desbits;
		public bool Empty;
//		public NumberAni()
//		{
//			Empty=true;//
//		}
	}
	//一个屏最多20个数字在滚
	NumberAni[] m_listNumber=new NumberAni[20];

	/// <summary>
	/// 设置时间间隔
	/// </summary>
	/// <param name="_vaule">Vaule.</param>
	public void SetTimeinterval(float  _vaule)
	{
		Timeinterval=_vaule;
	}
	/// <summary>
	/// 设置开始时间
	/// </summary>
	public void SetBeginTime()
	{
		AniBeginTime=RealTime.time;
		AniBeginTime2=AniBeginTime;
		PlayNumberAnimation=true;

		//InvokeRepeating("ChangeLastNumber",0,0.02f);
	}
	public void SetData(string _des)
	{
		
	}
	public void AddNumber(string _desNb,string _nowNb,ref UILabel _Laber,bool Random=false,CallBackFun _CallBack=null)
	{
		m_CallBack=_CallBack;
//		m_CallBackFinish=_CallBack2;
		//Sound=_sound;
		NumberAni temp=new NumberAni();
		temp.DesVaule=_desNb;
		temp.Label=_Laber;
		temp.Desbits=temp.DesVaule.Length;
		if(temp.Desbits>4)
		{
			Timeinterval=0.3f;
		}
		else 
		{
			Timeinterval=0.6f;
		}
		temp.NowVaule=new string('0',temp.DesVaule.Length);
		if(Random)
		{
			for(int i=2,max=temp.DesVaule.Length;i<max;i++)
			{
				temp.NowVaule=ReplaceChar(temp.NowVaule,i,true);
			}
		}
		for(int i=0;i<20;i++)
		{
			if(m_listNumber[i].Empty)
			{
				Debug.Log("ADD changeNb");
				m_listNumber[i]=temp;
				m_listNumber[i].Empty=false;
				break;
			}
		}

		//m_CallBack();
	}

	/// <summary>
	/// 添加要滚动的数字
	/// </summary>
	/// <param name="_NumberAni">Number ani.</param>
	public void AddNumber(ref NumberAni _NumberAni,bool Random=false)
	{


//		if(Random)//
//		{
//			for(int i=2,max=_NumberAni.DesVaule.Length;i<max;i++)
//			{
//				_NumberAni.NowVaule=ReplaceChar(_NumberAni.NowVaule,i,true);
//			}
//		}
//		m_listNumber.Add(_NumberAni);

	}
	/// <summary>
	/// 清空链表
	/// </summary>
	public void ClearList()
	{

		m_CallBack=null;
		//m_CallBackFinish=null;
//		for(int i=0;i<20;i++)
//		{
//			if()
//		}
		m_listNumber.Initialize();
	}
//	[WrapperlessIcall]
//	[MethodImpl (4096)]
//	public extern void CancelInvoke (string methodName);
//	[WrapperlessIcall]
//	[MethodImpl (4096)]
//	public extern void InvokeRepeating (string methodName, float time, float repeatRate);
	/// <summary>
	/// gaibian hou mian de shuzi
	/// </summary>
	void ChangeLastNumber()
	{

		AniEndTime2=RealTime.time;
		if(AniEndTime2-AniBeginTime2<Timeinterval2)
		{
			return ;
		}
		for(int j=0;j<20;j++)
		{
			if(!m_listNumber[j].Empty&&m_listNumber[j].DesVaule!=null)
			{
				if(m_listNumber[j].Desbits>4)
				{
					if(m_listNumber[j].DesVaule[0]==m_listNumber[j].NowVaule[0]&&m_listNumber[j].DesVaule[1]==m_listNumber[j].NowVaule[1])
					{
						m_listNumber[j].NowVaule=m_listNumber[j].DesVaule;
						m_listNumber[j].Label.text=m_listNumber[j].NowVaule;
						return;
					}
					else 
					{
						int tempbits=m_listNumber[j].DesVaule.Length;//位数2位以上
						for(int i=2,max=tempbits;i<max;i++)
						{
							m_listNumber[j].NowVaule=ReplaceChar(m_listNumber[j].NowVaule,i);
						}
					}
				}
				else 
				{
					if(m_listNumber[j].DesVaule[0]==m_listNumber[j].NowVaule[0])
					{
						m_listNumber[j].NowVaule=m_listNumber[j].DesVaule;
						m_listNumber[j].Label.text=m_listNumber[j].NowVaule;
						return;
					}
					else 
					{
						int tempbits=m_listNumber[j].DesVaule.Length;//位数2位以上
						for(int i=1,max=tempbits;i<max;i++)
						{
							m_listNumber[j].NowVaule=ReplaceChar(m_listNumber[j].NowVaule,i);
						}
					}
				}
	



				m_listNumber[j].Label.text=m_listNumber[j].NowVaule;
			}

		}
		AniBeginTime2=AniEndTime2;
	}
	void UpdataNumber()
	{


			ChangeLastNumber();
			//1.240完成 4秒
			//		if(FinishAnimation())
			//		{
			//			//finish
			//			PlayNumberAnimation=false;
			//			return ;
			//		}
			AniEndTime=RealTime.time;
			if(AniEndTime-AniBeginTime<Timeinterval)
			{
			#region dududu
//
//				for(int j=0;j<20;j++)
//				{
//					int tempbits=m_listNumber[j].DesVaule.Length;//位数2位以上
//					for(int i=2,max=tempbits;i<max;i++)
//					{
//						m_listNumber[j].NowVaule=ReplaceChar(m_listNumber[j].NowVaule,i);
//					}
//				}
			#endregion
			
			
			return ;

			}
			AniBeginTime=AniEndTime;
			for(int j=0;j<20;j++)
			{
			if(!m_listNumber[j].Empty&&m_listNumber[j].DesVaule!=null)
				{
					if(m_listNumber[j].DesVaule==m_listNumber[j].NowVaule)
					{

						m_listNumber[j].Label.text=m_listNumber[j].DesVaule;
						PlayNumberAnimation=false;

						m_listNumber[j].Empty=true;
							if(m_CallBack!=null)
								m_CallBack();
						//CancelInvoke("ChangeLastNumber");
						break;
						return ;
					}
					int tempbits=m_listNumber[j].DesVaule.Length;//位数2位以上
					if(tempbits>1)
					{
						if(tempbits>4)
						{
							if(m_listNumber[j].DesVaule[0]!=m_listNumber[j].NowVaule[0]||m_listNumber[j].DesVaule[1]!=m_listNumber[j].NowVaule[1])//不一样
							{
								string temp=m_listNumber[j].NowVaule;
								
								m_listNumber[j].NowVaule=ReplaceChar(m_listNumber[j].NowVaule,1);
								if(m_listNumber[j].NowVaule[1]<temp[1])
								{
									if(m_listNumber[j].NowVaule[0]==' ')
									{
										m_listNumber[j].NowVaule=m_listNumber[j].NowVaule.Remove(0,1);
										m_listNumber[j].NowVaule=m_listNumber[j].NowVaule.Insert(0,"1");
									}
									else 
									{
										m_listNumber[j].NowVaule=ReplaceChar(m_listNumber[j].NowVaule,0);
									}
								}
								if(m_listNumber[j].NowVaule[0]=='0')
								{
									m_listNumber[j].NowVaule=m_listNumber[j].NowVaule.Remove(0,1);
									m_listNumber[j].NowVaule=m_listNumber[j].NowVaule.Insert(0," ");
									Debug.LogWarning("Worning:"+m_listNumber[j].NowVaule);
								}
								
								//							for(int i=2,max=tempbits;i<max;i++)
								//							{
								//								m_listNumber[j].NowVaule=ReplaceChar(m_listNumber[j].NowVaule,i);
								//							}
							}
							else
							{
								m_listNumber[j].NowVaule=m_listNumber[j].DesVaule;
							}
						}
						else 
						{
							if(m_listNumber[j].DesVaule[0]!=m_listNumber[j].NowVaule[0])//不一样
							{
								//string temp=m_listNumber[j].NowVaule;
								
								m_listNumber[j].NowVaule=ReplaceChar(m_listNumber[j].NowVaule,0);
							}
							else
							{
								m_listNumber[j].NowVaule=m_listNumber[j].DesVaule;
							}
						}
		

					}
					else//1位的分数 
					{
						if(m_listNumber[j].DesVaule[0]!=m_listNumber[j].NowVaule[0])//不一样
						{
							m_listNumber[j].NowVaule=ReplaceChar(m_listNumber[j].NowVaule,0);
						}
					}
					m_listNumber[j].Label.text=m_listNumber[j].NowVaule;

				}
		}
	}
	#endregion
	public void UpdateTools()
	{
		
		if(PlayNumberAnimation)
		{
			UpdataNumber();
		}
	}
}
