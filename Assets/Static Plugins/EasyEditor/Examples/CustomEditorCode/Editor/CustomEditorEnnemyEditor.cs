using UnityEditor;
using UnityEngine;
using System.Collections;

namespace EasyEditor
{
    /// <summary>
    /// To let EE knows that he has to handle a function in the editor not as a button in the inspector
    /// but as editor script it needs to render, you need to specify the parameter 'rendererType' 
    /// of Inspector attribute as "CustomRenderer".
    /// </summary>
    [Groups("Game Designer Settings", "Basic Settings", "Advanced Settings")]
    [CustomEditor(typeof(CustomEditorEnnemy))]
    public class CustomEditorEnnemyEditor : EasyEditorBase
    {
        [Inspector(group = "Advanced Settings", rendererType = "CustomRenderer", order = 1)]
        [Comment("When the Armor reach zero, the Life drop twice faster.")]
        private void RenderProgressBars()
        {
    		RenderLifeBar ();

            GUILayout.Space(5);

    		RenderArmorBar ();
        }

    	private void RenderLifeBar()
    	{
    		Rect r = EditorGUILayout.BeginVertical();
    		EditorGUI.ProgressBar(r, 0.8f, "Life");
    		GUILayout.Space(18);
    		EditorGUILayout.EndVertical();
    	}

    	private void RenderArmorBar()
    	{
    		Rect r = EditorGUILayout.BeginVertical();
    		EditorGUI.ProgressBar(r, 0.4f, "Armor");
    		GUILayout.Space(18);
    		EditorGUILayout.EndVertical();
    	}
    }
}