using UnityEngine;
using UnityEditor;
using System;
using qtools.qhierarchy.pcomponent.pbase;
using qtools.qhierarchy.pdata;
using qtools.qhierarchy.phierarchy;

namespace qtools.qhierarchy.pcomponent
{
    public class QTreeMapComponent: QBaseComponent
    {
        // CONST
        private const float TREE_STEP_WIDTH  = 14.0f;
        private const float TREE_STEP_HEIGHT = 16.0f;

        // PRIVATE
        private Texture2D treeMapTexture;       
        private Texture2D treeMapParentTexture;    

        // CONSTRUCTOR
        public QTreeMapComponent()
        { 
            treeMapTexture = QResources.getInstance().getTexture(QTexture.QTreeMap);
            treeMapParentTexture = QResources.getInstance().getTexture(QTexture.QTreeMapParent);

            showComponentDuringPlayMode = true;

            QSettings.getInstance().addEventListener(QSetting.ShowTreeMapComponent, settingsChanged);
            settingsChanged();
        }

        // PRIVATE
        private void settingsChanged()
        {
            enabled = QSettings.getInstance().get<bool>(QSetting.ShowTreeMapComponent);
        }

        // DRAW
        public override void draw(GameObject gameObject, QObjectList objectList, Rect selectionRect, Rect curRect)
        {
            int ident = Mathf.FloorToInt(selectionRect.x / TREE_STEP_WIDTH);

            curRect.x      = - 31 * TREE_STEP_WIDTH + ident * TREE_STEP_WIDTH;
            curRect.y      = selectionRect.y;
            curRect.width  = treeMapTexture.width;
            curRect.height = TREE_STEP_HEIGHT;
                        
            GUI.DrawTexture(curRect, gameObject.transform.childCount > 0 ? treeMapParentTexture : treeMapTexture);
        }
    }
}

