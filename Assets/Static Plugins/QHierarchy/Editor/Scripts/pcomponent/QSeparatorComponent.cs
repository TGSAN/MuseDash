using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using qtools.qhierarchy.pcomponent.pbase;
using qtools.qhierarchy.pdata;
using qtools.qhierarchy.phelper;

namespace qtools.qhierarchy.pcomponent
{
    public class QSeparatorComponent: QBaseComponent
    {
        // PRIVATE
        private Color separatorColor;
        private Color shadingColor;
        private bool showRowShading;

        // CONSTRUCTOR
        public QSeparatorComponent ()
        {
            separatorColor = new Color(0f, 0f, 0f, 0.15f);
            shadingColor = new Color(0f, 0f, 0f, 0.05f);

            showComponentDuringPlayMode = true;

            QSettings.getInstance().addEventListener(QSetting.ShowRowShading, settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowSeparatorComponent, settingsChanged);
            settingsChanged();
        }
        
        // PRIVATE
        private void settingsChanged()
        {
            showRowShading = QSettings.getInstance().get<bool>(QSetting.ShowRowShading);
            enabled = QSettings.getInstance().get<bool>(QSetting.ShowSeparatorComponent);
        }

        // DRAW
        public override void draw(GameObject gameObject, QObjectList objectList, Rect selectionRect, Rect curRect)
        {
            curRect.y = selectionRect.y + selectionRect.height - 1;
            curRect.width = selectionRect.width + selectionRect.x;
            curRect.height = 1;
            curRect.x = 0;

            EditorGUI.DrawRect(curRect, separatorColor);

            if (showRowShading && (Mathf.FloorToInt(((selectionRect.y - 4) / 16) % 2) == 0))
            {
                selectionRect.width += selectionRect.x;
                selectionRect.x = 0;
                selectionRect.height -=1;
                EditorGUI.DrawRect(selectionRect, shadingColor);
            }
        }
    }
}

