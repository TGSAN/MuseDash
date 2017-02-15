using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using qtools.qhierarchy.pcomponent.pbase;
using qtools.qhierarchy.pdata;
using qtools.qhierarchy.phierarchy;

namespace qtools.qhierarchy.pcomponent
{
    public class QStaticComponent: QBaseComponent
    {
        // PRIVATE
        private Texture2D staticButtonOn;
        private Texture2D staticButtonOff;
        private Texture2D staticButtonHalf;
        private StaticEditorFlags staticFlags;
        private GameObject[] gameObjects;

        // CONSTRUCTOR
        public QStaticComponent()
        {
            staticButtonOn = QResources.getInstance().getTexture(QTexture.QStaticOnButton);
            staticButtonHalf = QResources.getInstance().getTexture(QTexture.QStaticHalfButton);
            staticButtonOff = QResources.getInstance().getTexture(QTexture.QStaticOffButton);

            QSettings.getInstance().addEventListener(QSetting.ShowStaticComponent, settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowStaticComponentDuringPlayMode, settingsChanged);
            settingsChanged();
        }

        // PRIVATE
        private void settingsChanged()
        {
            enabled = QSettings.getInstance().get<bool>(QSetting.ShowStaticComponent);
            showComponentDuringPlayMode = QSettings.getInstance().get<bool>(QSetting.ShowStaticComponentDuringPlayMode);
        }

        // DRAW
        public override void layout(GameObject gameObject, QObjectList objectList, ref Rect rect)
        {
            rect.x -= 14;
            rect.width = 14;
            staticFlags = GameObjectUtility.GetStaticEditorFlags(gameObject);
        }

        public override void draw(GameObject gameObject, QObjectList objectList, Rect selectionRect, Rect curRect)
        {
            bool isStatic = gameObject.isStatic;
            GUI.DrawTexture(curRect, isStatic ? ((int)staticFlags == -1 ? staticButtonOn : staticButtonHalf) : staticButtonOff);
        }

        public override void eventHandler(GameObject gameObject, QObjectList objectList, Event currentEvent, Rect curRect)
        {
            if (currentEvent.isMouse && currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && curRect.Contains(currentEvent.mousePosition))
            {
                currentEvent.Use();

                int intStaticFlags = (int)staticFlags;
                gameObjects = Selection.Contains(gameObject) ? Selection.gameObjects : new GameObject[] { gameObject };

                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Nothing"                   ), intStaticFlags == 0, staticChangeHandler, 0);
                menu.AddItem(new GUIContent("Everything"                ), intStaticFlags == -1, staticChangeHandler, -1);
                menu.AddItem(new GUIContent("Lightmap Static"           ), (intStaticFlags & (int)StaticEditorFlags.LightmapStatic) > 0, staticChangeHandler, (int)StaticEditorFlags.LightmapStatic);
                menu.AddItem(new GUIContent("Occluder Static"           ), (intStaticFlags & (int)StaticEditorFlags.OccluderStatic) > 0, staticChangeHandler, (int)StaticEditorFlags.OccluderStatic);
                menu.AddItem(new GUIContent("Batching Static"           ), (intStaticFlags & (int)StaticEditorFlags.BatchingStatic) > 0, staticChangeHandler, (int)StaticEditorFlags.BatchingStatic);
                menu.AddItem(new GUIContent("Navigation Static"         ), (intStaticFlags & (int)StaticEditorFlags.NavigationStatic) > 0, staticChangeHandler, (int)StaticEditorFlags.NavigationStatic);
                menu.AddItem(new GUIContent("Occludee Static"           ), (intStaticFlags & (int)StaticEditorFlags.OccludeeStatic) > 0, staticChangeHandler, (int)StaticEditorFlags.OccludeeStatic);
                menu.AddItem(new GUIContent("Off Mesh Link Generation"  ), (intStaticFlags & (int)StaticEditorFlags.OffMeshLinkGeneration) > 0, staticChangeHandler, (int)StaticEditorFlags.OffMeshLinkGeneration);
                #if UNITY_5
                menu.AddItem(new GUIContent("Reflection Probe Static"   ), (intStaticFlags & (int)StaticEditorFlags.ReflectionProbeStatic) > 0, staticChangeHandler, (int)StaticEditorFlags.ReflectionProbeStatic);
                #endif
                menu.ShowAsContext();
            }
        }

        // PRIVATE
        private void staticChangeHandler(object result)
        {
            StaticEditorFlags resultStaticFlags = (StaticEditorFlags)result;
            for (int i = gameObjects.Length - 1; i >= 0; i--)
            {
                GameObject gameObject = gameObjects[i];
                Undo.RecordObject(gameObject, "Change Static Flags");
                GameObjectUtility.SetStaticEditorFlags(gameObject, resultStaticFlags);
                EditorUtility.SetDirty(gameObject);
            }
        }
    }
}

