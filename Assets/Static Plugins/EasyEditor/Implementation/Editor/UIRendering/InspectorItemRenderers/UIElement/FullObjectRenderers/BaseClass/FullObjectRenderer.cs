//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEngine;
using UnityEditor;
using UEObject = UnityEngine.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace EasyEditor
{
    /// <summary>
    /// Render a list of renderers based on the group they belong to and the specified order. Allows as well to hide/show groups/fields.
    /// </summary>
    abstract public class FullObjectRenderer : InspectorItemRenderer
    {
        public List<InspectorItemRenderer> renderers{get; protected set;}
        protected Groups groups;
        private List<FullObjectDecoratedRenderer> decoratedRenderers;

        private bool currentLayoutIsHorizontal = false;

        public override void InitializeFromEntityInfo(EntityInfo entityInfo)
        {
            base.InitializeFromEntityInfo(entityInfo);

            InitializeRenderers();
        }

        protected void InitializeRenderers()
        {
            RetrieveGroupList();

            InitializeRenderersList();
            InitializeGroupDescription();
            InitializeGroupProperties();
            
            InitializeFullObjectDecoratedRenderers();
        }

        public void HideGroup(string group)
        {
            groups.HideGroup(group);
        }
        
        public void ShowGroup(string group)
        {
            groups.ShowGroup(group);
        }
        
        public void HideRenderer(string id)
        {
            InspectorItemRenderer renderer = LookForRenderer(id);
            if (renderer != null)
            {
                renderer.hidden = true;
            }
        }
        
        public void ShowRenderer(string id)
        {
            InspectorItemRenderer renderer = LookForRenderer(id);
            if (renderer != null)
            {
                renderer.hidden = false;
            }
        }

        public InspectorItemRenderer LookForRenderer(string id)
        {
            return InspectorItemRenderer.LookForRenderer(id, renderers.ToArray());
        }

        protected virtual void RetrieveGroupList()
        {
            this.groups = new Groups(new string[]{""});
        }
        
        /// <summary>
        /// Initializes the renderers list and order it. Should be called after <c>InitializeGroups</c> to avoid null error exception when 
        /// ordering the list based on groups.
        /// </summary>
        protected virtual void InitializeRenderersList()
        {
            if (groups == null)
            {
                Debug.LogError("InitializeGroups should be called before InitializeRenderersList since renderers order depends" +
                    " on the groups specified.");
            }
        }

        private void InitializeFullObjectDecoratedRenderers()
        {
            decoratedRenderers = new List<FullObjectDecoratedRenderer>();

            foreach (InspectorItemRenderer renderer in renderers)
            {
                decoratedRenderers.Add(new FullObjectDecoratedRenderer(renderer, this));
            }
        }

        private void InitializeGroupDescription()
        {
            if (renderers == null)
            {
                Debug.LogError("InitializeRenderersList should be called before InitializeGroupDescription since group description" +
                    " is described in renderers attributes");
            }

            foreach (InspectorItemRenderer renderer in renderers)
            {
                if(!string.IsNullOrEmpty(renderer.inspectorAttribute.groupDescription))
                {
                    groups.SetGroupDescription(renderer.inspectorAttribute.group, renderer.inspectorAttribute.groupDescription);
                }
            }
        }

        private void InitializeGroupProperties()
        {
            if (renderers == null)
            {
                Debug.LogError("InitializeRenderersList should be called before InitializeGroupProperties since group properties" +
                               " are described in renderers attributes");
            }

            foreach (InspectorItemRenderer renderer in renderers)
            {
                if(renderer.inspectorAttribute.displayHeader == false)
                {
                    groups.SetGroupDisplayHeader(renderer.inspectorAttribute.group, false);
                }

                if(renderer.inspectorAttribute.foldable == true)
                {
                    groups.SetGroupFoldable(renderer.inspectorAttribute.group, true);

                }
            }
        }
        
        public override void Render (Action preRender = null)
        {
            base.Render (preRender);
            
            EditorGUILayout.BeginVertical();
            
            Group nextGroup = groups[0];
            Group currentGroup = null;

            foreach (FullObjectDecoratedRenderer decoratedRenderer in decoratedRenderers)
            {
                if (nextGroup != null)
                {
                    if (decoratedRenderer.groupName == nextGroup.name)
                    {
                        currentGroup = nextGroup;

                        if (nextGroup.hidden == false)
                        {
                            DrawGroupHeader(nextGroup);
                        }
                        
                        int currentGroupIndex = groups.GetGroupIndex(currentGroup.name);
                        nextGroup = groups[currentGroupIndex + 1];
                    }
                }
                
                if (currentGroup == null || !currentGroup.hidden)
                {
                    if(currentGroup == null || GroupIsFoldout(currentGroup) || !currentGroup.foldable)
                    {
                        if(currentGroup != null && currentGroup.foldable)
                        {
                            EditorGUI.indentLevel += 1 * Settings.indentation;
                        }

                        SetBeginLayout(decoratedRenderer);

                        decoratedRenderer.Render(currentLayoutIsHorizontal);
                        
                        SetEndLayout(decoratedRenderer);

                        if(currentGroup != null && currentGroup.foldable)
                        {
                            EditorGUI.indentLevel -= 1  * Settings.indentation;
                        }
                    }
                }
            }
            
            EditorGUILayout.EndVertical();
        }

        bool firstTimeDrew = true;
        private void DrawGroupHeader(Group group)
        {
            if (!string.IsNullOrEmpty(group.name))
            {
                if(group.displayHeader)
                {
                    GUILayout.Space(10f);

                    if(group.foldable)
                    {
                        if(firstTimeDrew)
                        {
                            firstTimeDrew = false;
                            group.foldout = false;
                        }
                        group.foldout = EditorGUILayout.Foldout(group.foldout, group.name, InspectorStyle.DefaultStyle.foldableGroupHeaderStyle);
                    }
                    else
                    {
                        group.foldout = true;
                        GUILayout.Label(group.name, InspectorStyle.DefaultStyle.groupHeaderStyle);
                    }

                    if(!string.IsNullOrEmpty(group.description))
                    {
                        GUILayout.Label(group.description, InspectorStyle.DefaultStyle.groupDescriptionStyle);
                    }

                    GUILayout.Space(15f);
                }
                else
                {
                    GUILayout.Space(25f);
                }
            }
        }
        
        private bool GroupIsFoldout(Group group)
        {
            return group.foldable && group.foldout;
        }
        
        private void SetBeginLayout(FullObjectDecoratedRenderer subRenderer)
        {
            if(subRenderer.horizontalLayout == HorizontalLayout.BeginHorizontal)
            {
                EditorGUILayout.BeginHorizontal();
                currentLayoutIsHorizontal = true;
            }
            
            if(subRenderer.verticalLayout == VerticalLayout.BeginVertical)
            {
                EditorGUILayout.BeginVertical();
            }
        }
        
        private void SetEndLayout(FullObjectDecoratedRenderer subRenderer)
        {
            if(subRenderer.horizontalLayout == HorizontalLayout.EndHorizontal)
            {
                EditorGUILayout.EndHorizontal();
                currentLayoutIsHorizontal = false;
            }
            
            if(subRenderer.verticalLayout == VerticalLayout.EndVertical)
            {
                EditorGUILayout.EndVertical();
            }
        }

        /// <summary>
        /// Sets the visibility of a renderer based on the attribute [Visibility(string id, object value)]. If the object with the id 'id' 
        /// has the value 'value', the the renderer holding the attribute is visible, otherwise it is not display in the inspector.
        /// </summary>
        protected virtual void SetVisibility()
        {
            return;
        }

        /// <summary>
        /// Gets the message renderer for <c>InspectorItemRenderer</c> with the attribute MessageAttribute.
        /// Since messages can be displayed based on other fields or condition external to a specific
        /// InspectorItemRenderer, the renderer in charge of rendering a group of renderers (like ScriptObjectRenderer or
        /// InlineClassRenderer) needs to implement this function.
        /// </summary>
        /// <returns>The message renderer created to render MessageAttribute in the inspector.</returns>
        /// <param name="renderer">An item renderer.</param>
        protected virtual MessageRenderer[] GetMessageRenderers(InspectorItemRenderer renderer)
        {
            return null;
        }

        static public void GetContextualObjects(InspectorItemRenderer renderer, FullObjectRenderer contextRenderer, out object caller, out object classFieldBelongTo)
        {
            if(contextRenderer is InlineClassRenderer)
            {
                caller = ((InlineClassRenderer) contextRenderer).subtarget;
                classFieldBelongTo = ((InlineClassRenderer) contextRenderer).subtarget;
            }
            else if(contextRenderer is FullObjectRenderer)
            {
                caller = contextRenderer.serializedObject.targetObject;
                classFieldBelongTo = contextRenderer.serializedObject.targetObject;

                if(renderer.entityInfo.isMethod)
                {
                    if(typeof(EasyEditorBase).IsAssignableFrom(renderer.entityInfo.methodInfo.DeclaringType))
                    {
                        caller = ((ScriptObjectRenderer) contextRenderer).editorScript;
                    }
                }


            }
            else
            {
                caller = null;
                classFieldBelongTo = null;
            }
        }
    }
}