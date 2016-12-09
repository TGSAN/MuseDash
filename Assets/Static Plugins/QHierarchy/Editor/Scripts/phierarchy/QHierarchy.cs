using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using qtools.qhierarchy.pcomponent;
using qtools.qhierarchy.pcomponent.pbase;
using qtools.qhierarchy.pdata;
using qtools.qhierarchy.phelper;
using System.Reflection;

namespace qtools.qhierarchy.phierarchy
{
    public class QHierarchy
    {
        // PRIVATE
        private HashSet<int> errorHandled = new HashSet<int>();      
        private Dictionary<int, QBaseComponent> componentDictionary;          
        private List<QBaseComponent> preComponents;
        private List<QBaseComponent> orderedComponents;
        private List<QBaseComponent> postComponents;
        private bool hideIconsIfThereIsNoFreeSpace;
        private int indentation;
        private Texture2D trimIcon;

        // CONSTRUCTOR
        public QHierarchy ()
        {
            componentDictionary = new Dictionary<int, QBaseComponent>();
            componentDictionary.Add((int)QHierarchyComponentEnum.LockComponent             , new QLockComponent());
            componentDictionary.Add((int)QHierarchyComponentEnum.VisibilityComponent       , new QVisibilityComponent());
            componentDictionary.Add((int)QHierarchyComponentEnum.StaticComponent           , new QStaticComponent());
            componentDictionary.Add((int)QHierarchyComponentEnum.RendererComponent         , new QRendererComponent());
            componentDictionary.Add((int)QHierarchyComponentEnum.TagLayerComponent         , new QTagLayerComponent());
            componentDictionary.Add((int)QHierarchyComponentEnum.GameObjectIconComponent   , new QGameObjectIconComponent());
            componentDictionary.Add((int)QHierarchyComponentEnum.ErrorComponent            , new QErrorComponent());
            componentDictionary.Add((int)QHierarchyComponentEnum.TagIconComponent          , new QTagIconComponent());
            componentDictionary.Add((int)QHierarchyComponentEnum.ColorComponent            , new QColorComponent());
            componentDictionary.Add((int)QHierarchyComponentEnum.ComponentsComponent       , new QComponentsComponent());
            componentDictionary.Add((int)QHierarchyComponentEnum.ChildrenCountComponent    , new QChildrenCountComponent());
            componentDictionary.Add((int)QHierarchyComponentEnum.PrefabComponent           , new QPrefabComponent());

            preComponents = new List<QBaseComponent>();
            preComponents.Add(new QTreeMapComponent());
            preComponents.Add(new QMonoBehaviorIconComponent());

            orderedComponents = new List<QBaseComponent>();

            postComponents = new List<QBaseComponent>();
            postComponents.Add(new QSeparatorComponent());

            trimIcon = QResources.getInstance().getTexture(QTexture.QTrimIcon);

            QSettings.getInstance().addEventListener(QSetting.Identation                    , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ComponentOrder                , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.HideIconsIfNotFit             , settingsChanged);
            settingsChanged();
        }
         
        // PRIVATE
        private void settingsChanged()
        {
            orderedComponents.Clear(); 
            string componentOrder = QSettings.getInstance().get<string>(QSetting.ComponentOrder);
            string[] componentIds = componentOrder.Split(';');
            if (componentIds.Length != QSettings.DEFAULT_ORDER_COUNT) 
            {
                QSettings.getInstance().set(QSetting.ComponentOrder, QSettings.DEFAULT_ORDER);
                componentIds = QSettings.DEFAULT_ORDER.Split(';');
            }

            for (int i = 0; i < componentIds.Length; i++)                
                orderedComponents.Add(componentDictionary[int.Parse(componentIds[i])]);
            orderedComponents.Add(componentDictionary[(int)QHierarchyComponentEnum.ComponentsComponent]);

            indentation = QSettings.getInstance().get<int>(QSetting.Identation);
            hideIconsIfThereIsNoFreeSpace = QSettings.getInstance().get<bool>(QSetting.HideIconsIfNotFit);
        }

        public void hierarchyWindowItemOnGUIHandler(int instanceId, Rect selectionRect)
        {
            try
            {
                GameObject gameObject = (GameObject)EditorUtility.InstanceIDToObject(instanceId);
                if (gameObject == null) return;

                Rect curRect = new Rect(selectionRect);
                curRect.width = 16;
                curRect.x += selectionRect.width - indentation;

                float gameObjectNameWidth = hideIconsIfThereIsNoFreeSpace ? GUI.skin.label.CalcSize(new GUIContent(gameObject.name)).x : 0;

                QObjectList objectList = QObjectListManager.getInstance().getObjectList(gameObject, false);

                drawComponents(preComponents    , selectionRect, ref curRect, gameObject, objectList);
                drawComponents(orderedComponents, selectionRect, ref curRect, gameObject, objectList, hideIconsIfThereIsNoFreeSpace, selectionRect.x + gameObjectNameWidth + 7);
                drawComponents(postComponents   , selectionRect, ref curRect, gameObject, objectList);

                errorHandled.Remove(instanceId);
            }
            catch (Exception exception)
            {
                if (errorHandled.Add(instanceId))
                {
                    Debug.LogError(exception.ToString());
                }
            }
        }

        private void drawComponents(List<QBaseComponent> components, Rect selectionRect, ref Rect curRect, GameObject gameObject, QObjectList objectList, bool trim = false, float minX = 50)
        {
            Rect rect = new Rect(curRect);
            if (Event.current.type == EventType.Repaint)
            {
                for (int i = 0, n = components.Count; i < n; i++)
                {
                    QBaseComponent component = components[i];
                    if (component.isEnabled())
                    {
                        component.layout(gameObject, objectList, ref rect);
                        if (trim && minX > rect.x)
                        {
                            rect.Set(curRect.x - 7, curRect.y, 7, 16);
                            GUI.DrawTexture(rect, trimIcon);
                            return;
                        }
                        component.draw(gameObject, objectList, selectionRect, rect);
                        curRect.Set(rect.x, rect.y, rect.width, rect.height);
                    }
                    else
                    {
                        component.disabledHandler(gameObject, objectList);
                    }
                }
            }
            else if (Event.current.isMouse)
            {
                for (int i = 0, n = components.Count; i < n; i++)
                {
                    QBaseComponent component = components[i];
                    if (component.isEnabled())
                    {
                        component.layout(gameObject, objectList, ref rect);
                        if (trim && minX > rect.x)
                        {
                            rect.Set(curRect.x - 7, curRect.y, 7, 16);
                            GUI.DrawTexture(rect, trimIcon);
                            return;
                        }
                        component.eventHandler(gameObject, objectList, Event.current, rect);
                        curRect.Set(rect.x, rect.y, rect.width, rect.height);
                    }
                }
            }
        }
    }
}

