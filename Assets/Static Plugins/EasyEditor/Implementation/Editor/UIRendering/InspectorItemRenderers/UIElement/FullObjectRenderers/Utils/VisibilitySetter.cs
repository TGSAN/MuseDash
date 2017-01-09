//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEngine;
using System.Collections;
using System.Reflection;

namespace EasyEditor
{
    public class VisibilitySetter {

        InspectorItemRenderer inspectorItemRenderer;

        VisibilityAttribute visibilityAttribute;
        string method = "";
        string id = "";
        object value;
        
        InspectorItemRenderer[] otherRenderers;
        object caller;
        object classFieldBelongTo;

        public VisibilitySetter(InspectorItemRenderer inspectorItemRenderer, object caller, object classFieldBelongTo, InspectorItemRenderer[] otherRenderers = null)
        {
            visibilityAttribute = AttributeHelper.GetAttribute<VisibilityAttribute>(inspectorItemRenderer.entityInfo);

            if(visibilityAttribute != null)
            {
                this.method = visibilityAttribute.method;
                this.id = visibilityAttribute.id;
                this.value = visibilityAttribute.value;
            }

            this.caller = caller;
            this.classFieldBelongTo = classFieldBelongTo;
            this.inspectorItemRenderer = inspectorItemRenderer;
            this.otherRenderers = otherRenderers;
        }

        public void SetVisibility()
        {
            bool renderRenderer = !inspectorItemRenderer.hidden;

            if(inspectorItemRenderer.entityInfo.isField 
               && AttributeHelper.GetAttribute<HideInInspector>(inspectorItemRenderer.entityInfo.fieldInfo) != null)
            {
                renderRenderer = false;
            }
            else if (!string.IsNullOrEmpty(method))
            {
                MethodInfo methodInfo = caller.GetType().GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if(methodInfo != null)
                {
                    if(methodInfo.ReturnType == typeof(bool))
                    {
                        renderRenderer = (bool) methodInfo.Invoke(caller, null);
                    }
                    else
                    {
                        Debug.LogError("The method specified in the attribute Visibility have to return a bool.");
                    }
                }
                else
                {
                    Debug.LogError("The method specified in the attribute Visibility does not exist.");
                }
            }
            else if(!string.IsNullOrEmpty(id) && value != null)
            {
                InspectorItemRenderer conditionalRenderer = InspectorItemRenderer.LookForRenderer(id, otherRenderers);
                if(conditionalRenderer != null && conditionalRenderer.entityInfo.isField)
                {
                    if(!value.Equals(conditionalRenderer.entityInfo.fieldInfo.GetValue(classFieldBelongTo)))
                    {
                        renderRenderer = false;
                    }
                    else
                    {
                        renderRenderer = true;
                    }
                }
                else
                {
                    Debug.LogWarning("The identifier " + id + " was not found in the list of renderers, or this renderer " +
                                     "was not initialized from a field. Ensure that the id parameter of the attribute Visibility refers to the id of a field " +
                                     "(name of the field if you did not specify explicitly the id of the field in [Inspector(id = \"...\").");
                }
            }
            
            if (renderRenderer)
            {
                inspectorItemRenderer.hidden = false;
            }
            else
            {
                inspectorItemRenderer.hidden = true;
            }
        }

        /// <summary>
        /// Sets the visibility of a renderer based on the attribute [Visibility]. 
        /// If VisibilityAttribute parameters id and value are not null, and the object with the id 'id' 
        /// has the value 'value', then the renderer holding the attribute is visible, otherwise it is not display in the inspector.
        /// If method is not empty, then if the class where the visibility attribute is used has this function and this function return true,
        /// the renderer will be rendered.
        /// </summary>
        public static VisibilitySetter GetVisibilitySetter(InspectorItemRenderer renderer, FullObjectRenderer contextRenderer)
        {
            object caller = null;
            object classFieldBelongTo = null;
            
            FullObjectRenderer.GetContextualObjects(renderer, contextRenderer, out caller, out classFieldBelongTo);

            return new VisibilitySetter(renderer, caller, classFieldBelongTo, contextRenderer.renderers.ToArray());
        }
    }
}