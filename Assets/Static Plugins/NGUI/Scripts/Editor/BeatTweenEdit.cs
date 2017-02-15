
	//----------------------------------------------
	//            NGUI: Next-Gen UI kit
	// Copyright © 2011-2015 Tasharen Entertainment
	//----------------------------------------------
	
	using UnityEngine;
	using UnityEditor;
	
[CustomEditor(typeof(BeatTween),true)]
	public class BeatTweenEdit : Editor
	{
		public override void OnInspectorGUI ()
		{
			GUILayout.Space(6f);
			NGUIEditorTools.SetLabelWidth(110f);
			base.OnInspectorGUI();
		//	DrawCommonProperties();
//			Test tw = target as Test;
			DrawCommonPropertiesBegin();
			DrawCommonProperties1();
			DrawCommonProperties2();
			DrawCommonProperties3();
			DrawCommonProperties4();
			DrawCommonPropertiesEnd ();
		}
		
//	protected void DrawCommonProperties()
//	{
//
//		Test tw = target as Test;
//		if (NGUIEditorTools.DrawHeader("Tweener"))
//		{
//
//
//		AnimationCurve curve1 = EditorGUILayout.CurveField("Curve", tw.animationCurve1, GUILayout.Width(190f), GUILayout.Height(62f));
//		//UITweener.Method method = (UITweener.Method)EditorGUILayout.EnumPopup("Play Method", tw.method);
//		GUILayout.BeginHorizontal();
//		float dur1 = EditorGUILayout.FloatField("Duration", tw.duration1, GUILayout.Width(170f));
//		GUILayout.Label("seconds");
//		GUILayout.EndHorizontal();
//		GUILayout.BeginHorizontal();
//		float del1 = EditorGUILayout.FloatField("Start Delay", tw.delay1, GUILayout.Width(170f));
//		GUILayout.Label("seconds");
//		GUILayout.EndHorizontal();
//
//
//
//
//		AnimationCurve curve2 = EditorGUILayout.CurveField("Curve", tw.animationCurve2, GUILayout.Width(190f), GUILayout.Height(62f));
//		//UITweener.Method method = (UITweener.Method)EditorGUILayout.EnumPopup("Play Method", tw.method);
//		GUILayout.BeginHorizontal();
//		float dur2 = EditorGUILayout.FloatField("Duration", tw.duration2, GUILayout.Width(170f));
//		GUILayout.Label("seconds");
//		GUILayout.EndHorizontal();
//		
//		GUILayout.BeginHorizontal();
//		float del2 = EditorGUILayout.FloatField("Start Delay", tw.delay2, GUILayout.Width(170f));
//		GUILayout.Label("seconds");
//		GUILayout.EndHorizontal();
//
//
//
//
//		AnimationCurve curve3 = EditorGUILayout.CurveField("Curve", tw.animationCurve3, GUILayout.Width(190f), GUILayout.Height(62f));
//		//UITweener.Method method = (UITweener.Method)EditorGUILayout.EnumPopup("Play Method", tw.method);
//		GUILayout.BeginHorizontal();
//		float dur3 = EditorGUILayout.FloatField("Duration", tw.duration3, GUILayout.Width(170f));
//		GUILayout.Label("seconds");
//		GUILayout.EndHorizontal();
//		
//		GUILayout.BeginHorizontal();
//		float del3 = EditorGUILayout.FloatField("Start Delay", tw.delay3, GUILayout.Width(170f));
//		GUILayout.Label("seconds");
//		GUILayout.EndHorizontal();
//
//
//
//
//		AnimationCurve curve4 = EditorGUILayout.CurveField("Curve", tw.animationCurve4, GUILayout.Width(190f), GUILayout.Height(62f));
//		//UITweener.Method method = (UITweener.Method)EditorGUILayout.EnumPopup("Play Method", tw.method);
//		GUILayout.BeginHorizontal();
//		float dur4 = EditorGUILayout.FloatField("Duration", tw.duration4, GUILayout.Width(170f));
//		GUILayout.Label("seconds");
//		GUILayout.EndHorizontal();
//		
//		GUILayout.BeginHorizontal();
//		float del4 = EditorGUILayout.FloatField("Start Delay", tw.delay4, GUILayout.Width(170f));
//		GUILayout.Label("seconds");
//		GUILayout.EndHorizontal();
//
//
//			if (GUI.changed)
//			{
//				NGUIEditorTools.RegisterUndo("Tween Change", tw);
//				tw.animationCurve1 = curve1;
//				tw.animationCurve2 = curve2;
//				tw.animationCurve3 = curve3;
//				tw.animationCurve4 = curve4;
//				tw.duration1 = dur1;
//				tw.duration2 = dur2;
//				tw.duration3 = dur3;
//				tw.duration4 = dur4;
//				tw.delay1 = del1;
//				tw.delay2 = del2;
//				tw.delay3 = del3;
//				tw.delay4 = del4;
//				//tw.method = method;
//				//tw.style = style;
//				//tw.ignoreTimeScale = ts;
//				
//				//tw.tweenGroup = tg;
//				//	tw.duration = dur;
//				//	tw.delay = del;
//				NGUITools.SetDirty(tw);
//			}
//		}
//
//
//	}
	protected void DrawCommonProperties1( )
	{
		BeatTween tw = target as BeatTween;


			AnimationCurve curve1 = EditorGUILayout.CurveField("Curve", tw.animationCurve1, GUILayout.Width(170f), GUILayout.Height(62f));
			//UITweener.Method method = (UITweener.Method)EditorGUILayout.EnumPopup("Play Method", tw.method);
			GUILayout.BeginHorizontal();
			float dur1 = EditorGUILayout.FloatField("Duration", tw.duration1, GUILayout.Width(170f));
			GUILayout.Label("seconds");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			float del1 = EditorGUILayout.FloatField("Start Delay", tw.delay1, GUILayout.Width(170f));
			GUILayout.Label("seconds");
			GUILayout.EndHorizontal();
		if (GUI.changed)
		{
			NGUIEditorTools.RegisterUndo("Tween Change", tw);
			tw.animationCurve1 = curve1;
			tw.duration1 = dur1;
			tw.delay1 = del1;

			//tw.method = method;
			//tw.style = style;
			//tw.ignoreTimeScale = ts;
			
			//tw.tweenGroup = tg;
			//	tw.duration = dur;
			//	tw.delay = del;
			NGUITools.SetDirty(tw);
		}
	}
	protected void DrawCommonProperties2( )
	{
		BeatTween tw = target as BeatTween;
			AnimationCurve curve2 = EditorGUILayout.CurveField("Curve", tw.animationCurve2, GUILayout.Width(170f), GUILayout.Height(62f));
			//UITweener.Method method = (UITweener.Method)EditorGUILayout.EnumPopup("Play Method", tw.method);
			GUILayout.BeginHorizontal();
			float dur2 = EditorGUILayout.FloatField("Duration", tw.duration2, GUILayout.Width(170f));
			GUILayout.Label("seconds");
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			float del2 = EditorGUILayout.FloatField("Start Delay", tw.delay2, GUILayout.Width(170f));
			GUILayout.Label("seconds");
			GUILayout.EndHorizontal();
		if (GUI.changed)
		{
			NGUIEditorTools.RegisterUndo("Tween Change", tw);
			tw.animationCurve2 = curve2;
			tw.duration2 = dur2;
			tw.delay2 = del2;

			//tw.method = method;
			//tw.style = style;
			//tw.ignoreTimeScale = ts;
			
			//tw.tweenGroup = tg;
			//	tw.duration = dur;
			//	tw.delay = del;
			NGUITools.SetDirty(tw);
		}

	}
	protected void DrawCommonProperties3()
	{
		BeatTween tw = target as BeatTween;
			AnimationCurve curve3 = EditorGUILayout.CurveField("Curve", tw.animationCurve3, GUILayout.Width(170f), GUILayout.Height(62f));
			//UITweener.Method method = (UITweener.Method)EditorGUILayout.EnumPopup("Play Method", tw.method);
			GUILayout.BeginHorizontal();
			float dur3 = EditorGUILayout.FloatField("Duration", tw.duration3, GUILayout.Width(170f));
			GUILayout.Label("seconds");
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			float del3 = EditorGUILayout.FloatField("Start Delay", tw.delay3, GUILayout.Width(170f));
			GUILayout.Label("seconds");
			GUILayout.EndHorizontal();
		if (GUI.changed)
		{
			NGUIEditorTools.RegisterUndo("Tween Change", tw);
			tw.animationCurve3 = curve3;
			tw.duration3 = dur3;
			tw.delay3 = del3;
			//tw.method = method;
			//tw.style = style;
			//tw.ignoreTimeScale = ts;
			
			//tw.tweenGroup = tg;
			//	tw.duration = dur;
			//	tw.delay = del;
			NGUITools.SetDirty(tw);
		}
	}
	protected void DrawCommonProperties4()
	{

		BeatTween tw = target as BeatTween;
			AnimationCurve curve4 = EditorGUILayout.CurveField("Curve", tw.animationCurve4, GUILayout.Width(170f), GUILayout.Height(62f));
			//UITweener.Method method = (UITweener.Method)EditorGUILayout.EnumPopup("Play Method", tw.method);
			GUILayout.BeginHorizontal();
			float dur4 = EditorGUILayout.FloatField("Duration", tw.duration4, GUILayout.Width(170f));
			GUILayout.Label("seconds");
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			float del4 = EditorGUILayout.FloatField("Start Delay", tw.delay4, GUILayout.Width(170f));
			GUILayout.Label("seconds");
			GUILayout.EndHorizontal();
		if (GUI.changed)
		{
			NGUIEditorTools.RegisterUndo("Tween Change", tw);

			tw.animationCurve4 = curve4;
			tw.duration4 = dur4;
			tw.delay4 = del4;
			//tw.method = method;
			//tw.style = style;
			//tw.ignoreTimeScale = ts;
			
			//tw.tweenGroup = tg;
			//	tw.duration = dur;
			//	tw.delay = del;
			NGUITools.SetDirty(tw);
		}
	}


	protected void DrawCommonPropertiesBegin ()
	{
	//	BeatTween tw = target as BeatTween;
		//NGUIEditorTools.BeginContents();
		NGUIEditorTools.SetLabelWidth(110f);
		//UITweener.Style style = (UITweener.Style)EditorGUILayout.EnumPopup("Play Style", tw.style);
		
		GUI.changed = false;


	}

	protected void DrawCommonPropertiesEnd ()
	{

		BeatTween tw = target as BeatTween;
		EditorGUILayout.IntField("Tween Group", tw.tweenGroup, GUILayout.Width(170f));
		//bool ts = EditorGUILayout.Toggle("Ignore TimeScale", tw.ignoreTimeScale);
		NGUIEditorTools.SetLabelWidth(80f);
		NGUIEditorTools.DrawEvents("On Finished", tw, tw.onFinished);
		
	}
//		protected void DrawCommonProperties ()
//		{
//			Test tw = target as Test;

		//	tw.m_TweenArr[0]= tw.m_TweenArr[0] as UITweener;

//			UITweener ttt=  target as UITweener;
//			UITweener temp= tw.m_TweenArr222 as UITweener;
//		if (NGUIEditorTools.DrawHeader("Tweener"))
//		{

	//
//			if (NGUIEditorTools.DrawHeader("Tweener"))
//			{
//				NGUIEditorTools.BeginContents();
//				NGUIEditorTools.SetLabelWidth(110f);
//				
//				GUI.changed = false;

			//*******************************************************************
//				UITweener.Style style = (UITweener.Style)EditorGUILayout.EnumPopup("Play Style", tw.style);
//				AnimationCurve curve = EditorGUILayout.CurveField("Animation Curve", tw.animationCurve, GUILayout.Width(170f), GUILayout.Height(62f));
//				//UITweener.Method method = (UITweener.Method)EditorGUILayout.EnumPopup("Play Method", tw.method);
//				
//				GUILayout.BeginHorizontal();
//				float dur = EditorGUILayout.FloatField("Duration", tw.duration, GUILayout.Width(170f));
//				GUILayout.Label("seconds");
//				GUILayout.EndHorizontal();
//				
//				GUILayout.BeginHorizontal();
//				float del = EditorGUILayout.FloatField("Start Delay", tw.delay, GUILayout.Width(170f));
//				GUILayout.Label("seconds");
//				GUILayout.EndHorizontal();

	//		NGUIEditorTools.EndContents();
	//	}
			//*******************************************************************

//			int tg = EditorGUILayout.IntField("Tween Group", tw.tweenGroup, GUILayout.Width(170f));
//			bool ts = EditorGUILayout.Toggle("Ignore TimeScale", tw.ignoreTimeScale);

//				if (GUI.changed)
//				{
//				NGUIEditorTools.RegisterUndo("Tween Change", tw);
//				tw.animationCurve = curve;
//					//tw.method = method;
//			//	tw.m_TweenArr[0].style = style;
//				tw.ignoreTimeScale = ts;
//				tw.tweenGroup = tg;
//				tw.duration = dur;
//				tw.delay = del;
//					NGUITools.SetDirty(tw);
//				}
				
			

//		Test tw2 = target as Test;
//		if (NGUIEditorTools.DrawHeader("Tweener"))
//		{
//			NGUIEditorTools.BeginContents();
//			NGUIEditorTools.SetLabelWidth(110f);
//			
//			GUI.changed = false;
//			
//			UITweener.Style style = (UITweener.Style)EditorGUILayout.EnumPopup("Play Style", tw2.style);
//			AnimationCurve curve = EditorGUILayout.CurveField("Animation Curve", tw2.animationCurve, GUILayout.Width(170f), GUILayout.Height(62f));
//			//UITweener.Method method = (UITweener.Method)EditorGUILayout.EnumPopup("Play Method", tw.method);
//			
//			GUILayout.BeginHorizontal();
//			float dur = EditorGUILayout.FloatField("Duration", tw2.duration, GUILayout.Width(170f));
//			GUILayout.Label("seconds");
//			GUILayout.EndHorizontal();
//			
//			GUILayout.BeginHorizontal();
//			float del = EditorGUILayout.FloatField("Start Delay", tw2.delay, GUILayout.Width(170f));
//			GUILayout.Label("seconds");
//			GUILayout.EndHorizontal();
//			
//			int tg = EditorGUILayout.IntField("Tween Group", tw2.tweenGroup, GUILayout.Width(170f));
//			bool ts = EditorGUILayout.Toggle("Ignore TimeScale", tw2.ignoreTimeScale);
//			
//			if (GUI.changed)
//			{
//				NGUIEditorTools.RegisterUndo("Tween Change", tw2);
//				tw2.animationCurve = curve;
//				//tw.method = method;
//				//	tw.m_TweenArr[0].style = style;
//				tw2.ignoreTimeScale = ts;
//				tw2.tweenGroup = tg;
//				tw2.duration = dur;
//				tw.delay = del;
//				NGUITools.SetDirty(tw);
//			}
//			NGUIEditorTools.EndContents();
//	
//		NGUIEditorTools.SetLabelWidth(80f);
//		NGUIEditorTools.DrawEvents("On Finished", tw, tw.onFinished);
	
	//}
}

