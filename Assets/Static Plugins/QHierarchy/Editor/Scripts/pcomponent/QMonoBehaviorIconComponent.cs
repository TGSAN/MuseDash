using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using qtools.qhierarchy.pcomponent.pbase;
using qtools.qhierarchy.pdata;
using qtools.qhierarchy.phelper;

namespace qtools.qhierarchy.pcomponent
{
    public class QMonoBehaviorIconComponent: QBaseComponent
    {
        // CONST
        private const float TREE_STEP_WIDTH  = 14.0f;
        private const float TREE_STEP_HEIGHT = 16.0f;

        // PRIVATE
        private Texture2D monoBehaviourIconTexture;   
        private Texture2D monoBehaviourIconParentTexture; 
        private bool ignoreUnityMonobehaviour;

        // CONSTRUCTOR 
        public QMonoBehaviorIconComponent()
        {
            monoBehaviourIconTexture = QResources.getInstance().getTexture(QTexture.QMonoBehaviourIcon);
            monoBehaviourIconParentTexture = QResources.getInstance().getTexture(QTexture.QMonoBehaviourIconParent);

            QSettings.getInstance().addEventListener(QSetting.IgnoreUnityMonobehaviour, settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowMonoBehaviourIconComponent, settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowMonoBehaviourIconComponentDuringPlayMode, settingsChanged);
            settingsChanged();
        }

        // PRIVATE
        private void settingsChanged()
        {
            ignoreUnityMonobehaviour = QSettings.getInstance().get<bool>(QSetting.IgnoreUnityMonobehaviour);
            enabled = QSettings.getInstance().get<bool>(QSetting.ShowMonoBehaviourIconComponent);
            showComponentDuringPlayMode = QSettings.getInstance().get<bool>(QSetting.ShowMonoBehaviourIconComponentDuringPlayMode);
            EditorApplication.RepaintHierarchyWindow();  
        }

        public override void draw(GameObject gameObject, QObjectList objectList, Rect selectionRect, Rect curRect)
        {
            bool foundCustomComponent = false;   
            if (ignoreUnityMonobehaviour)
            {
                Component[] components = gameObject.GetComponents<MonoBehaviour>();                
                for (int i = components.Length - 1; i >= 0; i--)
                {
                    if (components[i] != null && !components[i].GetType().FullName.Contains("UnityEngine")) 
                    {
                        foundCustomComponent = true;
                        break;
                    }
                }                
            }
            else
            {
                foundCustomComponent = gameObject.GetComponent<MonoBehaviour>() != null;
            }

            if (foundCustomComponent)
            {
                int ident = Mathf.FloorToInt(selectionRect.x / TREE_STEP_WIDTH) - 1;
                
                curRect.x      = 1 + ident * TREE_STEP_WIDTH;
                curRect.y      = selectionRect.y;
                curRect.width  = TREE_STEP_WIDTH;
                curRect.height = selectionRect.height;
                
                GUI.DrawTexture(curRect, gameObject.transform.childCount > 0 ? monoBehaviourIconParentTexture : monoBehaviourIconTexture);
            }
        }
    }
}

