using UnityEditor;
using UnityEngine;
using System.Collections;
using EasyEditor;

[Groups("")]
[CustomEditor(typeof(EnemyListenListEvent))]
public class EnemyListenListEventEditor : EasyEditorBase
{
    new public void OnEnable()
    {
        base.OnEnable();
        
        ReorderableListRenderer reorderableList = (ReorderableListRenderer) LookForRenderer("listOfTarget");
        reorderableList.OnItemInserted += HandleOnItemInserted;
        reorderableList.OnItemBeingRemoved += HandleOnItemBeingRemoved;
    }
    
    void HandleOnItemInserted(int index, SerializedProperty list)
    {
        list.GetArrayElementAtIndex(index).boundsValue = new Bounds(Vector3.one, Vector3.zero);
    }
    
    void HandleOnItemBeingRemoved(int index, SerializedProperty list)
    {
        Debug.Log("Bounds being removed\n" + list.GetArrayElementAtIndex(index).boundsValue);
    }
}