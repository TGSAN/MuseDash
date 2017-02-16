using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using qtools.qhierarchy.pcomponent.pbase;
using qtools.qhierarchy.pdata;
using qtools.qhierarchy.phelper;

namespace qtools.qhierarchy.pcomponent
{
    public class QComponentsComponent: QBaseComponent
    {
        // PRIVATE
        private GUIStyle hintLabelStyle;
        private Color grayColor;
        private Color backgroundColor;
        private Color backgroundDarkColor;
        private Texture2D componentIcon;
        private Component[] components;        

        // CONSTRUCTOR
        public QComponentsComponent ()
        {
            this.backgroundColor     = QResources.getInstance().getColor(QColor.Background);
            this.backgroundDarkColor = QResources.getInstance().getColor(QColor.BackgroundDark);
            this.grayColor           = QResources.getInstance().getColor(QColor.Gray);
            this.componentIcon       = QResources.getInstance().getTexture(QTexture.QComponentUnknownIcon);

            hintLabelStyle = new GUIStyle();
            hintLabelStyle.normal.textColor = grayColor;
            hintLabelStyle.fontSize = 11;
            hintLabelStyle.clipping = TextClipping.Clip;  

            QSettings.getInstance().addEventListener(QSetting.ShowComponentsComponent              , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowComponentsComponentDuringPlayMode, settingsChanged);
            settingsChanged();
        }

        // PRIVATE
        private void settingsChanged()
        {
            enabled = QSettings.getInstance().get<bool>(QSetting.ShowComponentsComponent);
            showComponentDuringPlayMode = QSettings.getInstance().get<bool>(QSetting.ShowComponentsComponentDuringPlayMode);
        }

        // DRAW
        public override void layout(GameObject gameObject, QObjectList objectList, ref Rect curRect)
        {
            components = gameObject.GetComponents<Component>();
            int totalWidth = 2 + 16 * (components.Length - 1);
            curRect.x -= totalWidth;
            curRect.width = totalWidth;
        }

        public override void draw(GameObject gameObject, QObjectList objectList, Rect selectionRect, Rect curRect)
        {
            EditorGUI.DrawRect(curRect, backgroundColor);
            curRect.width = 16;

            for (int i = 0, n = components.Length; i < n; i++)
            {
                Component component = components[i];
                if (component is Transform) continue;
                                
                GUIContent content = EditorGUIUtility.ObjectContent(component, null);

                bool enabled = true;
                try
                {
                    PropertyInfo propertyInfo = component.GetType().GetProperty("enabled");
                    enabled = (bool)propertyInfo.GetGetMethod().Invoke(component, null);
                }
                catch {}

                Color color = GUI.color;
                color.a = enabled ? 1f : 0.3f;
                GUI.color = color;
                GUI.DrawTexture(curRect, content.image == null ? componentIcon : content.image);
                color.a = 1;
                GUI.color = color;

                if (curRect.Contains(Event.current.mousePosition))
                {        
                    string componentName = "Missing script";
                    if (component != null) componentName = component.GetType().Name;

                    int labelWidth = Mathf.CeilToInt(hintLabelStyle.CalcSize(new GUIContent(componentName)).x);                    
                    selectionRect.x = curRect.x - labelWidth / 2 - 4;
                    selectionRect.width = labelWidth + 8;
                    selectionRect.height -= 1;

                    if (selectionRect.y > 16) selectionRect.y -= 16;
                    else selectionRect.x += labelWidth;

                    EditorGUI.DrawRect(selectionRect, backgroundColor);

                    EditorGUI.DrawRect(selectionRect, backgroundDarkColor);
                    selectionRect.x += 4;
                    selectionRect.y += 1;

                    GUI.Label(selectionRect, componentName, hintLabelStyle);
                }

                curRect.x += 16;
            }
        }

        public override void eventHandler(GameObject gameObject, QObjectList objectList, Event currentEvent, Rect curRect)
        {
            if (currentEvent.isMouse && currentEvent.button == 0 && curRect.Contains(currentEvent.mousePosition))
            {
                if (currentEvent.type == EventType.MouseDown)
                {
                    int id = Mathf.FloorToInt((currentEvent.mousePosition.x - curRect.x) / 16) + 1;

                    try
                    {
                        PropertyInfo propertyInfo = components[id].GetType().GetProperty("enabled");
                        bool enabled = (bool)propertyInfo.GetGetMethod().Invoke(components[id], null);
                        Undo.RecordObject(components[id], enabled ? "Disable Component" : "Enable Component");
                        propertyInfo.GetSetMethod().Invoke(components[id], new object[] { !enabled });
                    }
                    catch {}

                    EditorUtility.SetDirty(gameObject);
                }
                currentEvent.Use();
            }
        }
    }
}

