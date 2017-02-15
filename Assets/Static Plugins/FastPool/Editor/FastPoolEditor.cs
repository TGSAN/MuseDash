using UnityEngine;
using UnityEditor;
using Action = System.Action;


[CustomPropertyDrawer(typeof(FastPool))]
public class FastPoolEditor : PropertyDrawer
{


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty customIDProp = property.FindPropertyRelative("customID");
        SerializedProperty useCustomIDProp = property.FindPropertyRelative("useCustomID");

        SerializedProperty sourcePrefabProp = property.FindPropertyRelative("sourcePrefab");
        SerializedProperty notificationTypeProp = property.FindPropertyRelative("NotificationType");
        GameObject sourcePrefab = (GameObject)sourcePrefabProp.objectReferenceValue;
        Rect propRect = position;
        propRect.height = EditorGUIUtility.singleLineHeight;
        

#if UNITY_4_5
        string displayName = "No Source Prefab";
#else
            string displayName = property.displayName;
#endif

        if (Application.isPlaying)
        {
            EditorGUI.LabelField(propRect, string.Concat(sourcePrefab != null ? sourcePrefab.name : displayName, " Pool"), EditorStyles.objectFieldThumb);

            propRect.height = 18;
            propRect.y += EditorGUIUtility.singleLineHeight + 1;

            SerializedProperty spCached = property.FindPropertyRelative("cached_internal");
            SerializedProperty spCapacity = property.FindPropertyRelative("Capacity");

            EditorGUI.ProgressBar(propRect, (float)spCached.intValue / spCapacity.intValue, string.Concat(spCached.intValue.ToString(), "/", spCapacity.intValue > 0 ? spCapacity.intValue.ToString() : "Unlimited"));
        }
        else
        {
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 1;

            Rect bgRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height - EditorGUIUtility.singleLineHeight);

            Rect buttonRect = new Rect(propRect);
            buttonRect.y -= 2;
            buttonRect.height += 4;
            if (GUI.Button(buttonRect, string.Concat((property.isExpanded ? "▼ " : "▶ "), (sourcePrefab != null ? sourcePrefab.name : displayName), (useCustomIDProp.boolValue ? string.Concat(" [Custom ID: ", customIDProp.intValue, "]") : "")), EditorStyles.objectFieldThumb))
                property.isExpanded = !property.isExpanded;

            if (property.isExpanded)
            {
                //Draw background
                EditorGUI.HelpBox(bgRect, "", MessageType.None); 

                //Draw sourcePrefab
                property = sourcePrefabProp;
                propRect.width -= 4;
                propRect.y += 4 + EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(propRect, property);

                //Draw Capacity
                property.NextVisible(false);
                propRect.y += EditorGUIUtility.singleLineHeight + 1;
                if (property.intValue < 0)
                    property.intValue = 0;
                EditorGUI.PropertyField(propRect, property);

                //Draw Preload
                int rightValue = property.intValue;
                property.NextVisible(false);
                propRect.y += EditorGUIUtility.singleLineHeight + 1;
                if (rightValue > 0)
                {
                    if (property.intValue > rightValue)
                        property.intValue = rightValue;
                    EditorGUIUtility.fieldWidth = 30;
                    EditorGUI.IntSlider(propRect, property, 0, rightValue);
                    EditorGUIUtility.fieldWidth = 0;
                }
                else
                {
                    if (property.intValue < 0)
                        property.intValue = 0;
                    EditorGUI.PropertyField(propRect, property);
                }

                //Draw Notification Type and UseSceneClone
                for (int i = 0; i < 3; i++)
                {
                    property.NextVisible(false);
                    propRect.y += EditorGUIUtility.singleLineHeight + 1;
                    EditorGUI.PropertyField(propRect, property);
                }

                //Draw Custom ID stuff
                property.NextVisible(false);
                propRect.y += EditorGUIUtility.singleLineHeight + 1;
                Rect customIDRect = new Rect(propRect);
                customIDRect.width -= 70;
                EditorGUI.PropertyField(customIDRect, property);
                if (property.boolValue)
                {
                    property.Next(false);
                    customIDRect.x += customIDRect.width;
                    customIDRect.width = 70; 
                    EditorGUIUtility.labelWidth = 20;
                    EditorGUI.indentLevel = 0;
                    EditorGUI.PropertyField(customIDRect, property, new GUIContent("ID", "Custom pool ID. By default it equals to the InstanceID of the source prefab."));
                    EditorGUI.indentLevel = 1;
                    EditorGUIUtility.labelWidth = 0;
                }

                
                //Draw Auto Despawn stuff
                propRect = EditorGUI.IndentedRect(propRect);
                propRect.y += EditorGUIUtility.singleLineHeight + 7;

                bgRect = new Rect(propRect);
                bgRect.y += EditorGUIUtility.singleLineHeight;
                bgRect.height = despawnerHeight - EditorGUIUtility.singleLineHeight * 2 + 9;

                GUI.Label(propRect, "Despawner Conditions [OR]", EditorStyles.objectFieldThumb);

                if (sourcePrefab == null)
                {
                    EditorGUI.HelpBox(bgRect, "Source Prefab is null", MessageType.Info);
                }
                else
                {
                    EditorGUI.HelpBox(bgRect, "", MessageType.None);
                    FPUniversalDespawner despawner = sourcePrefab.GetComponent<FPUniversalDespawner>();
                    if (despawner == null)
                    {
                        propRect.x += 10;
                        propRect.width -= 20;
                        propRect.y += EditorGUIUtility.singleLineHeight + 1 + 3;
                        if (GUI.Button(propRect, "Add Auto Despawner", EditorStyles.miniButton))
                        {
                            FPUniversalDespawner dsp = sourcePrefab.AddComponent<FPUniversalDespawner>();
                            useCustomIDProp.boolValue = true;
                            dsp.TargetPoolID = customIDProp.intValue;

                            PoolItemNotificationType notificationType = (PoolItemNotificationType)notificationTypeProp.enumValueIndex;
                            if (notificationType == PoolItemNotificationType.None || notificationType == PoolItemNotificationType.Interface)
                                notificationTypeProp.enumValueIndex = (int)PoolItemNotificationType.SendMessage;
                        }
                        
                    }
                    else
                    {
                        if (notificationTypeProp.enumValueIndex == 0)
                            notificationTypeProp.enumValueIndex = 2;

                        if (!useCustomIDProp.boolValue)
                            useCustomIDProp.boolValue = true;

                        if (customIDProp.intValue != despawner.TargetPoolID)
                            despawner.TargetPoolID = customIDProp.intValue;

                        if (despawner.DespawnDelayed)
                        {
                            propRect.y += EditorGUIUtility.singleLineHeight + 1;
                            EditorGUI.LabelField(propRect, "Despawn after", despawner.Delay + " seconds");
                        }
                        if (despawner.DespawnOnParticlesDead)
                        {
                            propRect.y += EditorGUIUtility.singleLineHeight + 1;
                            EditorGUI.LabelField(propRect, "Particles is dead");
                        }
                        if (despawner.DespawnOnAudioSourceStop)
                        {
                            propRect.y += EditorGUIUtility.singleLineHeight + 1;
                            EditorGUI.LabelField(propRect, "AudioSource stops playing");
                        }

                        propRect.y += EditorGUIUtility.singleLineHeight + 3;
                        propRect.x += 2;
                        propRect.width -= 4;
                        propRect.width = propRect.width / 2;
                        if (GUI.Button(propRect, "Configure", EditorStyles.miniButtonLeft))
                        {
                            Selection.activeObject = sourcePrefab;
                            EditorGUIUtility.PingObject(sourcePrefab);
                        }

                        GUI.backgroundColor = Color.red;
                        propRect.x += propRect.width;
                        if (GUI.Button(propRect, "Remove", EditorStyles.miniButtonRight))
                            GameObject.DestroyImmediate(despawner, true);

                        GUI.backgroundColor = Color.white;
                    }
                }

            }

            EditorGUI.indentLevel = indent;
        }

        EditorGUI.EndProperty();
    }

    float despawnerHeight;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        despawnerHeight = EditorGUIUtility.singleLineHeight * 2;

        GameObject sourcePrefab = (GameObject)property.FindPropertyRelative("sourcePrefab").objectReferenceValue;
        if (sourcePrefab != null)
        {
            FPUniversalDespawner despawner = sourcePrefab.GetComponent<FPUniversalDespawner>();
            if (despawner != null)
            {
                if (despawner.DespawnDelayed)
                    despawnerHeight += EditorGUIUtility.singleLineHeight;
                if (despawner.DespawnOnParticlesDead)
                    despawnerHeight += EditorGUIUtility.singleLineHeight;
                if (despawner.DespawnOnAudioSourceStop)
                    despawnerHeight += EditorGUIUtility.singleLineHeight;

                despawnerHeight += EditorGUIUtility.singleLineHeight;
            }
            else
                despawnerHeight += EditorGUIUtility.singleLineHeight;
        }
        else
            despawnerHeight += EditorGUIUtility.singleLineHeight;

        if (Application.isPlaying)
            return (EditorGUIUtility.singleLineHeight + (property.isExpanded ? EditorGUIUtility.standardVerticalSpacing : 0)) * 2;
        else
            return (property.isExpanded ? despawnerHeight : 0) + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * property.CountInProperty();
    }


    void DrawFold(SerializedProperty property, string caption, Rect buttonRect, Rect contentRect, Action drawContent)
    {
        if (GUI.Button(buttonRect, caption, EditorStyles.objectFieldThumb))
            property.isExpanded = !property.isExpanded;

        if (property.isExpanded)
        {
            //Draw background
            EditorGUI.HelpBox(contentRect, "", MessageType.None);

            drawContent();
        }
    }
}
