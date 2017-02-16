using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BlurPanel : MonoBehaviour {

	UIPanel m_panel=null;

	public Camera cam;
	public UnityStandardAssets.ImageEffects.BlurOptimized m_Blur;
	public UITexture m_texture;
	public UITexture m_texture2;
	RenderTexture Tex=null;

	public float TimeInterval=0.001f;
	public int  maxBlur=5;
	public int IntervalBlur=3;

	public float toblackTime=0.1f;
	public float toBlackAlphVaule=0.7f;
	public TweenAlpha m_blackTween;

	//List<UIPanel> m_ListPanel=new List<UIPanel>();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	
	}

	public void SetRenderOredrAddOne(UIPanel _panel)
	{

		if(Tex==null)
			InitBlurTex();
		this.gameObject.SetActive(true);
		if(m_panel==null)
		{
			m_panel=GetComponent<UIPanel>();
		}
		TimeWork.g_Instace.AddTime("BlurPanel_AddOne",0.001f,3f,PlayToBlur);


		m_panel.depth=_panel.depth+1;
		m_panel.sortingOrder=_panel.sortingOrder+1;
		m_Blur.blurIterations=3;
		//TweenAlpha.Begin (m_blackTween.gameObject, toblackTime, 0, toBlackAlphVaule);
		TweenAlpha.Begin (m_blackTween.gameObject, toblackTime, 0);
//		m_blackTween.ResetToBeginning();
//		m_blackTween.PlayForward();
	}
	public void SetRenderOredrSubOne(UIPanel _panel)
	{
		if(Tex==null)
			InitBlurTex();
		this.gameObject.SetActive(true);
		if(m_panel==null)
		{
			m_panel=GetComponent<UIPanel>();
		}
		//_panel.gameObject.layer=10;
		//m_ListPanel.Add(_panel);
		m_panel.depth=_panel.depth-1;
		m_panel.sortingOrder=_panel.sortingOrder-1;
		m_Blur.blurIterations=3;
		TimeWork.g_Instace.AddTime("BlurPanel_AddOne",0.001f,3f,PlayToBlur);


		//TweenAlpha.Begin(m_blackTween.gameObject,toblackTime,0,toBlackAlphVaule);
		//m_blackTween.ResetToBeginning();//TweenAlpha.Begin(m_blackTween.gameObject,0.1f,0,200);
		m_blackTween.PlayForward();
	}
	void PlayToBlur()
	{
		Debug.Log("PlayToBlur");
		m_blackin=true;
		if(m_Blur.blurIterations>=maxBlur)
		{
			TimeWork.g_Instace.KillTime("BlurPanel_AddOne");
			return ;
		}
		m_Blur.blurIterations+=IntervalBlur;
		
	}
	void PlayAwayBlur()
	{
		
		if(m_Blur.blurIterations<=1)
		{
			TimeWork.g_Instace.KillTime("BlurPanel_Close");
			return ;
		}

		m_Blur.blurIterations-=IntervalBlur;
		if(m_Blur.blurIterations<0)
		{
			m_Blur.blurIterations=1;
		}

	}
	bool m_blackin=true;
	public void CloseBlur()
	{
		TimeWork.g_Instace.AddTime("BlurPanel_Close",0.001f,3f,PlayAwayBlur);
		m_blackin=false;
		m_blackTween.PlayReverse();
	//	TweenAlpha.Begin(m_blackTween.gameObject,toblackTime,toBlackAlphVaule,0);
	//	m_blackTween.PlayReverse();
//		for(int i=0;i<m_ListPanel.Count;i++)
//		{
//			m_ListPanel[i].gameObject.layer=5;
//		}
//		m_ListPanel.Clear();
	}
	public void OnfinishMissBlack()
	{
		if(!m_blackin)
		{
			this.gameObject.SetActive(false);

		}
	}
	public void InitBlurTex()
	{

		Tex=new RenderTexture(Screen.width,Screen.height,0);
		cam.targetTexture=Tex;
		m_texture.mainTexture=Tex;
		m_texture.MakePixelPerfect();

		m_texture2.mainTexture=Tex;
		m_texture2.MakePixelPerfect();
	}


	void OnEnable()
	{
		//InvokeRepeating("Test",0.01f,1f);
		//Invoke()
	

	}
	public void Test()

	{
	//	Debug.Log("Time");
	}

}
//using UnityEngine;
//using System.Collections;
//
//public class test222 : MonoBehaviour {
//	public UnityStandardAssets.ImageEffects.BlurOptimized m_blur;
//
//	public float Timeinterval=0.02f;
//	public float BlurInterval=0.01f;
//	public float MaxBlur=6f;
//	// Use this for initialization
//	void Start () {
//
//	}
//
//	// Update is called once per frame
//	void Update () {
//
//
//
//	}
//
//	public void ClickTest()
//	{
//
//		m_blur.blurSize=0;
//		m_blur.blurIterations=0;
//		InvokeRepeating("ToBig",0.01f,Timeinterval);
//		m_blur.blurIterations=3;
//	}
//
//
//	public void ToBig()
//	{
//		m_blur.blurSize+=BlurInterval;
//
//
//
//		if(m_blur.blurSize>=MaxBlur)
//		{
//			CancelInvoke("ToBig");
//			Debug.Log("Finish");
//		}
//
//
//	}
//	public void ToSmall()
//	{
//
//	}
//
//
//}
