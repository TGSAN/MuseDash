using System;
using UnityEngine;
using UnityEditor;
using qtools.qhierarchy.pcomponent.pbase;
using qtools.qhierarchy.phierarchy;
using qtools.qhierarchy.phelper;
using qtools.qhierarchy.pdata;
using System.Reflection;

namespace qtools.qhierarchy.pcomponent
{
    public class QGameObjectIconComponent: QBaseComponent
    {
        // PRIVATE
        private MethodInfo getIconMethodInfo;
        private object[] getIconMethodParams;
        private Color backgroundColor;

        // CONSTRUCTOR
        public QGameObjectIconComponent ()
        {
            getIconMethodInfo   = typeof(EditorGUIUtility).GetMethod("GetIconForObject", BindingFlags.NonPublic | BindingFlags.Static );
            getIconMethodParams = new object[1];

            backgroundColor = QResources.getInstance().getColor(QColor.Background);

            QSettings.getInstance().addEventListener(QSetting.ShowGameObjectIconComponent   , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowGameObjectIconComponentDuringPlayMode   , settingsChanged);
            settingsChanged();
        }
        
        // PRIVATE
        private void settingsChanged()
        {
            enabled = QSettings.getInstance().get<bool>(QSetting.ShowGameObjectIconComponent);
            showComponentDuringPlayMode = QSettings.getInstance().get<bool>(QSetting.ShowGameObjectIconComponentDuringPlayMode);
        }

        // DRAW
        public override void layout(GameObject gameObject, QObjectList objectList, ref Rect rect)
        {
            rect.x -= 18;
            rect.width = 18;
        }

        public override void draw(GameObject gameObject, QObjectList objectList, Rect selectionRect, Rect rect)
        {                      
            getIconMethodParams[0] = gameObject;
            Texture2D icon = (Texture2D)getIconMethodInfo.Invoke(null, getIconMethodParams );              
            EditorGUI.DrawRect(rect, backgroundColor);
            if (icon != null)
            {
                rect.width = 16;
                GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit, true);
            }
        }
                
        public override void eventHandler(GameObject gameObject, QObjectList objectList, Event currentEvent, Rect curRect)
        {
            if (currentEvent.isMouse && currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && curRect.Contains(currentEvent.mousePosition))
            {
                currentEvent.Use();

                Type iconSelectorType = Assembly.Load("UnityEditor").GetType("UnityEditor.IconSelector");
                MethodInfo showIconSelectorMethodInfo = iconSelectorType.GetMethod("ShowAtPosition", BindingFlags.Static | BindingFlags.NonPublic);
                showIconSelectorMethodInfo.Invoke(null, new object[] { gameObject, curRect, true });
            }
        }
    }
}

