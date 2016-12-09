using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using qtools.qhierarchy.pcomponent.pbase;
using qtools.qhierarchy.phierarchy;
using qtools.qhierarchy.phelper;
using qtools.qhierarchy.pdata;

namespace qtools.qhierarchy.pcomponent
{
    public class QChildrenCountComponent: QBaseComponent
    {
        // PRIVATE
        private Color backgroundColor;
        private GUIStyle labelStyle;
        private int childrenCount;

        // CONSTRUCTOR
        public QChildrenCountComponent ()
        {
            backgroundColor = QResources.getInstance().getColor(QColor.Background);

            labelStyle = new GUIStyle();
            labelStyle.normal.textColor = QResources.getInstance().getColor(QColor.Gray);
            labelStyle.fontSize = 9;
            labelStyle.clipping = TextClipping.Clip;  
            labelStyle.alignment = TextAnchor.MiddleRight;

            QSettings.getInstance().addEventListener(QSetting.ShowChildrenCountComponent              , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowChildrenCountComponentDuringPlayMode, settingsChanged);
            settingsChanged();
        }

        // PRIVATE
        private void settingsChanged()
        {
            enabled = QSettings.getInstance().get<bool>(QSetting.ShowChildrenCountComponent);
            showComponentDuringPlayMode = QSettings.getInstance().get<bool>(QSetting.ShowChildrenCountComponentDuringPlayMode);
        }

        // DRAW
        public override void layout(GameObject gameObject, QObjectList objectList, ref Rect rect)
        {
            childrenCount = gameObject.transform.childCount;
            rect.x -= 22;
            rect.width = 22;
        }
        
        public override void draw(GameObject gameObject, QObjectList objectList, Rect selectionRect, Rect curRect)
        {  
            EditorGUI.DrawRect(curRect, backgroundColor);
            curRect.x -= 2;
            if (childrenCount > 0) GUI.Label(curRect, childrenCount.ToString(), labelStyle);
        }
    }
}

