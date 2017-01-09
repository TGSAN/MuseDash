//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEditor;
using UnityEngine;
using UEObject = UnityEngine.Object;
using System;

namespace EasyEditor
{
    public class FullObjectDecoratedRenderer 
    {
        public string groupName{get; private set;}
        public HorizontalLayout horizontalLayout{get; private set;}
        public VerticalLayout verticalLayout{get; private set;}

        private FullObjectRenderer contextRenderer;
        private InspectorItemRenderer renderer;
        private MessageRenderer[] messageRenderers;
        private VisibilitySetter visibilitySetter;

        private Action preRenderOperations = null;
        private Action postRenderOperations = null;

        private Action drawMessage = null;
        private Action optimizeLabelWidth = null;
        private Action cancelOptimizeLabelWidth = null;

        public FullObjectDecoratedRenderer(InspectorItemRenderer renderer, FullObjectRenderer contextRenderer)
        {
            this.renderer = renderer;
            this.contextRenderer = contextRenderer;

            groupName = renderer.inspectorAttribute.group;
            InitializeLayouts();

            visibilitySetter = VisibilitySetter.GetVisibilitySetter(renderer, contextRenderer);
            InitializeMessageRenderers();
        }

        private void InitializeMessageRenderers()
        {
            messageRenderers = MessageRenderer.GetMessageRenderers(renderer, contextRenderer);
            foreach(MessageRenderer messageRenderer in messageRenderers)
            {
                drawMessage += messageRenderer.Render;
            }

            #pragma warning disable 162
            if(Settings.messageRenderPosition == Settings.RenderPosition.Above)
            {
                preRenderOperations += drawMessage;
            }
            else if(Settings.messageRenderPosition == Settings.RenderPosition.Below)
            {
                postRenderOperations += drawMessage;
            }
            #pragma warning restore 162
        }

        public void Render(bool currentLayoutIsHorizontal)
        {
            visibilitySetter.SetVisibility();
            if(IsVisible())
            {
                EditorGUILayout.BeginVertical();

                if(currentLayoutIsHorizontal)
                {
                    OptimizeLabelWidth();
                }

                renderer.Render(preRenderOperations);
                renderer.PostRender(postRenderOperations);
                
                
                EditorGUILayout.EndVertical();
            }
        }

        private bool IsVisible()
        {
            return !renderer.hidden;
        }

        private void OptimizeLabelWidth()
        {
            float oldLabelWidth = EditorGUIUtility.labelWidth;

            if(!string.IsNullOrEmpty(renderer.GetLabel()))
            {
                optimizeLabelWidth = () => 
                {
                    var textDimensions = GUI.skin.label.CalcSize(new GUIContent(renderer.GetLabel()));
                    EditorGUIUtility.labelWidth = textDimensions.x + 34f;
                };
                
                cancelOptimizeLabelWidth = () => 
                {
                    EditorGUIUtility.labelWidth = oldLabelWidth;
                };

                preRenderOperations += optimizeLabelWidth;
                postRenderOperations += cancelOptimizeLabelWidth;
            }
        }

        private void InitializeLayouts()
        {
            horizontalLayout = HorizontalLayout.None;
            verticalLayout = VerticalLayout.None;

            if (AttributeHelper.GetAttribute<BeginHorizontalAttribute> (renderer.entityInfo) != null) 
            {
                this.horizontalLayout = HorizontalLayout.BeginHorizontal;
            }
            else if (AttributeHelper.GetAttribute<EndHorizontalAttribute> (renderer.entityInfo) != null) 
            {
                this.horizontalLayout = HorizontalLayout.EndHorizontal;
            }
            
            if (AttributeHelper.GetAttribute<BeginVerticalAttribute> (renderer.entityInfo) != null) 
            {
                this.verticalLayout = VerticalLayout.BeginVertical;
            }
            else if (AttributeHelper.GetAttribute<EndVerticalAttribute> (renderer.entityInfo) != null) 
            {
                this.verticalLayout = VerticalLayout.EndVertical;
            }
        }
    }
}