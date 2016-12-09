using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using qtools.qhierarchy.pcomponent.pbase;
using qtools.qhierarchy.pdata;

namespace qtools.qhierarchy.pcomponent
{
    public class QPrefabComponent: QBaseComponent
    {
        // PRIVATE
        private Texture2D prefabConnectedTexture;
        private Texture2D prefabDisconnectedTexture;
        private Color backgroundColor;
        private bool showPrefabConnectedIcon;

        // CONSTRUCTOR
        public QPrefabComponent()
        {
            prefabConnectedTexture = QResources.getInstance().getTexture(QTexture.QPrefabConnectedIcon);
            prefabDisconnectedTexture = QResources.getInstance().getTexture(QTexture.QPrefabDisconnectedIcon);
            backgroundColor = QResources.getInstance().getColor(QColor.Background);

            QSettings.getInstance().addEventListener(QSetting.ShowBreakedPrefabsOnly           , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowPrefabComponent              , settingsChanged);
            settingsChanged();
        }
        
        // PRIVATE
        private void settingsChanged()
        {
            showPrefabConnectedIcon = QSettings.getInstance().get<bool>(QSetting.ShowBreakedPrefabsOnly);
            enabled = QSettings.getInstance().get<bool>(QSetting.ShowPrefabComponent);
        }

        // DRAW
        public override void layout(GameObject gameObject, QObjectList objectList, ref Rect rect)
        {
            rect.x -= 9;
            rect.width = 9;
        }
        
        public override void draw(GameObject gameObject, QObjectList objectList, Rect selectionRect, Rect curRect)
        {  
            PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject);
            if (prefabType == PrefabType.MissingPrefabInstance || 
                prefabType == PrefabType.DisconnectedPrefabInstance ||
                prefabType == PrefabType.DisconnectedModelPrefabInstance)
            {
                GUI.DrawTexture(curRect, prefabDisconnectedTexture);
            }
            else
            {
                if (!showPrefabConnectedIcon && prefabType != PrefabType.None)
                {
                    GUI.DrawTexture(curRect, prefabConnectedTexture);
                }
                else
                {
                    EditorGUI.DrawRect(curRect, backgroundColor);
                }
            }
        }
    }
}

