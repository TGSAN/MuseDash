using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using qtools.qhierarchy.pcomponent.pbase;
using qtools.qhierarchy.phierarchy;
using qtools.qhierarchy.pdata;
using qtools.qhierarchy.phelper;

namespace qtools.qhierarchy.pcomponent
{
    public class QRendererComponent: QBaseComponent
    {
        // PRIVATE
        private Texture2D rendererOn;
        private Texture2D rendererOff;
        private Texture2D rendererWireframe;
        private int targetRendererMode = -1; 
        private Color backgroundColor;

        // CONSTRUCTOR
        public QRendererComponent()
        {
            rendererWireframe = QResources.getInstance().getTexture(QTexture.QRendererWireframeButton);
            rendererOn        = QResources.getInstance().getTexture(QTexture.QRendererOnButton);
            rendererOff       = QResources.getInstance().getTexture(QTexture.QRendererOffButton);
            backgroundColor   = QResources.getInstance().getColor(QColor.Background);

            QSettings.getInstance().addEventListener(QSetting.ShowRendererComponent              , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowRendererComponentDuringPlayMode, settingsChanged);
            settingsChanged();
        }

        // PRIVATE
        private void settingsChanged()
        {
            enabled = QSettings.getInstance().get<bool>(QSetting.ShowRendererComponent);
            showComponentDuringPlayMode = QSettings.getInstance().get<bool>(QSetting.ShowRendererComponentDuringPlayMode);
        }

        // DRAW
        public override void layout(GameObject gameObject, QObjectList objectList, ref Rect rect)
        {
            rect.x -= 14;
            rect.width = 14;
        }

        public override void disabledHandler(GameObject gameObject, QObjectList objectList)
        {
            if (objectList != null && objectList.wireframeHiddenObjects.Contains(gameObject))
            {      
                objectList.wireframeHiddenObjects.Remove(gameObject);
                Renderer renderer = gameObject.GetComponent<Renderer>();
                if (renderer != null) setSelectedRenderState(renderer, false);
            }
        }

        public override void draw(GameObject gameObject, QObjectList objectList, Rect selectionRect, Rect curRect)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                bool wireframeHiddenObjectsContains = isWireframeHidden(gameObject, objectList);
                GUI.DrawTexture(curRect, wireframeHiddenObjectsContains ? rendererWireframe : (renderer.enabled ? rendererOn : rendererOff));
            }
            else
            {
                EditorGUI.DrawRect(curRect, backgroundColor);
            }
        }

        public override void eventHandler(GameObject gameObject, QObjectList objectList, Event currentEvent, Rect rect)
        {
            if (currentEvent.isMouse && currentEvent.button == 0 && rect.Contains(currentEvent.mousePosition))
            {
                Renderer renderer = gameObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    bool wireframeHiddenObjectsContains = isWireframeHidden(gameObject, objectList);
                    bool isEnabled = renderer.enabled;
                    
                    if (currentEvent.type == EventType.MouseDown)
                    {
                        targetRendererMode = ((!isEnabled) == true ? 1 : 0);
                    }
                    else if (currentEvent.type == EventType.MouseDrag && targetRendererMode != -1)
                    {
                        if (targetRendererMode == (isEnabled == true ? 1 : 0)) return;
                    } 
                    else
                    {
                        targetRendererMode = -1;
                        return;
                    }

                    Undo.RecordObject(renderer, "renderer visibility change");                    
                    
                    if (currentEvent.control || currentEvent.command)
                    {
                        if (!wireframeHiddenObjectsContains)
                        {
                            setSelectedRenderState(renderer, true);
                            SceneView.RepaintAll();
                            setWireframeMode(gameObject, objectList, true);
                        }
                    }
                    else
                    {
                        if (wireframeHiddenObjectsContains)
                        {
                            setSelectedRenderState(renderer, false);
                            SceneView.RepaintAll();
                            setWireframeMode(gameObject, objectList, false);
                        }
                        else
                        {
                            Undo.RecordObject(renderer, isEnabled ? "Disable Component" : "Enable Component");
                            renderer.enabled = !isEnabled;
                        }
                    }
                    
                    EditorUtility.SetDirty(gameObject);
                }
                currentEvent.Use();
            }
        }

        // PRIVATE
        public bool isWireframeHidden(GameObject gameObject, QObjectList objectList)
        {
            return objectList == null ? false : objectList.wireframeHiddenObjects.Contains(gameObject);
        }
        
        public void setWireframeMode(GameObject gameObject, QObjectList objectList, bool targetWireframe)
        {
            if (objectList == null && targetWireframe) objectList = QObjectListManager.getInstance().getObjectList(gameObject, true);
            if (objectList != null)
            {
                Undo.RecordObject(objectList, "Renderer Visibility Change");
                if (targetWireframe) objectList.wireframeHiddenObjects.Add(gameObject);
                else objectList.wireframeHiddenObjects.Remove(gameObject);
                EditorUtility.SetDirty(objectList);
            }
        }

        static public void setSelectedRenderState(Renderer renderer, bool visible)
        {
            #if UNITY_5_5_OR_NEWER
            EditorUtility.SetSelectedRenderState(renderer, visible ? EditorSelectedRenderState.Wireframe : EditorSelectedRenderState.Hidden);
            #else
            EditorUtility.SetSelectedWireframeHidden(renderer, visible);
            #endif
        }
    }
}

