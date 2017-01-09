//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEngine;
using UEObject = UnityEngine.Object;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace EasyEditor
{
    /// <summary>
    /// Renders an Object, custom class or structure directly into the monobehaviour/scriptableobject inspector.
    /// </summary>
    public class InlineClassRenderer : FullObjectRenderer 
    {
        public object subtarget{get; private set;}
        public string FoldoutTitle{set{foldoutTitle = value;}}
        private string foldoutTitle = "";
        private bool foldout = Settings.inlineUnfolded;

        public override void InitializeFromEntityInfo(EntityInfo entityInfo)
        {
            base.InitializeFromEntityInfo(entityInfo);

            if(entityInfo.isField)
            {
                foldoutTitle = ObjectNames.NicifyVariableName(entityInfo.fieldInfo.Name);
            }
        }

        protected override void InitializeRenderersList()
        {
            base.InitializeRenderersList();

            subtarget = FieldInfoHelper.GetObjectFromPath(entityInfo.propertyPath, serializedObject.targetObject);

            List<InspectorItemRenderer> fieldsRenderers = new List<InspectorItemRenderer>();
            List<InspectorItemRenderer> methodsRenderers = new List<InspectorItemRenderer>();

            if(subtarget != null)
            {
                fieldsRenderers = RendererFinder.GetListOfFields(subtarget, serializedObject, entityInfo.propertyPath);
                methodsRenderers = RendererFinder.GetListOfMethods(subtarget, serializedObject);
            }
            
            renderers = new List<InspectorItemRenderer>();
            renderers.AddRange(fieldsRenderers);
            renderers.AddRange(methodsRenderers);
            
            InspectorItemRendererOrderComparer comparer = new InspectorItemRendererOrderComparer(groups, renderers);
            renderers.Sort(comparer);
        }

        protected override void RetrieveGroupList()
        {
            Type classType = GetClassType();

            if(classType != null)
            {
                if(typeof(UEObject).IsAssignableFrom(classType))
                {
                    RetrieveObjectClassGroup();
                }
                else
                {
                    RetrieveCustomClassGroup();
                }
            }
            else
            {
                groups = new Groups(new string[]{""});
            }
        }

        private void RetrieveObjectClassGroup()
        {
            IEnumerable<Type> collection = Utils.FindSubClassesOf<EasyEditorBase>();
            foreach (Type editorScript in collection)
            {
                CustomEditor customEditorAttribute = AttributeHelper.GetAttribute<CustomEditor>(editorScript);
                if (customEditorAttribute != null)
                {
                    Type inspectedType = typeof(CustomEditor).
                        GetField ("m_InspectedType", BindingFlags.NonPublic | BindingFlags.Instance).
                            GetValue (customEditorAttribute) as Type;

                    bool editorForChildClasses = (bool) typeof(CustomEditor).
                        GetField ("m_EditorForChildClasses", BindingFlags.NonPublic | BindingFlags.Instance).
                            GetValue (customEditorAttribute);

                    if (GetClassType() == inspectedType
                        || (inspectedType.IsAssignableFrom(GetClassType()) 
                        && editorForChildClasses))
                    {
                        GroupsAttribute groupAttribute = AttributeHelper.GetAttribute<GroupsAttribute>(editorScript);
                        if (groupAttribute != null)
                        {
                            groups = new Groups(groupAttribute.groups);
                        }
                        else
                        {
                            groups = new Groups(new string[]{""});
                        }

                        break;
                    }
                }
            }

            if(groups == null)
            {
                groups = new Groups(new string[]{""});
            }
        }

        private void RetrieveCustomClassGroup()
        {
            GroupsAttribute groupAttribute = AttributeHelper.GetAttribute<GroupsAttribute>(GetClassType());
            if (groupAttribute != null)
            {
                groups = new Groups(groupAttribute.groups);
            }
            else
            {
                groups = new Groups(new string[]{""});
            }
        }

        public override void Render(Action preRender = null)
        {
            EditorGUILayout.BeginVertical(InspectorStyle.DefaultStyle.inlineFoldableBackgroundStyle);

            if(typeof(UEObject).IsAssignableFrom(GetClassType()))
            {
                RenderObjectClass(preRender);
            }
            else
            {
                RenderCustomClass(preRender);
            }

            EditorGUILayout.EndVertical();
        }

        void RenderCustomClass(Action preRender = null)
        {
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);

            EditorGUILayout.BeginVertical();

            foldout = EditorGUILayout.Foldout(foldout, foldoutTitle);
            if (foldout)
            {
                EditorGUILayout.BeginVertical(InspectorStyle.DefaultStyle.inlineFoldableBackgroundStyle);
                base.Render(preRender);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        SerializedObject subSerializedObject = null;
        void RenderObjectClass(Action preRender = null)
        {
            EditorGUILayout.PropertyField(FieldInfoHelper.GetSerializedPropertyFromPath(entityInfo.propertyPath, 
                                                                                        serializedObject));

            if(subtarget != null)
            {
                CreateSerializedObject();
                subSerializedObject.Update();

                EditorGUI.indentLevel += 2 * Settings.indentation;

                foldout = EditorGUILayout.Foldout(foldout, "");
                if (foldout)
                {
                    GUILayout.Space(15 * Settings.indentation);
                    EditorGUILayout.BeginHorizontal(InspectorStyle.DefaultStyle.inlineFoldableBackgroundStyle);
                    
                    base.Render(preRender);
                        
                    EditorGUILayout.EndHorizontal();
                }
            
                EditorGUI.indentLevel -= 2 * Settings.indentation;

                subSerializedObject.ApplyModifiedProperties();
            }

            CheckIfTargetNotNull();
        }

        private void CreateSerializedObject()
        {
            if(subSerializedObject == null && subtarget != null)
            {
                subSerializedObject = new SerializedObject((UEObject) subtarget);
            }
        }

        bool subtargetWasNull = true;
        private void CheckIfTargetNotNull()
        {
            if (Event.current.type == EventType.Repaint)
            {
                subtarget = FieldInfoHelper.GetObjectFromPath(entityInfo.propertyPath, serializedObject.targetObject);
                if(subtarget == null)
                {
                    subtargetWasNull = true;
                }
                else if(subtarget != null && subtargetWasNull)
                {
                    InitializeRenderers();
                    subtargetWasNull = false;
                }
            }
        }

        private Type GetClassType()
        {
            Type classType = null;
            if(entityInfo.isField)
            {
                classType = entityInfo.fieldInfo.FieldType;
            }
            else if(entityInfo.isType)
            {
                classType = entityInfo.type;
            }

            return classType;
        }
    }
}