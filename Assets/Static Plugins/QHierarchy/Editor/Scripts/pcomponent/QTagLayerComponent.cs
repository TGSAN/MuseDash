using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using qtools.qhierarchy.pcomponent.pbase;
using qtools.qhierarchy.pdata;
using System.Reflection;

namespace qtools.qhierarchy.pcomponent
{
    public class QTagLayerComponent: QBaseComponent
    {
        // PRIVATE
        private GUIStyle labelStyle;
        private Color greyLightColor;
        private Color greyColor;
        private Color backgroundColor;
        private bool showAlways;
        private bool sizeIsPixel;
        private int pixelSize;
        private float percentSize;
        private GameObject[] gameObjects;
        private QHierarchyTagAndLayerLabelSize labelSize;

        // CONSTRUCTOR
        public QTagLayerComponent()
        {
            backgroundColor = QResources.getInstance().getColor(QColor.Background);
            greyColor = QResources.getInstance().getColor(QColor.GrayDark);
            greyLightColor = QResources.getInstance().getColor(QColor.GrayLight);

            labelStyle = new GUIStyle();
            labelStyle.normal.textColor = greyColor;
            labelStyle.fontSize = 8;
            labelStyle.clipping = TextClipping.Clip;  

            QSettings.getInstance().addEventListener(QSetting.TagAndLayerType            , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.TagAndLayerSizeValueType   , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.TagAndLayerSizeValuePixel  , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.TagAndLayerSizeValuePercent, settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.TagAndLayerLabelSize       , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowTagLayerComponent      , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowTagLayerComponentDuringPlayMode, settingsChanged);
            settingsChanged();
        }

        // PRIVATE
        private void settingsChanged()
        {
            showAlways  = QSettings.getInstance().get<int>(QSetting.TagAndLayerType) == (int)QHierarchyTagAndLayerType.Always;
            sizeIsPixel = QSettings.getInstance().get<int>(QSetting.TagAndLayerSizeValueType) == (int)QHierarchyTagAndLayerSizeType.Pixel;
            pixelSize   = QSettings.getInstance().get<int>(QSetting.TagAndLayerSizeValuePixel);
            percentSize = QSettings.getInstance().get<float>(QSetting.TagAndLayerSizeValuePercent);
            labelSize   = (QHierarchyTagAndLayerLabelSize)QSettings.getInstance().get<int>(QSetting.TagAndLayerLabelSize);
            enabled     = QSettings.getInstance().get<bool>(QSetting.ShowTagLayerComponent);
            showComponentDuringPlayMode = QSettings.getInstance().get<bool>(QSetting.ShowTagLayerComponentDuringPlayMode);
        }

        // DRAW
        public override void layout(GameObject gameObject, QObjectList objectList, ref Rect rect)
        {
            float textWidth = sizeIsPixel ? pixelSize : percentSize * rect.x;
            rect.width = textWidth + 4;
            rect.x -= rect.width;
        }

        public override void draw(GameObject gameObject, QObjectList objectList, Rect selectionRect, Rect curRect)
        {
            int layer  = gameObject.layer; 
            string tag = getTagName(gameObject);                             

            EditorGUI.DrawRect(curRect, backgroundColor);

            if (showAlways || (tag != "Untagged" || layer != 0))
            {
                curRect.height = 17;
                curRect.y -= 2;
                curRect.width -= 4;
                curRect.x += 2;
                
                int aligment = QSettings.getInstance().get<int>(QSetting.TagAndLayerAligment);
                if (aligment == (int)QHierarchyTagAndLayerAligment.Left)
                    labelStyle.alignment = TextAnchor.UpperLeft;
                else if (aligment == (int)QHierarchyTagAndLayerAligment.Center)
                    labelStyle.alignment = TextAnchor.UpperCenter;
                else if (aligment == (int)QHierarchyTagAndLayerAligment.Right)
                    labelStyle.alignment = TextAnchor.UpperRight;
                
                if (layer == 0 && tag != "Untagged" && !showAlways)
                {
                    if (labelSize == QHierarchyTagAndLayerLabelSize.Small) labelStyle.fontSize = 8;
                    else labelStyle.fontSize = 9;

                    curRect.y += 5;
                    labelStyle.normal.textColor = greyLightColor;
                    EditorGUI.LabelField(curRect, tag, labelStyle);
                }
                else if (layer != 0 && tag == "Untagged" && !showAlways)
                {
                    if (labelSize == QHierarchyTagAndLayerLabelSize.Small) labelStyle.fontSize = 8;
                    else labelStyle.fontSize = 9;

                    curRect.y += 5;
                    labelStyle.normal.textColor = greyLightColor;
                    EditorGUI.LabelField(curRect, getLayerName(layer), labelStyle);
                }
                else
                {
                    if (labelSize == QHierarchyTagAndLayerLabelSize.Normal) labelStyle.fontSize = 9;
                    else labelStyle.fontSize = 8;

                    labelStyle.normal.textColor = tag == "Untagged" ? greyColor : greyLightColor;
                    EditorGUI.LabelField(curRect, tag, labelStyle);
                    curRect.y += 8;
                    labelStyle.normal.textColor = layer == 0 ? greyColor : greyLightColor;
                    EditorGUI.LabelField(curRect, getLayerName(layer), labelStyle);
                }
            }
        }

        public override void eventHandler(GameObject gameObject, QObjectList objectList, Event currentEvent, Rect rect)
        {                       
            if (Event.current.isMouse && currentEvent.type == EventType.MouseDown && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
            {
                int layer  = gameObject.layer;  
                string tag = getTagName(gameObject);

                if (showAlways || (tag != "Untagged" || layer != 0))
                {
                    Event.current.Use();
                    gameObjects = Selection.Contains(gameObject) ? Selection.gameObjects : new GameObject[] { gameObject };

                    if (layer == 0 && tag != "Untagged" && !showAlways)
                    {
                        showTagsContextMenu(tag);
                    }
                    else if (layer != 0 && tag == "Untagged" && !showAlways)
                    {
                        showLayersContextMenu(LayerMask.LayerToName(layer));
                    }
                    else
                    {
                        if (Event.current.mousePosition.y < rect.y + rect.height / 2)
                        {
                            showTagsContextMenu(tag);
                        }
                        else
                        {
                            showLayersContextMenu(LayerMask.LayerToName(layer));
                        }
                    }
                }
            }
        }

        private string getTagName(GameObject gameObject)
        {
            string tag = "Undefined";
            try { tag = gameObject.tag; }
            catch {}
            return tag;
        }

        public string getLayerName(int layer)
        {
            string layerName = LayerMask.LayerToName(layer);
            if (layerName.Equals("")) layerName = "Undefined";
            return layerName;
        }

        // PRIVATE
        private void showTagsContextMenu(string tag)
        {
            List<string> tags = new List<string>(UnityEditorInternal.InternalEditorUtility.tags);
            
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Untagged"  ), false, tagChangedHandler, "Untagged");
            
            for (int i = 0, n = tags.Count; i < n; i++)
            {
                string curTag = tags[i];
                menu.AddItem(new GUIContent(curTag), tag == curTag, tagChangedHandler, curTag);
            }
            
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Add Tag..."  ), false, addTagOrLayerHandler, "Tags");
            menu.ShowAsContext();
        }

        private void showLayersContextMenu(string layer)
        {
            List<string> layers = new List<string>(UnityEditorInternal.InternalEditorUtility.layers);
            
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Default"  ), false, layerChangedHandler, "Default");
            
            for (int i = 0, n = layers.Count; i < n; i++)
            {
                string curLayer = layers[i];
                menu.AddItem(new GUIContent(curLayer), layer == curLayer, layerChangedHandler, curLayer);
            }
            
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Add Layer..."  ), false, addTagOrLayerHandler, "Layers");
            menu.ShowAsContext();
        }

        private void tagChangedHandler(object newTag)
        {
            for (int i = gameObjects.Length - 1; i >= 0; i--)
            {
                GameObject gameObject = gameObjects[i];
                Undo.RecordObject(gameObject, "Change Tag");
                gameObject.tag = (string)newTag;
                EditorUtility.SetDirty(gameObject);
            }
        }

        private void layerChangedHandler(object newLayer)
        {
            int newLayerId = LayerMask.NameToLayer((string)newLayer);
            for (int i = gameObjects.Length - 1; i >= 0; i--)
            {
                GameObject gameObject = gameObjects[i];
                Undo.RecordObject(gameObject, "Change Layer");
                gameObject.layer = newLayerId;
                EditorUtility.SetDirty(gameObject);
            }
        }

        private void addTagOrLayerHandler(object value)
        {
            PropertyInfo propertyInfo = typeof(EditorApplication).GetProperty("tagManager", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty);
            UnityEngine.Object obj = (UnityEngine.Object)(propertyInfo.GetValue(null, null));
            obj.GetType().GetField("m_DefaultExpandedFoldout").SetValue(obj, value);
            Selection.activeObject = obj;
        }
    }
}

