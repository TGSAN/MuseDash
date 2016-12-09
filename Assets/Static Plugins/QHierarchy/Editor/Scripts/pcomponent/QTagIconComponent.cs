using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using qtools.qhierarchy.pcomponent.pbase;
using qtools.qhierarchy.phierarchy;
using qtools.qhierarchy.phelper;
using qtools.qhierarchy.pdata;

namespace qtools.qhierarchy.pcomponent
{
    public class QTagIconComponent: QBaseComponent
    {
        // PRIVATE
        private Color backgroundColor;

        // CONSTRUCTOR
        public QTagIconComponent()
        {
            backgroundColor = QResources.getInstance().getColor(QColor.Background);

            QSettings.getInstance().addEventListener(QSetting.ShowTagIconComponent              , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowTagIconComponentDuringPlayMode, settingsChanged);
            settingsChanged();
        }
        
        // PRIVATE
        private void settingsChanged()
        {
            enabled = QSettings.getInstance().get<bool>(QSetting.ShowTagIconComponent);
            showComponentDuringPlayMode = QSettings.getInstance().get<bool>(QSetting.ShowTagIconComponentDuringPlayMode);
        }

        // DRAW
        public override void layout(GameObject gameObject, QObjectList objectList, ref Rect rect)
        {
            rect.x -= 18;
            rect.width = 18;
        }

        public override void draw(GameObject gameObject, QObjectList objectList, Rect selectionRect, Rect curRect)
        {                       
            string gameObjectTag = "";
            try { gameObjectTag = gameObject.tag; }
            catch {}

            QTagTexture tagTexture = QSettings.getInstance().get<List<QTagTexture>>(QSetting.CustomTagIcon).Find(t => t.tag == gameObjectTag);
            EditorGUI.DrawRect(curRect, backgroundColor);
            if (tagTexture != null && tagTexture.texture != null)
            {
                curRect.width = 16;
                GUI.DrawTexture(curRect, tagTexture.texture, ScaleMode.ScaleToFit, true);
            }
        }
    }
}

