using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using qtools.qhierarchy.pcomponent.pbase;
using qtools.qhierarchy.phierarchy;
using qtools.qhierarchy.phelper;
using qtools.qhierarchy.pdata;
using System.Reflection;
using System.Collections;
using UnityEditorInternal;
using System.Text;

namespace qtools.qhierarchy.pcomponent
{
    public class QErrorComponent: QBaseComponent
    {
        // PRIVATE
        private Texture2D errorIconTexture;
        private Texture2D errorChildIconTexture;
        private Color backgroundColor;
        private bool showErrorOfChildren;
        private bool showErrorTypeReferenceIsNull;
        private bool showErrorTypeStringIsEmpty;
        private bool showErrorIconScriptIsMissing;
        private bool showErrorIconWhenTagIsUndefined;
        private bool showErrorForDisabledComponents;
        private bool showErrorIconMissingEventMethod;
        private List<string> ignoreErrorOfMonoBehaviours;
        private StringBuilder errorStringBuilder;
        private int errorCount;

        // CONSTRUCTOR
        public QErrorComponent ()
        {
            errorIconTexture = QResources.getInstance().getTexture(QTexture.QErrorIcon);
            errorChildIconTexture = QResources.getInstance().getTexture(QTexture.QErrorChildIcon);
            backgroundColor = QResources.getInstance().getColor(QColor.Background);

            QSettings.getInstance().addEventListener(QSetting.ShowErrorIconParent             , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowErrorIconReferenceIsNull    , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowErrorIconStringIsEmpty      , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowErrorIconScriptIsMissing    , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowErrorForDisabledComponents  , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowErrorIconMissingEventMethod , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowErrorIconWhenTagOrLayerIsUndefined , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowErrorComponent              , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowErrorComponentDuringPlayMode, settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.IgnoreErrorOfMonoBehaviours     , settingsChanged);
            settingsChanged();
        }

        // PRIVATE
        private void settingsChanged()
        {
            showErrorOfChildren = QSettings.getInstance().get<bool>(QSetting.ShowErrorIconParent);
            showErrorTypeReferenceIsNull = QSettings.getInstance().get<bool>(QSetting.ShowErrorIconReferenceIsNull);
            showErrorTypeStringIsEmpty = QSettings.getInstance().get<bool>(QSetting.ShowErrorIconStringIsEmpty);
            showErrorIconScriptIsMissing = QSettings.getInstance().get<bool>(QSetting.ShowErrorIconScriptIsMissing);
            showErrorForDisabledComponents = QSettings.getInstance().get<bool>(QSetting.ShowErrorForDisabledComponents);
            showErrorIconMissingEventMethod = QSettings.getInstance().get<bool>(QSetting.ShowErrorIconMissingEventMethod);
            showErrorIconWhenTagIsUndefined = QSettings.getInstance().get<bool>(QSetting.ShowErrorIconWhenTagOrLayerIsUndefined);
            enabled = QSettings.getInstance().get<bool>(QSetting.ShowErrorComponent);
            showComponentDuringPlayMode = QSettings.getInstance().get<bool>(QSetting.ShowErrorComponentDuringPlayMode);
            string ignoreErrorOfMonoBehavioursString = QSettings.getInstance().get<string>(QSetting.IgnoreErrorOfMonoBehaviours);
            if (ignoreErrorOfMonoBehavioursString != "") 
            {
                ignoreErrorOfMonoBehaviours = new List<string>(ignoreErrorOfMonoBehavioursString.Split(new char[] { ',', ';', '.', ' ' }));
                ignoreErrorOfMonoBehaviours.RemoveAll(item => item == "");
            }
            else ignoreErrorOfMonoBehaviours = null;
        }

        // DRAW
        public override void layout(GameObject gameObject, QObjectList objectList, ref Rect rect)
        {
            rect.x -= 16;
            rect.width = 16;
        }

        public override void draw(GameObject gameObject, QObjectList objectList, Rect selectionRect, Rect curRect)
        {
            bool errorFound = findError(gameObject, gameObject.GetComponents<MonoBehaviour>());

            EditorGUI.DrawRect(curRect, backgroundColor);
            if (errorFound)
            {           
                GUI.DrawTexture(curRect, errorIconTexture);
            }
            else if (showErrorOfChildren) 
            {
                errorFound = findError(gameObject, gameObject.GetComponentsInChildren<MonoBehaviour>(true));
                if (errorFound) GUI.DrawTexture(curRect, errorChildIconTexture);
            }            
        }

        public override void eventHandler(GameObject gameObject, QObjectList objectList, Event currentEvent, Rect curRect)
        {
            if (currentEvent.isMouse && currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && curRect.Contains(currentEvent.mousePosition))
            {
                currentEvent.Use();

                errorCount = 0;
                errorStringBuilder = new StringBuilder();
                findError(gameObject, gameObject.GetComponents<MonoBehaviour>(), true);

                if (errorCount > 0)
                {
                    EditorUtility.DisplayDialog(errorCount + (errorCount == 1 ? " error was found" : " errors were found"), errorStringBuilder.ToString(), "OK");
                }
            }
        }

        // PRIVATE
        private bool findError(GameObject gameObject, MonoBehaviour[] components, bool printError = false)
        {
            if (showErrorIconWhenTagIsUndefined)
            {
                try
                { 
                    gameObject.tag.CompareTo(null); 
                }
                catch 
                {
                    if (printError)
                    {
                        appendErrorLine("Tag is undefined");
                    }
                    else
                    {
                        return true;
                    }
                }

                if (LayerMask.LayerToName(gameObject.layer).Equals("")) 
                {
                    if (printError)
                    {
                        appendErrorLine("Layer is undefined");
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            for (int i = 0; i < components.Length; i++)
            {
                MonoBehaviour monoBehaviour = components[i];
                if (monoBehaviour == null)
                {
                    if (showErrorIconScriptIsMissing)
                    {
                        if (printError)
                        {
                            appendErrorLine("Component #" + i + " is missing");
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (ignoreErrorOfMonoBehaviours != null)
                    {
                        for (int j = ignoreErrorOfMonoBehaviours.Count - 1; j >= 0; j--)
                        {
                            if (monoBehaviour.GetType().FullName.Contains(ignoreErrorOfMonoBehaviours[j]))
                            {
                                return false;
                            }
                        }
                    }

                    if (showErrorIconMissingEventMethod)
                    {
                        if (monoBehaviour.gameObject.activeSelf || showErrorForDisabledComponents)
                        {
                            try
                            {
                                if (isUnityEventsNullOrMissing(monoBehaviour, printError))
                                {
                                    return true;
                                }
                            }
                            catch
                            {
                            }
                        }
                    }

                    if (showErrorTypeReferenceIsNull || showErrorTypeStringIsEmpty)
                    {                       
                        if (!(monoBehaviour.enabled && monoBehaviour.gameObject.activeSelf) && !showErrorForDisabledComponents) continue;
                        
                        FieldInfo[] fieldArray = monoBehaviour.GetType().GetFields();
                        for (int j = 0; j < fieldArray.Length; j++)
                        {
                            FieldInfo field = fieldArray[j];
                            
                            try
                            {
                                if (System.Attribute.IsDefined(field, typeof(HideInInspector)) || field.IsStatic) continue;
                                
                                object value = field.GetValue(monoBehaviour);
                                
                                if (showErrorTypeReferenceIsNull && (value == null || value.Equals(null)))
                                {           
                                    if (printError)
                                    {
                                        appendErrorLine(monoBehaviour.GetType().Name + "." + field.Name + ": Reference is null");
                                    }
                                    else
                                    {
                                        return true;
                                    }
                                }
                                else if (field.FieldType == typeof(string))
                                {                                
                                    if (showErrorTypeStringIsEmpty && value != null && ((string)value).Equals(""))
                                    {
                                        if (printError)
                                        {
                                            appendErrorLine(monoBehaviour.GetType().Name + "." + field.Name + ": String value is empty");
                                        }
                                        else
                                        {
                                            return true;                                 
                                        }
                                    }
                                }
                                else 
                                {
                                    if (showErrorTypeReferenceIsNull && (value is IEnumerable))
                                    {
                                        foreach (var item in (IEnumerable)value)
                                        {
                                            if (item == null)
                                            {
                                                if (printError)
                                                {
                                                    appendErrorLine(monoBehaviour.GetType().Name + "." + field.Name + ": IEnumerable has value with null reference");
                                                }
                                                else
                                                {
                                                    return true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            return false;
        }

        private List<string> targetPropertiesNames = new List<string>(10);
        
        private bool isUnityEventsNullOrMissing(MonoBehaviour monoBehaviour, bool printError) 
        {
            targetPropertiesNames.Clear();
            FieldInfo[] fieldArray = monoBehaviour.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance); 
   
            for (int i = fieldArray.Length - 1; i >= 0; i--) 
            {
                FieldInfo field = fieldArray[i];                    
                if (field.FieldType == typeof(UnityEvent) || field.FieldType.IsSubclassOf(typeof(UnityEvent))) 
                {
                    targetPropertiesNames.Add(field.Name);
                }
            }
            
            if (targetPropertiesNames.Count > 0) 
            {
                SerializedObject serializedMonoBehaviour = new SerializedObject(monoBehaviour); 
                for (int i = targetPropertiesNames.Count - 1; i >= 0; i--) 
                {
                    string targetProperty = targetPropertiesNames[i];

                    SerializedProperty property = serializedMonoBehaviour.FindProperty(targetProperty);
                    SerializedProperty propertyRelativeArrray = property.FindPropertyRelative("m_PersistentCalls.m_Calls");
                    
                    for (int j = propertyRelativeArrray.arraySize - 1; j >= 0; j--)
                    {
                        SerializedProperty arrayElementAtIndex = propertyRelativeArrray.GetArrayElementAtIndex(j);

                        SerializedProperty propertyTarget       = arrayElementAtIndex.FindPropertyRelative("m_Target");
                        if (propertyTarget.objectReferenceValue == null)
                        {
                            if (printError)
                            {
                                appendErrorLine(monoBehaviour.GetType().Name + ": Event object reference is null");
                            }
                            else
                            {
                                return true;
                            }
                        }

                        SerializedProperty propertyMethodName   = arrayElementAtIndex.FindPropertyRelative("m_MethodName");
                        if (string.IsNullOrEmpty(propertyMethodName.stringValue)) 
                        {
                            if (printError)
                            {
                                appendErrorLine(monoBehaviour.GetType().Name + ": Event handler function is not selected");
                                continue;
                            }
                            else
                            {
                                return true;
                            }
                        }
                         
                        string argumentAssemblyTypeName = arrayElementAtIndex.FindPropertyRelative("m_Arguments").FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue;
                        System.Type argumentAssemblyType;
                        if (!string.IsNullOrEmpty(argumentAssemblyTypeName)) argumentAssemblyType = System.Type.GetType(argumentAssemblyTypeName, false) ?? typeof(UnityEngine.Object);
                        else argumentAssemblyType = typeof(UnityEngine.Object);

                        UnityEventBase dummyEvent;
                        System.Type propertyTypeName = System.Type.GetType(property.FindPropertyRelative("m_TypeName").stringValue, false);
                        if (propertyTypeName == null) dummyEvent = (UnityEventBase) new UnityEvent();
                        else dummyEvent = Activator.CreateInstance(propertyTypeName) as UnityEventBase;

                        if (!UnityEventDrawer.IsPersistantListenerValid(dummyEvent, propertyMethodName.stringValue, propertyTarget.objectReferenceValue, (PersistentListenerMode)arrayElementAtIndex.FindPropertyRelative("m_Mode").enumValueIndex, argumentAssemblyType))
                        { 
                            if (printError)
                            {
                                appendErrorLine(monoBehaviour.GetType().Name + ": Event handler function is missing");
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }   
            }            
            return false;
        }

        private void appendErrorLine(string error)
        {
            errorCount++;
            errorStringBuilder.Append(errorCount.ToString());
            errorStringBuilder.Append(") ");
            errorStringBuilder.AppendLine(error);
        }
    }
}

