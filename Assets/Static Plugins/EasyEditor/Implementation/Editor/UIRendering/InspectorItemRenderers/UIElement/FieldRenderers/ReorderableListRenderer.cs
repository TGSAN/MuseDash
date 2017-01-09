//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEngine;
using UnityEditor;
using UEObject = UnityEngine.Object;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using EasyEditor.ReorderableList;

namespace EasyEditor
{
    /// <summary>
    /// Render a list in the inspector. The list UI allows to change element position, or remove them from the list in a very easy way.
    /// Adding the attribute [Selectable] to the list allows to select item to render the details of their field below the list.
    /// </summary>
    [RenderType(typeof(IList))]
    public class ReorderableListRenderer : FieldRenderer
    {
        public Action<int, SerializedProperty> OnItemInserted, OnItemBeingRemoved, OnItemMoving;

        private SerializedProperty list;
        private ReorderableListControl listControl;
        private EESerializedPropertyAdaptor listAdaptor;

       
        bool isReadOnly = false;
        bool isSelectable = false;

        public override void CreateAsset(string path)
        {
            Utils.CreateAssetFrom<ReorderableListRenderer>(this, "List_" + label, path);
        }

        public override void InitializeFromEntityInfo(EntityInfo entityInfo)
        {
            base.InitializeFromEntityInfo(entityInfo);

            isReadOnly = (AttributeHelper.GetAttribute<ReadOnlyAttribute>(entityInfo.fieldInfo) != null);
            isSelectable = (AttributeHelper.GetAttribute<SelectableAttribute>(entityInfo.fieldInfo) != null);

            list = FieldInfoHelper.GetSerializedPropertyFromPath(entityInfo.propertyPath, entityInfo.serializedObject);
            listControl = new ReorderableListControl();

            listControl.ItemInserted += OnItemInsertedHandler;
            listControl.ItemRemoving += OnItemRemovingHandler;

            if(isReadOnly)
            {
                listControl.Flags = ReorderableListFlags.DisableReordering 
                        | ReorderableListFlags.DisableContextMenu 
                        | ReorderableListFlags.HideAddButton
                        | ReorderableListFlags.HideRemoveButtons;
            }

			listAdaptor = new EESerializedPropertyAdaptor(serializedProperty, isReadOnly);
            listAdaptor.OnItemSelected += HandleOnItemSelected;
        }

        private void OnItemInsertedHandler(object sender, ItemInsertedEventArgs args) {
            if(OnItemInserted != null)
            {
                OnItemInserted(args.ItemIndex, list);
            }
        }
        
        private void OnItemRemovingHandler(object sender, ItemRemovingEventArgs args) {
            if(OnItemBeingRemoved != null)
            {
                OnItemBeingRemoved(args.ItemIndex, list);
            }
        }

        InlineClassRenderer inlineClassRenderer;

        private void HandleOnItemSelected(int index, SerializedProperty list)
        {
            if(isSelectable)
            {
                inlineClassRenderer = null;

                SerializedProperty propertyToRender = list.GetArrayElementAtIndex(index);
                if(propertyToRender.propertyType == SerializedPropertyType.ObjectReference || propertyToRender.propertyType == SerializedPropertyType.Generic)
                {
                    object listElement = FieldInfoHelper.GetObjectFromPath(propertyToRender.propertyPath, serializedObject.targetObject);

                    if(listElement != null)
                    {
                        EntityInfo info = new EntityInfo(listElement.GetType(), 
                                                     serializedObject, propertyToRender.propertyPath);
                        inlineClassRenderer = InspectorItemRenderer.CreateRenderer<InlineClassRenderer>();
                        inlineClassRenderer.InitializeFromEntityInfo(info);

                        if(propertyToRender.propertyType == SerializedPropertyType.Generic)
                        {
                            inlineClassRenderer.FoldoutTitle = propertyToRender.displayName;
                        }
                    }
                }
            }
        }

        public override void Render(Action preRender = null)
        {
            base.Render(preRender);

            directParentSerializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * 10f * Settings.indentation);

            if(inlineClassRenderer != null)
            {
                EditorGUILayout.BeginVertical(InspectorStyle.DefaultStyle.inlineFoldableBackgroundStyle);
            }
            else
            {
                EditorGUILayout.BeginVertical();
            }

            ReorderableListGUI.Title(label);

            listControl.Draw(listAdaptor);

            if(inlineClassRenderer != null)
            {
                inlineClassRenderer.Render(preRender);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            directParentSerializedObject.ApplyModifiedProperties();
        }
    }
}