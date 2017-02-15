using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using qtools.qhierarchy.pcomponent.pbase;
using qtools.qhierarchy.pdata;
using qtools.qhierarchy.phelper;

namespace qtools.qhierarchy.pcomponent
{
    public class QColorComponent: QBaseComponent
    {
        // PRIVATE
        private Texture2D colorTexture;
        private Color backgroundColor;

        // CONSTRUCTOR
        public QColorComponent()
        {
            colorTexture = QResources.getInstance().getTexture(QTexture.QColorButton);
            backgroundColor = QResources.getInstance().getColor(QColor.Background);

            QSettings.getInstance().addEventListener(QSetting.ShowColorComponent              , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowColorComponentDuringPlayMode, settingsChanged);
            settingsChanged();
        }

        // PRIVATE
        private void settingsChanged()
        {
            enabled = QSettings.getInstance().get<bool>(QSetting.ShowColorComponent);
            showComponentDuringPlayMode = QSettings.getInstance().get<bool>(QSetting.ShowColorComponentDuringPlayMode);
        }

        // LAYOUT
        public override void layout(GameObject gameObject, QObjectList objectList, ref Rect rect)
        {
            rect.x -= 9;
            rect.width = 9;
        }

        // DRAW
        public override void draw(GameObject gameObject, QObjectList objectList, Rect selectionRect, Rect curRect)
        {
            EditorGUI.DrawRect(curRect, backgroundColor);
            GUI.DrawTexture(curRect, colorTexture, ScaleMode.StretchToFill, true, 1);

            if (objectList != null)
            {
                curRect.x += 1;
                curRect.width = 5;
                curRect.height -= 1;

                Color newColor;
                if (objectList.gameObjectColor.TryGetValue(gameObject, out newColor))
                {
                    EditorGUI.DrawRect(curRect, newColor);
                }
            }
        }

        // EVENTS
        public override void eventHandler(GameObject gameObject, QObjectList objectList, Event currentEvent, Rect curRect)
        {
            if (currentEvent.isMouse && currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && curRect.Contains(currentEvent.mousePosition))
            {
                if (currentEvent.type == EventType.MouseDown)
                {
                    Color color = QResources.getInstance().getColor(QColor.Background);
                    color.a = 0.1f;

                    if (objectList != null) objectList.gameObjectColor.TryGetValue(gameObject, out color);   

                    try 
                    {
                        PopupWindow.Show(curRect, new QColorPickerWindow(Selection.Contains(gameObject) ? Selection.gameObjects : new GameObject[] { gameObject }, colorSelectedHandler, colorRemovedHandler));
                    } 
                    catch 
                    {}
                }
                currentEvent.Use();
            }
        }

        // PRIVATE
        private void colorSelectedHandler(GameObject[] gameObjects, Color color)
        {
            for (int i = gameObjects.Length - 1; i >= 0; i--)
            {
                GameObject gameObject = gameObjects[i];
                QObjectList objectList = QObjectListManager.getInstance().getObjectList(gameObjects[i], true);
                Undo.RecordObject(objectList, "Color Changed");
                if (objectList.gameObjectColor.ContainsKey(gameObject))
                {
                    objectList.gameObjectColor[gameObject] = color;
                }
                else
                {
                    objectList.gameObjectColor.Add(gameObject, color);
                }                
            }
            EditorApplication.RepaintHierarchyWindow();
        }

        private void colorRemovedHandler(GameObject[] gameObjects)
        {
            for (int i = gameObjects.Length - 1; i >= 0; i--)
            {
                GameObject gameObject = gameObjects[i];
                QObjectList objectList = QObjectListManager.getInstance().getObjectList(gameObjects[i], true);
                if (objectList.gameObjectColor.ContainsKey(gameObject))                
                {
                    Undo.RecordObject(objectList, "Color Changed");
                    objectList.gameObjectColor.Remove(gameObject);                          
                }
            }
            EditorApplication.RepaintHierarchyWindow();
        }
    }
}

