//EDIT :SF 2015 12 16
//主界面相关东西 未全部完成 待续
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameLogic;


public class PlanetData
{
	public ushort m_id;                       //ID
	public ushort m_Layer; 					  //Chengshu
	public string m_planetSpriteName;  		  //xingqiu mingzi	
	public float  m_scaling;				  //suofang xishu	
}

/// <summary>
/// Planet manage.
/// </summary>
public class PlanetManage:System.Object
{
	public static PlanetManage m_Instan=null;
	public static PlanetManage Inst
	{
		get
		{
			if(m_Instan==null)
			{
				m_Instan=new PlanetManage();
			}
			return m_Instan;
		}
	}
	private Dictionary<ushort,PlanetData> m_dicPlanetManage=new Dictionary<ushort, PlanetData>();
	
	public  PlanetData GetPlanetData(ushort _id)
	{
		if(m_dicPlanetManage.ContainsKey(_id))
		{
			return m_dicPlanetManage[_id];
		}
		else
		{
			//	Debug.LogError("error id"+_id);
			return null;
		}
	}
	
	public int GetplanetNumber()
	{
		return m_dicPlanetManage.Count; 
		
	}
	/// <summary>
	/// Init
	/// </summary>
	public void InitPlanetManage(int _nb)
	{
		for(ushort i=1;i<=_nb;i++)
		{
			PlanetData temp=new PlanetData();
			temp.m_id=i;
			temp.m_Layer=(ushort)(5-i);
			temp.m_scaling=1.2f;
			//	temp.m_Layer=(ushort)((i+3)%5);
			m_dicPlanetManage[i]=temp;
		}
		
	}
	//*********************************************************
	//bei yong xing xi
	//0 1 2 3 4 
	//	public static ushort m_PlayerPlanetId=1;
	
	//*********************************************************
}


public class MainPanetPanel : MonoBehaviour {
	public GameObject m_PanetPerfab;
	public List<GameObject> m_Object2=new List<GameObject>();
	public UITable m_table;
	public Camera m_UIcamera2;
	float ScreenSizeWidth;
	
	
	float ScreenSizeMid;
	float ViewSizeWidth;

	#region //stars

	public GameObject []m_Stars=new GameObject[4];

	public void  RoatStars()
	{

		m_Stars[2].transform.parent.RotateAround(m_flagArr[1].transform.position,Vector3.forward,Time.deltaTime*5f);

		m_Stars[3].transform.parent.RotateAround(m_flagArr[1].transform.position,Vector3.forward,Time.deltaTime*2f);


	}

	public float tttt1=0.3f;
	public float tttt2=0.4f;
	public float tttt3=1f;
	public float tttt4=0.5f;

	public void MoveStars(Vector3 _temp)
	{
		m_Stars[0].transform.localPosition=m_arrOldStarSkyPos[0]-_temp*tttt1;//0.3
		m_Stars[1].transform.localPosition=m_arrOldStarSkyPos[1]-_temp*tttt2;//0.4
		m_Stars[2].transform.parent.parent.localPosition=m_arrOldStarSkyPos[2]-_temp*tttt3;//1
		m_Stars[3].transform.parent.parent.localPosition=m_arrOldStarSkyPos[3]-_temp*tttt4;//0.5


//		for(int i=0;i<4;i++)
//		{
		//	m_Stars[i].transform.localPosition=m_OldStarSkyPos-_temp*Mathf.Lerp(0.6f,1.1f);
		//}
		//m_StarSky.transform.localPosition=m_OldStarSkyPos-new Vector3(temp*nb2,0,0);


	}
	#endregion

	#region //Music

	public GameObject AudioBeat;

	
	public bool m_energy=false;
	public bool m_hithat=false;
	public bool m_kick=false;
	public bool m_snare=false;
	public float ToSmallTime=5f;
	public float ToBigTime=1f;
	List<TweenScale> mList_scal=new List<TweenScale>();

	public void ShowChange(int _index)
	{

//		for(int i=0,max=mList_scal.Count;i<1;i++)
//		{
//			if(mList_scal[i]!=null)
//			{
//				mList_scal[i].duration=ToSmallTime;
//				mList_scal[i].PlayForward();
//			}
//		}

//		Debug.Log("Play");
//		TweenScale[] arr=m_table.GetComponentsInChildren<TweenScale>();
//		for(int i=0;i<arr.Length;i++)
//		{
//			arr[i].duration=ToSmallTime;
//		//	arr[i].ResetToBeginning();
//		}
	//	m_table.GetComponent<UIPlayTween>().resetOnPlay();
	//	m_table.GetComponent<UIPlayTween>().Play(true);


	//	m_Scal.duration=ToSmallTime;
//		m_Scal.PlayForward();
	}


	public void MyCallbackEventHandler(BeatDetection.EventInfo eventInfo)
	{
//		switch(eventInfo.messageInfo)
//		{
//		case BeatDetection.EventType.Energy:
//		//	Debug.Log ("energy");
//			
//			if(m_energy)
//				ShowChange(0);
//			break;
//		case BeatDetection.EventType.HitHat:
//			if(m_hithat)
//				ShowChange(1);
//		//	Debug.Log ("hithat");
//			break;
//		case BeatDetection.EventType.Kick:
//		//	Debug.Log ("kick");
//			if(m_kick)
//				ShowChange(2);
//			
//			break;
//		case BeatDetection.EventType.Snare:
//		//	Debug.Log ("snare");
//			if(m_snare)
//				ShowChange(3);
//			break;
//		}
	}

	
	public void PlayerFinsih()
	{

//		for(int i=0,max=mList_scal.Count;i<1;i++)
//		{
//			if(mList_scal[i]!=null)
//			{
//
//				mList_scal[i].duration=ToBigTime;
//				mList_scal[i].PlayReverse();
//			}
//		}

		m_table.GetComponent<UIPlayTween>().Play(false);
//		if (m_Scal == null) 
//			return;
		
		//m_Scal.PlayReverse();
	}
	#endregion
	public int m_playerNowLevelPass=1;// dangqian yingxiong suozai guanka
	
	void InitPlanetInfo2()
	{
		//Debug.Log("Init");
		
		ScreenSizeWidth=Screen.width;
		//ScreenSize.y=Screen.height;
		for(int i=0,max=PlanetManage.Inst.GetplanetNumber();i<max;i++)
		{
			GameObject temp=Instantiate(m_PanetPerfab);
			temp.name+=i.ToString();
			temp.transform.parent=m_table.transform;
			temp.transform.localScale=m_PanetPerfab.transform.localScale;
			temp.SetActive(true);
			temp.GetComponent<PlanetCell>().InitPlanetCell(PlanetManage.Inst.GetPlanetData((ushort)(i+1)));
			//m_PlayerPlanetI

			mList_scal.Add(temp.GetComponent<TweenScale>());
			m_Object2.Add(temp);
		}
		m_table.Reposition();
	}
	
	public void MovePanet2()
	{
		
		//Debug.Log("Screen:"+Screen.width+"dasdasd" );
		for(int i=0,max=PlanetManage.Inst.GetplanetNumber();i<max;i++)
		{
			Vector3 temp= m_UIcamera2.WorldToScreenPoint(m_Object2[i].transform.position);
			if(temp.x>ScreenSizeWidth+500||(-500)>temp.x)
			{
				//	Debug.Log(m_Object2[i].name+"bu zai pingmu"+"x:"+temp.x);
			}
			else
			{
				
				ScalePlanet2(i,temp.x);
				//	Debug.Log(m_Object2[i].name+"zai"+"x:"+temp.x);
			}
			//m_Object2[i].transform.
		}
		
	}
	
	public void ScalePlanet2(int  _index,float _x)
	{
		float t_MidX=ScreenSizeMid;
		//		float Lenght=m_UIcamera2.WorldToScreenPoint(m_Object2[1].transform.position).x-m_UIcamera2.WorldToScreenPoint(m_Object2[0].transform.position).x;
		float Lenght=ViewSizeWidth;
		
		float n1=1f;
		
		//	Debug.Log("x:"+_x+"t_MidX"+t_MidX+"index:"+_index);
		
		if(_x>t_MidX)
		{
			if(_index-1>=0)
			{			
				n1=(_x-t_MidX)/(Lenght/2);
				//n1=Mathf.Lerp(1,m_Object2[_index].GetComponent<PlanetCell>().m_PlanetData.m_scaling,n1);
				n1=Mathf.Lerp(1,1.2f,n1);
				
			}
		}
		else
		{
			if(_index+1<m_Object2.Count)
			{
				n1=(t_MidX-_x)/(Lenght/2);
				//n1=Mathf.Lerp(1,m_Object2[_index].GetComponent<PlanetCell>().m_PlanetData.m_scaling,n1);
				n1=Mathf.Lerp(1,0.3f,n1);
			}
			
		}
		m_Object2[_index].transform.localScale=new Vector3(n1,n1,1);
	}
	public void ChangeLevel2(string _str)//you
	{
		//		float t_MidX=ScreenSizeMid;
		//		float nearPos=Mathf.Abs(m_UIcamera2.WorldToScreenPoint(m_Object2[m_playerNowLevelPass-1].transform.position).x-t_MidX);	
		//		float t_near=nearPos;
		//		int tIndex=m_playerNowLevelPass;
		for(int i=0,max=PlanetManage.Inst.GetplanetNumber();i<max;i++)
		{
			
			
			//			Vector3 temp= m_UIcamera2.WorldToScreenPoint(m_Object2[i].transform.position);
			//			float t=Mathf.Abs(temp.x-t_MidX);
			//
			//			if(t<t_near)
			//			{
			//				tIndex=i+1;
			//				t_near=t;
			//			}
			
			if(_str==m_Object2[i].name)
			{
				m_playerNowLevelPass=i+1;
				ChangeLevel3();
				return;
			}
		}
	}
	
	//预存位置信息
	public Transform []m_flagArr=new Transform[3];
	
	public List<GameObject> m_listLev=new List<GameObject>();
	
	//实际物体
	public List<GameObject> m_Object=new List<GameObject>();
	
	public bool m_Loop=false;                           //是否循环
	
	public float m_MaxSize=2f;							//最大倍率
	public float m_MinSize=0.3f;						//最小倍率
	
	public GameObject m_Scrollview;						//ScrollView起始位置
	//private Vector3 m_ScrollViewBeginPos;
	public SpringPanel m_SpringPanel;
	int m_PlayerPlanetId=1;                      //PlayerNowPlanet form 1
	
	public GameObject m_ChoseSongPanel;
	
	
	public UICenterOnChild m_UIcenterOnChuld;
	
	//	float flagDis=0;
	
	public void ChangeLevel3()//you
	{
		
		for(int i=0;i<5;i++)
		{
			m_listLev[i].GetComponent<StageSelectIcon>().stageId=(uint)((m_playerNowLevelPass-1)*5+i);
		}
		m_tPlayTween.Play(true);
	}
	void OnDisable()
	{
		if(m_ChoseSongPanel.activeSelf)
		{
			m_ChoseSongPanel.SetActive(false);
		}
	}
	void OnEnable()
	{
		
	}
	void Awake()
	{
		//m_UIcenterOnChuld.springStrength=20;
	}
	// Use this for initialization
	void Start () {
		Application.targetFrameRate=60;
		
		ScreenSizeMid=m_UIcamera2.WorldToScreenPoint(m_PanetPerfab.transform.position).x;
		
		
		ViewSizeWidth=2*Screen.width-ScreenSizeMid*2;
		
		//	Debug.Log("asdasd"+ScreenSizeMid);
		
		//Debug.Log(m_Scrollview.transform.localPosition.x);
		
		PlanetManage.Inst.InitPlanetManage(25);
		m_Scrollview.GetComponent<UIScrollView>().onStoppedMoving=onStoppedMoving;
		InitPlanetInfo();
		
		Time.timeScale=1;
		
		InitPlanetInfo2();

		m_skyBeginPosX=m_UIcamera2.WorldToScreenPoint(m_Object2[0].transform.position).x;

		m_skyBeginPosY=m_UIcamera2.WorldToScreenPoint(m_Object2[0].transform.position).y;

		for(int i=0;i<2;i++)
		{
			m_arrOldStarSkyPos[i]=m_Stars[i].transform.localPosition;
		}
		m_arrOldStarSkyPos[2]=m_Stars[2].transform.parent.localPosition;
		m_arrOldStarSkyPos[3]=m_Stars[3].transform.parent.localPosition;

		if(ui_camera==null)
		{
			//ui_camera.GetComponent<BeatDetection>().CallBackFunction = MyCallbackEventHandler;
		}
		else
		{
			ui_camera.GetComponent<BeatDetection>().CallBackFunction = MyCallbackEventHandler;
		}
	}
	///初始化星球信息
	void InitPlanetInfo()
	{
		for(int i=0;i<3;i++)
		{
			m_Object[i].GetComponent<PlanetCell>().InitPlanetCell(PlanetManage.Inst.GetPlanetData((ushort)(m_PlayerPlanetId-1+i)));
			//m_PlayerPlanetI
		}
	}
	// Update is called once per frame
	void Update () {
	}


	void FixedUpdate()
	{
		//MovePlanet2();
		MoveStarSky();
		MovePanet2();
		RoatStars();
		//	Debug.Log("fixEdUpdate");
	}
	public GameObject m_leftTg;
	public GameObject m_rightTg;
	public void TweenMoveLevelCell()
	{
		Debug.Log("TweenMoveLevelCell");
	}
	public UIPlayTween m_tPlayTween;
	//	int t_addOrSub=0;
	//	public void ChangeLevel(int _add)//you
	//	{
	//		if(_add>0)
	//		{
	//			for(int i=0;i<5;i++)
	//			{
	//				m_listLev[i].GetComponent<StageSelectIcon>().stageId+=5;
	//
	//				//m_listLev[i].GetComponent<UITweener>().PlayForward();
	//
	//			}
	//			t_addOrSub=1;
	//
	//		//	SwapObject(m_Object[2],m_Object[0]);
	//		//	Debug.Log(m_listLev[0].name+"  "+m_flagArr[2].name);
	////			for(int i=0;i<3;i++)
	////			{
	////				Debug.Log(m_Object[i].name);
	////			}
	//		
	//		}
	//		else
	//		{
	//			if(_add<0)
	//			{
	//					for(int i=0;i<5;i++)
	//				{
	//					m_listLev[i].GetComponent<StageSelectIcon>().stageId=m_listLev[i].GetComponent<StageSelectIcon>().stageId-5;
	//					//m_listLev[i].GetComponent<UITweener>().PlayForward();
	//				}
	//					t_addOrSub=-1;
	//			}
	//			else{
	//			//	Debug.Log("MB de zj yi zj wei mubiao");
	//				t_addOrSub=0;
	//			}
	//
	//		
	//
	////			for(int i=0;i<3;i++)
	////			{
	////				Debug.Log(m_Object[i].name);
	////			}
	//		//	m_tPlayTween.Play(true);
	//		}
	//		m_tPlayTween.Play(true);
	//		t_playForward=true;
	//	}
	
	//	public void SwapObject(GameObject  _t1,GameObject _t2)
	//	{
	//		GameObject temp=_t1;
	//		_t1=_t2;
	//		_t2=temp;
	//		
	//	}
	
	//对预设球体进行新的赋值与
	void onStoppedMoving()
	{
		//Debug.Log("onStoppedMoving");
		//		for(int i=0;i<3;i++)
		//		{
		//			m_Object[i].GetComponent<PlanetCell>().InitPlanetCell(PlanetManage.Inst.GetPlanetData((ushort)(m_PlayerPlanetId-1+i)));
		//			//m_PlayerPlanetI
		//		}
		
		//Debug.Log("ScrollView 移动结束 该干什么 干什么了"+t_addOrSub+"sssssssssss"+m_PlayerPlanetId+1);
		
		//		if(t_addOrSub>0)
		//		{
		//			m_PlayerPlanetId++;
		//			m_Object[0].transform.position+=3*m_LenghBetwwenPlanet;
		//			m_Object[0].GetComponent<PlanetCell>().InitPlanetCell(PlanetManage.Inst.GetPlanetData((ushort)(m_PlayerPlanetId+1)));
		//
		//			m_Object.Add(m_Object[0]);
		//			m_Object.RemoveAt(0);
		//			
		//		}
		//		else
		//		{
		//			if(t_addOrSub<0)
		//			{
		//				m_PlayerPlanetId--;
		//				m_Object[2].transform.position-=3*m_LenghBetwwenPlanet;
		//				m_Object[2].GetComponent<PlanetCell>().InitPlanetCell(PlanetManage.Inst.GetPlanetData((ushort)(m_PlayerPlanetId-1)));
		//				//SwapObject(m_Object[2],m_Object[0]);
		//				m_Object.Insert(0,m_Object[2]);
		//				m_Object.RemoveAt(3);
		//			}
		//
		//		}
		//
		//	//	t_offsetX=0;
		//		m_ScrollViewBeginPos=m_Scrollview.transform.position;
		
	}
	
	public void ClickSong(uint _id)
	{
		
		m_ChoseSongPanel.SetActive(true);
		m_ChoseSongPanel.GetComponent<ChoseSongPanel>().InitChoseSongPanel(_id);
		
	}
	#region Move PlanetMove
	
	
	/// <summary>
	/// 由小变大
	/// </summary>
	/// <param name="_ob1">Ob1.</param>Midd
	/// <param name="_ob2">Ob2.</param>
	//	public void SmalltoBig(GameObject _ob1,GameObject _ob2,int _index)
	//	{
	//
	////	Debug.Log("SmalltoBig");
	//		float nb1=	Mathf.Abs(_ob2.transform.position.x-m_flagArr[_index].position.x)/Mathf.Abs(flagDis);
	//
	//		if(nb1>1)
	//			nb1=1;
	//		float temp1=Mathf.Lerp(1f,m_MaxSize,1-nb1);
	//		temp1=(float)((int)(temp1*100))/100;
	//		_ob2.transform.localScale=new Vector3(temp1,temp1,temp1);
	////		//nb1.Round(); 
	////		nb1=decimal.Round(decimal.Parse("0.55555"),2);
	////		Debug.Log("Nb111"+nb1);
	//
	//		//float nb2=	Mathf.Abs(_ob2.transform.position.x-m_flagArr[_index].position.x)/Mathf.Abs(m_flagArr[2].position.x-m_flagArr[1].position.x);
	//		//Debug.Log("asdgdfgdsf"+nb2+"");
	//		float temp2=Mathf.Lerp(1,m_MinSize,nb1);
	//
	//		temp2=(float)((int)(temp2*100))/100;
	//		_ob1.transform.localScale=new Vector3(temp2,temp2,temp2);
	//
	//	}
	/// <summary>
	/// 大变小
	/// </summary>
	/// <param name="_ob1">_ob1.中间球</param>
	/// <param name="_ob2">_ob2.边缘球</param>
	/// <param name="_index">_index.目的地</param>
	//	public void BigtoSmall(GameObject _ob1,GameObject _ob2,int _index)
	//	{
	//
	//	//	Debug.Log("bIGtOsMALL");
	//
	//		//Debug.Log(_ob1.transform.position.x+"+"+m_flagArr[_index].position.x+"+"+(m_flagArr[1].position.x-m_flagArr[2].position.x));
	//		float nb1=	Mathf.Abs(_ob1.transform.position.x-m_flagArr[_index].position.x)/Mathf.Abs(flagDis);
	//		if(nb1>1)
	//			nb1=1;
	//		float temp1=Mathf.Lerp(1f,m_MinSize,1-nb1);
	//
	//		temp1=(float)((int)(temp1*100))/100;
	//		_ob1.transform.localScale=new Vector3(1,1,1)*temp1;
	//
	//		//Debug.Log("BigToSmall"+nb1);
	//
	//
	//		//float nb1=	Mathf.Abs(_ob1.transform.position.x-m_flagArr[_index].position.x)/Mathf.Abs(m_flagArr[2].position.x-m_flagArr[1].position.x);
	//		float temp2=Mathf.Lerp(1f,m_MaxSize,nb1);
	//		temp2=(float)((int)(temp2*100))/100;
	//		_ob2.transform.localScale=new Vector3(1,1,1)*temp2;
	//
	//	}
	public  bool  t_playForward=true;
	
	
	public void LevelMoveFinish()
	{
		
		for(int i=0;i<5;i++)
		{
			m_listLev[i].GetComponent<StageSelectIcon>().InitStageSelectIcon();//-=5;
			//m_listLev[i].GetComponent<UITweener>().PlayForward();s
		}
		m_tPlayTween.Play(false);
		//	//	Debug.Log("gai bian yudingyi weizhi");
		//		if(!t_playForward)
		//		{
		//			t_playForward=!t_playForward;
		//			return ;
		//
		//		}
		//		for(int i=0;i<5;i++)
		//			{
		//				m_listLev[i].GetComponent<StageSelectIcon>().InitStageSelectIcon();//-=5;
		//				//m_listLev[i].GetComponent<UITweener>().PlayForward();s
		//			}
		//
		//
		//		//Debug.Log("ScrollView 移动结束 该干什么 干什么了"+t_addOrSub+"sssssssssss"+m_PlayerPlanetId+1);
		//		if(t_addOrSub>0)
		//		{
		//			m_PlayerPlanetId++;
		//			//m_Object[0].transform.position+=3*m_LenghBetwwenPlanet;
		//			m_Object[0].transform.position=m_flagArr[2].transform.position;
		//			m_Object[0].GetComponent<PlanetCell>().InitPlanetCell(PlanetManage.Inst.GetPlanetData((ushort)(m_PlayerPlanetId+1)));
		////			if(!m_Loop)
		////			{
		////				m_Object[0].GetComponent<PlanetCell>().InitPlanetCell(PlanetManage.Inst.GetPlanetData((ushort)(m_PlayerPlanetId+1)));
		////			}
		////			else{
		////				if(m_PlayerPlanetId+1>PlanetManage.Inst.GetplanetNumber())
		////				{
		////					m_Object[0].GetComponent<PlanetCell>().InitPlanetCell(PlanetManage.Inst.GetPlanetData((ushort)(m_PlayerPlanetId-1)));
		////				}
		////				else{
		////					m_Object[0].GetComponent<PlanetCell>().InitPlanetCell(PlanetManage.Inst.GetPlanetData((ushort)(m_PlayerPlanetId+1)));
		////				}
		//			//}
		//
		//
		//			m_Object.Add(m_Object[0]);
		//			m_Object.RemoveAt(0);
		//
		//		}
		//		else
		//		{
		//			if(t_addOrSub<0)
		//			{
		//				m_PlayerPlanetId--;
		//				//m_Object[2].transform.position-=3*m_LenghBetwwenPlanet;
		//				m_Object[2].transform.position=m_flagArr[0].transform.position;
		//				m_Object[2].GetComponent<PlanetCell>().InitPlanetCell(PlanetManage.Inst.GetPlanetData((ushort)(m_PlayerPlanetId-1)));
		////				if(!m_Loop)
		////				{
		////					m_Object[2].GetComponent<PlanetCell>().InitPlanetCell(PlanetManage.Inst.GetPlanetData((ushort)(m_PlayerPlanetId-1)));
		////				}
		////				else{
		////					if(m_PlayerPlanetId-1<0)
		////					{
		////						m_Object[2].GetComponent<PlanetCell>().InitPlanetCell(PlanetManage.Inst.GetPlanetData((ushort)(PlanetManage.Inst.GetplanetNumber()-1)));
		////					}
		////					else{
		////						m_Object[2].GetComponent<PlanetCell>().InitPlanetCell(PlanetManage.Inst.GetPlanetData((ushort)(m_PlayerPlanetId-1)));
		////					}
		////				}
		//
		//				//SwapObject(m_Object[2],m_Object[0]);
		//				m_Object.Insert(0,m_Object[2]);
		//				m_Object.RemoveAt(3);
		//			}
		
		//		}
		//
		//
		//
		////		if(m_Object[0].GetComponent<PlanetCell>().m_LayerPos>m_Object[1].GetComponent<PlanetCell>().m_LayerPos)
		////		{
		////			m_Object[0].transform.localScale=new Vector3(m_MaxSize,m_MaxSize,1);
		////		}
		////		else
		////		{
		////			m_Object[0].transform.localScale=new Vector3(m_MinSize,m_MinSize,1);
		////		}
		////
		////		if(m_Object[2].GetComponent<PlanetCell>().m_LayerPos>m_Object[1].GetComponent<PlanetCell>().m_LayerPos)
		////		{
		////			m_Object[2].transform.localScale=new Vector3(m_MaxSize,m_MaxSize,1);
		////		}
		////		else
		////		{
		////			m_Object[2].transform.localScale=new Vector3(m_MinSize,m_MinSize,1);
		//		//}
		//
		//
		//
		//	//	t_offsetX=0;
		//		m_ScrollViewBeginPos=m_Scrollview.transform.position;
		//
		//
		//		t_playForward=!t_playForward;
		////			Debug.Log("mOVEfINISH");
		//		m_tPlayTween.Play(false);
		//		t_addOrSub=0;
		//		//StartCoroutine("PlayerMove");
	}
	
	/// <summary>
	/// Planets IS loop.
	/// </summary>
	public void PlanetLoop()
	{
		if(!m_Loop)
			return ;
		
		//if(m_PlayerPlanetId+1>PlanetManage.Inst.GetplanetNumber)
		
	}
	
	public UICamera t_UICamera;
	public Camera ui_camera;
	//public float t_1228MidPosX;
	public void MovePlanet2()
	{
		
		
		Vector3 t_tagPos1=ui_camera.WorldToScreenPoint(m_flagArr[1].transform.position);
		
		//ui_camera.WorldToScreenPoint()
		//		Debug.Log("pos:"+ui_camera.WorldToScreenPoint(m_Object[1].transform.position));
		//
		//		Debug.Log("pos0000:"+ui_camera.WorldToScreenPoint(m_flagArr[0].transform.position));
		//		Debug.Log("pos1111:"+ui_camera.WorldToScreenPoint(m_flagArr[1].transform.position));
		//		Debug.Log("pos2222:"+ui_camera.WorldToScreenPoint(m_flagArr[2].transform.position));
		//		Debug.Log("ScreenSize:"+Screen.width+"::"+Screen.height);
		//	Vector3 = camera.WorldToScreenPoint (target.position);
		//Camera.WorldToScreenPoint()
		//Debug.Log("m_ScrollViewBeginPos:"+m_ScrollViewBeginPos.x+"m_Scrollview.transform.position.x:"+m_Scrollview.transform.position.x);
		
		//	tpos0=
		Vector3 tpos1=ui_camera.WorldToScreenPoint(m_Object[1].transform.position);
		if(Mathf.Abs(t_tagPos1.x-tpos1.x)<1)
		{
			// xiuzheng renwei xiangdeng
			//Debug.Log("NO Move+"+(t_tagPos1.x-tpos1.x));
			return;
		}
		if(tpos1.x>t_tagPos1.x)
		{
			//Debug.Log ("right");
			
			if(!m_Object[1].activeSelf||!m_Object[0].activeSelf)
			{
				//	Debug.Log("the end");
				return;
			}
			MoveRight2();
		}
		else
		{
			//Debug.Log ("left");
			if(!m_Object[1].activeSelf||!m_Object[2].activeSelf)
			{
				//	Debug.Log("the end");
				return;
			}
			MoveLeft2();
			
		}
		//		if(m_ScrollViewBeginPos.x<m_Scrollview.transform.position.x)
		//		{
		//			Debug.Log("Move right");
		//			if(!m_Object[1].activeSelf||!m_Object[0].activeSelf)
		//			{
		//				Debug.Log("the end");
		//				return;
		//			}
		//
		//		}
		//		else{
		//			if(m_ScrollViewBeginPos.x==m_Scrollview.transform.position.x)
		//			{
		//
		//				Debug.Log("No move");
		//				return;
		//			}
		//			if(!m_Object[1].activeSelf||!m_Object[2].activeSelf)
		//			{
		//				Debug.Log("the end");
		//				return;
		//			}
		//
		//		}
		
	}
	
	public void FANGDA(GameObject _obMidd,GameObject _ob,bool Left=true)
	{
		Vector3 t_tagPos1=ui_camera.WorldToScreenPoint(m_flagArr[1].transform.position);
		Vector3 tpos1=ui_camera.WorldToScreenPoint(_obMidd.transform.position);
		Vector3 tpos2=ui_camera.WorldToScreenPoint(_ob.transform.position);
		float Screen_w=Screen.width/2f;
		
		
		float t_float=Mathf.Abs((tpos1.x-t_tagPos1.x))/Screen_w;
		
		float t_float2;
		float n2;
		//		if(Left)
		//		{
		if(tpos2.x>t_tagPos1.x)
		{
			t_float2=Mathf.Abs((tpos2.x-t_tagPos1.x))/Screen_w;
			if(t_float2>1)
				t_float2=1;
			n2=Mathf.Lerp(1,m_MinSize,t_float2);
		}
		else
		{
			//				//caoguoyiban
			//				if(_ob.GetComponent<PlanetCell>().m_LayerPos<m_Object[2].GetComponent<PlanetCell>().m_LayerPos)
			//				{
			//					t_float2=Mathf.Abs((tpos2.x-t_tagPos1.x))/Screen_w;
			//					if(t_float2>1)
			//						t_float2=1;
			//					n2=Mathf.Lerp(1,m_MinSize,t_float2);
			//				}
			//				else
			//				{
			t_float2=Mathf.Abs((tpos2.x-t_tagPos1.x))/Screen_w;
			if(t_float2>1)
				t_float2=1;
			n2=Mathf.Lerp(1,m_MaxSize,t_float2);
			//				}
		}
		
		
		//		}
		//		else{
		//
		//
		//		}
		
		
		
		
		float n1=Mathf.Lerp(1,m_MaxSize,t_float);
		
		
		//Debug.Log("n2:"+n2);
		_obMidd.transform.localScale=new Vector3(n1,n1,1);
		_ob.transform.localScale=new Vector3(n2,n2,1);
		
	}
	public void SUOXIAO(GameObject _obMidd,GameObject _ob,bool Left=true)
	{
		
		Vector3 t_tagPos1=ui_camera.WorldToScreenPoint(m_flagArr[1].transform.position);
		Vector3 tpos1=ui_camera.WorldToScreenPoint(_obMidd.transform.position);
		Vector3 tpos2=ui_camera.WorldToScreenPoint(_ob.transform.position);
		float Screen_w=Screen.width/2f;
		
		float t_float=Mathf.Abs((tpos1.x-t_tagPos1.x))/Screen_w;
		float t_float2;
		float n2;
		if(tpos2.x<t_tagPos1.x)
		{
			t_float2=Mathf.Abs((tpos2.x-t_tagPos1.x))/Screen_w;
			if(t_float2>1)
				t_float2=1;
			n2=Mathf.Lerp(1,m_MaxSize,t_float2);
		}
		else
		{
			//				//caoguoyiban
			//				if(_ob.GetComponent<PlanetCell>().m_LayerPos<m_Object[2].GetComponent<PlanetCell>().m_LayerPos)
			//				{
			//					t_float2=Mathf.Abs((tpos2.x-t_tagPos1.x))/Screen_w;
			//					if(t_float2>1)
			//						t_float2=1;
			//					n2=Mathf.Lerp(1,m_MinSize,t_float2);
			//				}
			//				else
			//				{
			t_float2=Mathf.Abs((tpos2.x-t_tagPos1.x))/Screen_w;
			if(t_float2>1)
				t_float2=1;
			n2=Mathf.Lerp(1,m_MinSize,t_float2);
			//				}
		}
		
		//t_float2=Mathf.Abs((tpos2.x-t_tagPos1.x))/Screen_w;
		//		if(t_float2>1)
		//			t_float2=1;
		
		float n1=Mathf.Lerp(1,m_MinSize,t_float);
		//	n2=Mathf.Lerp(1,m_MaxSize,t_float2);
		
		_obMidd.transform.localScale=new Vector3(n1,n1,1);
		_ob.transform.localScale=new Vector3(n2,n2,1);
		
	}
	public void MoveLeft2()
	{
		if(m_Object[1].GetComponent<PlanetCell>().m_LayerPos>m_Object[2].GetComponent<PlanetCell>().m_LayerPos)
		{
			//fangda
			FANGDA(m_Object[1],m_Object[2]);
		}
		else
		{
			if(m_Object[1].GetComponent<PlanetCell>().m_LayerPos==m_Object[2].GetComponent<PlanetCell>().m_LayerPos)
			{
				//xiangdeng
				return ;
			}
			else{
				SUOXIAO(m_Object[1],m_Object[2]);
				//suoxiao
			}
		}
		
	}
	public void MoveRight2()
	{
		if(m_Object[1].GetComponent<PlanetCell>().m_LayerPos>m_Object[0].GetComponent<PlanetCell>().m_LayerPos)
		{
			//fangda
			FANGDA(m_Object[1],m_Object[0],false);
		}
		else
		{
			if(m_Object[1].GetComponent<PlanetCell>().m_LayerPos==m_Object[0].GetComponent<PlanetCell>().m_LayerPos)
			{
				//xiangdeng
				return ;
			}
			else{
				SUOXIAO(m_Object[1],m_Object[0],false);
				//suoxiao
			}
		}
		
	}
	
	//	public void MovePlanet()
	//	{
	//	//pan duan zuoyou zuo wei +
	//		//if(m_Object[1].transform.position.x>m_flagArr[1].position.x)//xiang you
	//	//	Debug.Log(m_ScrollViewBeginPos+"123123"+m_Scrollview.transform.position);
	//		if(Mathf.Abs(m_ScrollViewBeginPos.magnitude-m_Scrollview.transform.position.magnitude)<0.01)
	//		{
	//	//		Debug.Log("====================================");
	//		}
	//		else
	//		{
	//		if(m_ScrollViewBeginPos.x<m_Scrollview.transform.position.x)
	//			{
	//
	//		//		Debug.Log("Move right");
	//				if(!m_Object[1].activeSelf||!m_Object[0].activeSelf)
	//				{
	//					//Debug.Log("dont scal");
	//					return;
	//				}
	//				if(m_Object[1].GetComponent<PlanetCell>().m_LayerPos>=m_Object[0].GetComponent<PlanetCell>().m_LayerPos)
	//					{
	//					SmalltoBig(m_Object[0],m_Object[1],2);//small to bigX
	//					}
	//					else
	//					{
	//				//	BigtoSmall(m_Object[1],m_Object[0],2);
	//					}
	//			}
	//			else//xiang zuo
	//			{
	//	//			Debug.Log("Move left");
	//				if(!m_Object[1].activeSelf||!m_Object[2].activeSelf)
	//				{
	//			//		Debug.Log("dont scal");
	//					return;
	//				}
	//				if(m_Object[1].GetComponent<PlanetCell>().m_LayerPos>=m_Object[2].GetComponent<PlanetCell>().m_LayerPos)
	//				{
	//			//		SmalltoBig(m_Object[2],m_Object[1],0);//small to big
	//				}
	//				else
	//				{
	//				BigtoSmall(m_Object[1],m_Object[2],0);  //x
	//				}
	//				
	//			}
	//
	//		}
	//	}
	
	#endregion 
	
	#region Test
	public static float MainScroview=2.0f;
	public GameObject m_touchV;
	public UILabel m_TouchVlabel;
	public void ClickTestButton()
	{
		if(m_touchV.activeSelf)
		{
			m_touchV.SetActive(false);
		}
		else
		{
			m_touchV.SetActive(true);
		}
	}
	
	public void SetTestVaule(float _value)
	{
		MainScroview=Mathf.Lerp(1,3,_value);
		m_TouchVlabel.text=MainScroview.ToString();
	}
	#endregion 

	public GameObject m_StarSky;

	public Vector3 m_OldStarSkyPos;
	public Vector3[] m_arrOldStarSkyPos=new Vector3[4];

	public GameObject m_StandWeight;


	float m_skyBeginPosX;
	float m_skyBeginPosY;


	Vector3 m_SkyRoationCenter;

	public void MoveStarSky()
	{

		float mid=m_UIcamera2.WorldToScreenPoint(m_flagArr[1].transform.position).x;
		if(m_Object2.Count<2)
			return;
		if(mid-m_UIcamera2.WorldToScreenPoint(m_Object2[0].transform.position).x<=0)
		{

	//		Debug.Log("m_skyBeginPosX:"+m_UIcamera2.WorldToScreenPoint(m_flagArr[1].transform.position)+"m_Object[0].transform.localPosition"+m_UIcamera2.WorldToScreenPoint(m_Object2[0].transform.position));
			return;
		}

		//float mid=m_UIcamera2.WorldToScreenPoint(m_flagArr[1].transform.position).x;
		float nb=(mid -m_UIcamera2.WorldToScreenPoint(m_Object2[0].transform.position).x)/(m_UIcamera2.WorldToScreenPoint(m_Object2[m_Object2.Count-1].transform.position).x-m_UIcamera2.WorldToScreenPoint(m_Object2[0].transform.position).x);

	//	Debug.Log("up"+(mid -m_UIcamera2.WorldToScreenPoint(m_Object2[0].transform.position).x)+"down"+(m_UIcamera2.WorldToScreenPoint(m_Object2[m_Object2.Count-1].transform.position).x-m_UIcamera2.WorldToScreenPoint(m_Object2[0].transform.position).x));
		//=(ScreenSizeMid-m_UIcamera2.WorldToScreenPoint(m_Object2[0].transform.position).x)/(m_UIcamera2.WorldToScreenPoint(m_Object2[m_Object.Count-1].transform.position).x-m_UIcamera2.WorldToScreenPoint(m_Object2[0].transform.position).x);
		//Debug.Log("nb:"+nb);


		//ALL LENGTH
		float temp=m_StarSky.GetComponent<UIWidget>().width-m_StandWeight.GetComponent<UIWidget>().width;
	//	Debug.Log("temp:"+temp);                      


		float nb2=Mathf.Lerp(0f,1f,nb);
	//	Debug.Log("nb2:"+nb2);


		//m_StarSky.transform.localPosition=m_OldStarSkyPos-new Vector3(temp*nb2,0,0);
		MoveStars(new Vector3(temp*nb2,0,0));

	}
//	Vector3 pos = Camera.main.WorldToScreenPoint(worldPos);
//	pos.z = 0f;   //z一定要为0.
//	
//	2. 使用UI摄像机转换到NGUI的世界坐标
//		Vector3 pos2 = UICamera.currentCamera.ScreenToWorldPoint(pos);
//	
//	3. 赋值给NGUI控件
//		temp.transform.position = pos2; //temp为NGUI控件.
}
