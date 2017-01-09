//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEngine;
using UEObject = UnityEngine.Object;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace EasyEditor
{
    public class MessageRenderer
    {
        string text = "";
        UnityEditor.MessageType messageType;
        string method = "";
        string id = "";
        object value;
        
        InspectorItemRenderer[] otherRenderers;
        object caller;
        object classFieldBelongTo;
        
        public MessageRenderer(MessageAttribute messageAttribute, object caller, object classFieldBelongTo, InspectorItemRenderer[] otherRenderers = null)
        {
            this.text = messageAttribute.text;
            this.method = messageAttribute.method;
            this.id = messageAttribute.id;
            this.value = messageAttribute.value;
            this.caller = caller;
            this.classFieldBelongTo = classFieldBelongTo;
            
            switch (messageAttribute.messageType)
            {
                case MessageType.Info:
                    this.messageType = UnityEditor.MessageType.Info;
                    break;
                case MessageType.Warning:
                    this.messageType = UnityEditor.MessageType.Warning;
                    break;
                case MessageType.Error:
                    this.messageType = UnityEditor.MessageType.Error;
                    break;
            }
            
            this.otherRenderers = otherRenderers;
        }
        
        public void Render()
        {
            bool renderMessage = true;
            
            if (!string.IsNullOrEmpty(method))
            {
                MethodInfo methodInfo = caller.GetType().GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if(methodInfo != null)
                {
                    if(methodInfo.ReturnType == typeof(bool))
                    {
                        renderMessage = (bool) methodInfo.Invoke(caller, null);
                    }
                    else
                    {
                        Debug.LogError("The method specified in the attribute Message have to return a bool.");
                    }
                }
                else
                {
                    Debug.LogError("The method specified in the attribute Message does not exist.");
                }
            }
            else if(!string.IsNullOrEmpty(id) && value != null)
            {
                InspectorItemRenderer conditionalRenderer = InspectorItemRenderer.LookForRenderer(id, otherRenderers);
                if(conditionalRenderer != null && conditionalRenderer.entityInfo.isField)
                {
                    if(!value.Equals(conditionalRenderer.entityInfo.fieldInfo.GetValue(classFieldBelongTo)))
                    {
                        renderMessage = false;
                    }
                }
                else
                {
                    Debug.LogWarning("The identifier " + id + " was not found in the list of renderers, or this renderer " +
                                     "was not initialized from a field. Ensure that the id parameter of the attribute Visibility refers to the id of a field " +
                                     "(name of the field if you did not specify explicitly the id of the field in [Inspector(id = \"...\").");
                }
            }
            
            if (renderMessage)
            {
                if(!string.IsNullOrEmpty(text))
                {
                    EditorGUILayout.HelpBox(text, messageType, true);
                }
            }
        }
        
        //// <summary>
        /// Gets the message renderer for <c>InspectorItemRenderer</c> with the attribute MessageAttribute.
        /// </summary>
        /// <returns>The message renderer created to render MessageAttribute in the inspector.</returns>
        /// <param name="renderer">An item renderer.</param>
        /// <param name="contextRenderer">The context in which the renderer is rendered.</param>
        public static MessageRenderer[] GetMessageRenderers(InspectorItemRenderer renderer, FullObjectRenderer contextRenderer)
        {
            List<MessageRenderer> result = new List<MessageRenderer>();
            
            object caller = null;
            object classFieldBelongTo = null;

            FullObjectRenderer.GetContextualObjects(renderer, contextRenderer, out caller, out classFieldBelongTo);
            
            MessageAttribute[] messageAttributes = AttributeHelper.GetAttributes<MessageAttribute>(renderer.entityInfo);
            if(messageAttributes != null)
            {
                foreach(MessageAttribute messageAttribute in messageAttributes)
                {
                    result.Add(new MessageRenderer(messageAttribute, caller, classFieldBelongTo, contextRenderer.renderers.ToArray()));
                }
            }   
            
            return result.ToArray();
        }
    }
}