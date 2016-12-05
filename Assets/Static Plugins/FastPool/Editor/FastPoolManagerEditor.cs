using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


[CustomEditor(typeof(FastPoolManager))]
public class FastPoolManagerEditor : Editor
{
    SerializedProperty pools;
    SerializedProperty randomCustomPoolID;
    FastPoolManager fpm;
    bool curElementIsNotDeleted;
    GUIStyle gs;

    void OnEnable()
    {
        pools = serializedObject.FindProperty("predefinedPools");
        fpm = (FastPoolManager)target;
    } 


    public override void OnInspectorGUI()
    {
        GUILayout.Space(5);

        if (EditorApplication.isPlaying)
        {
            foreach (KeyValuePair<int, FastPool> pool in fpm.Pools)
            {
                if (pool.Value.IsValid)
                {
                    EditorGUILayout.LabelField(string.Format("{0} [ID {1}]", pool.Value.Name, pool.Value.ID), EditorStyles.objectFieldThumb);

                    Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
                    EditorGUI.ProgressBar(rect, (float)pool.Value.Cached / pool.Value.Capacity, string.Concat(pool.Value.Cached.ToString(), "/",  pool.Value.Capacity > 0 ? pool.Value.Capacity.ToString() : "Unlimited"));
                    EditorGUILayout.Space();
                }
            }
        }
        else
        {
            serializedObject.Update();
            EditorGUILayout.Space();


            gs = new GUIStyle("box");
            gs.alignment = TextAnchor.MiddleCenter;
            gs.normal.textColor = Color.gray;
            GUILayout.Label("Drag and Drop prefab in this box \r\nor press the button below", gs, GUILayout.Height(50), GUILayout.ExpandWidth(true));
            
            EventType eventType = Event.current.type;
            if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
            {
                bool dragOk = true;

                //Check dragged objects type
                for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                    if (DragAndDrop.objectReferences[i].GetType() != typeof(GameObject))
                        dragOk = false;

                //Check mouse position
                if (!GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    dragOk = false;

                // Show a copy icon on the drag
                if (dragOk)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (eventType == EventType.DragPerform)
                    {
                        for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                        {
                            pools.InsertArrayElementAtIndex(pools.arraySize);
                            pools.GetArrayElementAtIndex(pools.arraySize - 1).FindPropertyRelative("sourcePrefab").objectReferenceValue = DragAndDrop.objectReferences[i];
                            pools.GetArrayElementAtIndex(pools.arraySize - 1).FindPropertyRelative("customID").intValue = GetRandomCustomPoolID();
                        }
                        DragAndDrop.AcceptDrag();
                    }

                    Event.current.Use();
                }
            }


            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Add New Pool"))
            {
                pools.InsertArrayElementAtIndex(pools.arraySize);
                pools.GetArrayElementAtIndex(pools.arraySize - 1).FindPropertyRelative("customID").intValue = GetRandomCustomPoolID();
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.Space();


            //iterate pools
            for (int i = 0; i < pools.arraySize; i++)
            {
                curElementIsNotDeleted = true;
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.PropertyField(pools.GetArrayElementAtIndex(i), false);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUILayout.Width(20));
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    pools.DeleteArrayElementAtIndex(i);
                    curElementIsNotDeleted = false;
                }

                if (curElementIsNotDeleted && pools.GetArrayElementAtIndex(i).isExpanded)
                {
                    GUI.backgroundColor = Color.gray;
                    GUILayout.Space(20);
                    if (i > 0)
                    {
                        if (GUILayout.Button("↑", GUILayout.Width(20), GUILayout.Height(30)))
                            pools.MoveArrayElement(i, i - 1);
                    }
                    else
                        GUILayout.Space(30 + EditorGUIUtility.standardVerticalSpacing * 2);

                    if (i < pools.arraySize - 1 && GUILayout.Button("↓", GUILayout.Width(20), GUILayout.Height(30)))
                        pools.MoveArrayElement(i, i + 1);
                }

                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
                
                GUILayout.Space(5);
            }

            GUILayout.Space(5);

            serializedObject.ApplyModifiedProperties();
        }
    }

    int GetRandomCustomPoolID()
    {
        int randomID;
        bool idExists;
        do
        {
            idExists = false;
            randomID = Random.Range(1, 10000);

            for (int i = 0; i < pools.arraySize; i++)
            {
                if (pools.GetArrayElementAtIndex(i).FindPropertyRelative("customID").intValue == randomID)
                {
                    idExists = true;
                    break;
                }
            }
        } while (idExists);

        return randomID;
    }
}

