//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System;
/// <summary>
/// Tween the object's local scale.
/// </summary>

//[AddComponentMenu("NGUI/Tween/Tween Scale")]
public class Beat :BeatTween
{
	public GameObject AudioBeat;
	
	public GameObject genergy;
	public bool m_energy=false;
	public bool m_hithat=false;
	public bool m_kick=false;
	public bool m_snare=false;
	public bool Roat=true;

	public bool m_piay=false;

	public Vector3 from = Vector3.one;
	public Vector3 to = Vector3.one;

	public bool updateTable = false;

	public Vector3 from1 = Vector3.one;
	public Vector3 to1 = Vector3.one;
	//public bool updateTable1 = false;

	public Vector3 from2 = Vector3.one;
	public Vector3 to2 = Vector3.one;
//	public bool updateTable1 = false;

	public Vector3 from3 = Vector3.one;
	public Vector3 to3 = Vector3.one;

	public Vector3 from4 = Vector3.one;
	public Vector3 to4 = Vector3.one;

	public void Start()
	{
 	//	AudioBeat.GetComponent<BeatDetection>().CallBackFunction = MyCallbackEventHandler;
		GameObject.Find("Camera").GetComponent<BeatDetection>().CallBackFunction=MyCallbackEventHandler;
		this.gameObject.transform.localScale=Vector3.one;
		this.animationCurve=this.animationCurve1;
		this.AddOnFinished(new EventDelegate( FinishPlay));
	}


	public void MyCallbackEventHandler(BeatDetection.EventInfo eventInfo)
	{
		switch(eventInfo.messageInfo)
		{
		case BeatDetection.EventType.Energy:
			Debug.Log ("energy");
			
			if(m_energy)
				ShowChange(0);
			
			break;
		case BeatDetection.EventType.HitHat:
			if(m_hithat)
				ShowChange(1);
			Debug.Log ("hithat");
			break;
		case BeatDetection.EventType.Kick:
			Debug.Log ("kick");
			if(m_kick)
				ShowChange(2);
			
			break;
		case BeatDetection.EventType.Snare:
			Debug.Log ("snare");
			if(m_snare)
				ShowChange(3);
			break;
		}
	}


	public int TempInt;


	public void FinishPlay()
	{
		
		//_index=TempInt;

		this.PlayReverse();
		//		if (m_Scal == null) {
		//			return;
		//			Debug.Log ("m_Scal is null");
		//		}
		//		m_Scal.duration = ToBigTime;
		//		m_Scal.PlayReverse ();
		
	}
	public void ShowChange(int _index)
	{
		TempInt=_index;
		if(_index==0)
		{
			this.from=from1;
			this.to=to1;
			this.animationCurve=this.animationCurve1;
			this.delay=this.delay1;
			this.duration=this.duration1;
			Debug.Log("this.duration:"+this.duration);

		}
		if(_index==1)
		{
			this.from=from2;
			this.to=to2;
			this.animationCurve=this.animationCurve2;
			this.delay=this.delay2;
			this.duration=this.duration2;
			
		}
		if(_index==2)
		{
			this.from=from3;
			this.to=to3;
			this.animationCurve=this.animationCurve3;
			this.delay=this.delay3;
			this.duration=this.duration3;
			
		}
		if(_index==3)
		{
			this.from=from4;
			this.to=to4;
			this.animationCurve=this.animationCurve4;
			this.delay=this.delay4;
			this.duration=this.duration4;
			
		}
		this.PlayForward();
//		m_arrData[_index].m_TweenScal.duration=m_arrData[_index].ToSmallTime;
//		
//		m_arrData[_index].m_TweenScal.PlayForward();
//		
//		//		m_Scal.duration = ToSmallTime;
//		//		m_Scal.PlayForward ();
//		//		
//		//		
//		if(Roat)
//		{
//			
//			m_arrData[_index].m_TweenRotation.duration=m_arrData[_index].RAngleTime;
//			
//			
//			m_arrData[_index].m_TweenRotation.from.z=NowAngle;
//			m_arrData[_index].m_TweenRotation.to.z=m_arrData[_index].m_TweenRotation.from.z+m_arrData[_index].Angel;
//			NowAngle=NowAngle+m_arrData[_index].Angel;
//			m_arrData[_index].m_TweenRotation.Play();
//			
//			//		m_rota.duration=RAngleTime;
//			
//			
//			//			m_rota.from=m_rota.to;
//			//			m_rota.to=m_rota.from+new Vector3(0,0,Angel);
//			//			
//			//			m_rota.Play();
//			
//		}
//		
		
	}

	public void SetFromAndTo()
	{

	}
	public void PlayTween(int _index)
	{
	//	switch()
	}

	Transform mTrans;
	UITable mTable;
	public Transform cachedTransform{ get { if (mTrans == null) mTrans = transform; return mTrans; } }
	public Vector3 Scalevalue{ get { return cachedTransform.localScale; } set { cachedTransform.localScale = value; } }
	
//	[System.Obsolete("Use 'value' instead")]
//	public Vector3 scale { get { return this.value; } set { this.value = value; } }
	
	/// <summary>
	/// Tween the value.
	/// </summary>
	
	protected override void OnUpdate (float factor, bool isFinished)
	{


		Scalevalue = from * (1f - factor) + to * factor;

		if (updateTable)
		{
			if (mTable == null)
			{
				mTable = NGUITools.FindInParents<UITable>(gameObject);
				if (mTable == null) { updateTable = false; return; }
			}
			mTable.repositionNow = true;
		}
	}
	
	/// <summary>
	/// Start the tweening operation.
	/// </summary>
//	
//	static public TweenScale Begin (GameObject go, float duration, Vector3 scale)
//	{
//		TweenScale comp = UITweener.Begin<TweenScale>(go, duration);
//		comp.from = comp.value;
//		comp.to = scale;
//		
//		if (duration <= 0f)
//		{
//			comp.Sample(1f, true);
//			comp.enabled = false;
//		}
//		return comp;
//	}
	
//	[ContextMenu("Set 'From' to current value")]
//	public override void SetStartToCurrentValue () { from = value; }
//	
//	[ContextMenu("Set 'To' to current value")]
//	public override void SetEndToCurrentValue () { to = value; }
//	
//	[ContextMenu("Assume value of 'From'")]
//	void SetCurrentValueToStart () { value = from; }
//	
//	[ContextMenu("Assume value of 'To'")]
//	void SetCurrentValueToEnd () { value = to; }
}
