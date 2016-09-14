
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Beat))]
public class BeatEdit: BeatTweenEdit
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(120f);
		
		Beat tw = target as Beat;
		GUI.changed = false;

		GUILayout.BeginHorizontal();
	//	float dur2 = EditorGUILayout.FloatField("Duration", tw.duration2, GUILayout.Width(170f));
		GUILayout.Label("先勾选模式 在勾选脚本即可运行 只做调参数用");
		GUILayout.EndHorizontal();


	//	Vector3 from = EditorGUILayout.Vector3Field("From", tw.from);
	//	Vector3 to = EditorGUILayout.Vector3Field("To", tw.to);
	//	bool table = EditorGUILayout.Toggle("Update Table", tw.updateTable);
		//bool bplay = EditorGUILayout.Toggle("play", tw.m_piay);
		bool temp = EditorGUILayout.Toggle("energy", tw.m_energy);
		bool temp2 = EditorGUILayout.Toggle("hithat", tw.m_hithat);
		bool temp3 = EditorGUILayout.Toggle("kick", tw.m_kick);
		bool temp4 = EditorGUILayout.Toggle("snare", tw.m_snare);


		Vector3	from1=Vector3.one;
		Vector3	to1=Vector3.one;
		Vector3	from2=Vector3.one;
		Vector3	to2=Vector3.one;
		Vector3	from3=Vector3.one;
		Vector3	to3=Vector3.one;
		Vector3	from4=Vector3.one;
		Vector3	to4=Vector3.one;
		if (GUI.changed)
		{
			Debug.Log("changge");
			NGUIEditorTools.RegisterUndo("Tween Change", tw);
			tw.m_energy=temp;
			tw.m_hithat=temp2;
			tw.m_kick=temp3;
			tw.m_snare=temp4;
			NGUITools.SetDirty(tw);
		}
		if (NGUIEditorTools.DrawHeader("Tweener"))
		{
			NGUIEditorTools.BeginContents();
			NGUIEditorTools.SetLabelWidth(110f);
			DrawCommonPropertiesBegin();
			if (NGUIEditorTools.DrawHeader("energy"))
			{
				NGUIEditorTools.BeginContents();
				NGUIEditorTools.SetLabelWidth(110f);
				from1 = EditorGUILayout.Vector3Field("energy_From", tw.from1);
				to1 = EditorGUILayout.Vector3Field("energy_To", tw.to1);
			
				if (GUI.changed)
				{
					NGUIEditorTools.RegisterUndo("Tween Change", tw);

					tw.from1=from1;
					tw.to1=to1;
								tw.m_energy=temp;
				tw.m_hithat=temp2;
				tw.m_kick=temp3;
				tw.m_snare=temp4;
					NGUITools.SetDirty(tw);
				}
				DrawCommonProperties1();
				NGUIEditorTools.EndContents();
			}

			if (NGUIEditorTools.DrawHeader("hithat"))
			{
				NGUIEditorTools.BeginContents();
				NGUIEditorTools.SetLabelWidth(110f);
				from2 = EditorGUILayout.Vector3Field("hithat_From", tw.from2);
				to2 = EditorGUILayout.Vector3Field("hithat_To", tw.to2);

				if (GUI.changed)
				{
					NGUIEditorTools.RegisterUndo("Tween Change", tw);

					tw.from2=from2;
					tw.to2=to2;
					tw.m_energy=temp;
					tw.m_hithat=temp2;
					tw.m_kick=temp3;
					tw.m_snare=temp4;
					NGUITools.SetDirty(tw);
				}
				DrawCommonProperties2();
				NGUIEditorTools.EndContents();
			}
			if (NGUIEditorTools.DrawHeader("kick"))
			{
				NGUIEditorTools.BeginContents();
				NGUIEditorTools.SetLabelWidth(110f);
					from3 = EditorGUILayout.Vector3Field("kick_From", tw.from3);
					to3 = EditorGUILayout.Vector3Field("kick_To", tw.to3);

				if (GUI.changed)
				{
					NGUIEditorTools.RegisterUndo("Tween Change", tw);

					tw.from3=from3;
					tw.to3=to3;
					tw.m_energy=temp;
					tw.m_hithat=temp2;
					tw.m_kick=temp3;
					tw.m_snare=temp4;
					NGUITools.SetDirty(tw);
				}
				DrawCommonProperties3();
				NGUIEditorTools.EndContents();
			}
			if (NGUIEditorTools.DrawHeader("snare"))
			{
				NGUIEditorTools.BeginContents();
				NGUIEditorTools.SetLabelWidth(110f);
					from4 = EditorGUILayout.Vector3Field("snare_From", tw.from4);
					to4 = EditorGUILayout.Vector3Field("snare_To", tw.to4);

				if (GUI.changed)
				{
					NGUIEditorTools.RegisterUndo("Tween Change", tw);
					tw.from4=from4;
					tw.to4=to4;
					tw.m_energy=temp;
					tw.m_hithat=temp2;
					tw.m_kick=temp3;
					tw.m_snare=temp4;
					NGUITools.SetDirty(tw);
				}
				DrawCommonProperties4();
				NGUIEditorTools.EndContents();
			}
			DrawCommonPropertiesEnd();
			NGUIEditorTools.EndContents();
		}

		//DrawCommonProperties();
	
	}

}
