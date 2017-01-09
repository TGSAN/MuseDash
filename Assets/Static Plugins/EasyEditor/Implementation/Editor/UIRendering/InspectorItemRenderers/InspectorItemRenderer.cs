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
    /// <summary>
    /// Can be called by an <c>EasyEditorBase</c> script to render in the inspector.
    /// </summary>
    abstract public class InspectorItemRenderer : ScriptableObject
    {
		/// <summary>
		/// Gets the attribute specifying the group to which belongs this renderer and other indication on how to render it.
		/// </summary>
		/// <value>
		/// The attribute.
		/// </value>
		public InspectorAttribute inspectorAttribute {
			get{return _inspectorAttribute;}
		}

		[SerializeField] protected InspectorAttribute _inspectorAttribute;

		/// <summary>
		/// Gets the entity info (field info or method info) which was used to initialize the renderer.
		/// </summary>
		/// <value>The entity info.</value>
		public EntityInfo entityInfo {
			get{return _entityInfo;}
		}

		[SerializeField] protected EntityInfo _entityInfo;

        /// <summary>
        /// The serialized object the renderer is rendering in the inspector.
        /// </summary>
        public SerializedObject serializedObject{ 
            get
            {
                return entityInfo.serializedObject;
            }
        }

        /// <summary>
        /// If set to false, this renderer will not be rendered.
        /// </summary>
        public bool hidden = false;

		/// <summary>
		/// Comment that should be displayed below the item if a comment attribute was specified.
		/// </summary>
		public string comment = "";
		
		/// <summary>
		/// Gets the identifier of a renderer. It is used when looking for a renderer to hide for example. By default, the id returned is the name of the field or the method.
		/// This id can be replaced by a custom one in the attribute Inspector.
		/// </summary>
		/// <returns></returns>
		public string GetIdentifier()
		{
			string identifier = inspectorAttribute.id;
			if(string.IsNullOrEmpty(identifier))
			{
				identifier = entityInfo.ToString();
			}

			return identifier;
		}

        /// <summary>
        /// Gets the label of the renderer if it is relevant to the renderer.
        /// </summary>
        /// <returns>The label.</returns>
        virtual public string GetLabel()
        {
            return "";
        }

        /// <summary>
        /// Creates an instance of the renderer. Renderers are <c>ScriptableObject</c> because for later development, 
        /// we may saved them as unity assets to allow users to create interfaces in a wysiwyg fashion.
        /// </summary>
        /// <typeparam name="T">The type of renderer you want to create.</typeparam>
        /// <returns>The renderer ready to be rendered in the inspector.</returns>
        static public T CreateRenderer<T>() where T : InspectorItemRenderer
        {
            return ScriptableObject.CreateInstance<T>();
        }

        /// <summary>
        ///Creates an instance of the renderer. Renderers are <c>ScriptableObject</c> because for later development, 
        /// we may saved them as unity assets to allow users to create interfaces in a wysiwyg fashion.
        /// </summary>
        /// <param name="classType">The type of renderer you want to create.</param>
        /// <returns>The renderer ready to be rendered in the inspector.</returns>
        static public InspectorItemRenderer CreateRenderer(Type classType)
        {
            return (InspectorItemRenderer) ScriptableObject.CreateInstance(classType);
        }

        /// <summary>
        /// Creates an asset to save the renderer in the editor if necessary.
        /// </summary>
        /// <param name="path">The path starting with "Assets/".</param>
        virtual public void CreateAsset(string path)
        {
            Utils.CreateAssetFrom<InspectorItemRenderer>(this, "renderer.asset name", path);
        }

		virtual public void InitializeFromEntityInfo(EntityInfo entityInfo)
		{
			_entityInfo = entityInfo;
			InitializeInspectorAttribute ();
			InitializeCommentAttribute ();
		}

		protected void InitializeInspectorAttribute()
		{
			InspectorAttribute inspectorAttribute = AttributeHelper.GetAttribute<InspectorAttribute> (entityInfo);
			if (inspectorAttribute != null)
			{
				_inspectorAttribute = inspectorAttribute;
			}
			else
			{
				_inspectorAttribute = InspectorAttribute.DefaultInspectorAttribute;
			}
		}

		protected void InitializeCommentAttribute()
		{
			CommentAttribute commentAttribute = AttributeHelper.GetAttribute<CommentAttribute> (entityInfo);
			if (commentAttribute != null)
			{
				this.comment = commentAttribute.comment;
			}
		}

        /// <summary>
        /// Function to be called to render the UI related to the renderer delegate. Should always be called after the editor script delegator is set since
        /// the renderer usually needs to access the editor target. User can inject an optional delegate to render some custom code (usually used for UI Layout).
        /// </summary>
        /// <param name="preRender">Optional delegate injected by user and executed just before the core code of the function.</param>
        virtual public void Render(Action preRender = null)
        {
            if (preRender != null)
            {
                preRender();
            }

#pragma warning disable 162
            if (Settings.commentRenderPosition == Settings.RenderPosition.Above)
            {
                RenderComment();
            }
#pragma warning restore 162

            if(entityInfo.serializedObject == null)
            {
                Debug.LogWarning("Your InspectorItemRenderer for the entity info " + entityInfo + " does not have a serialized object set. This could raise errors in the rendering process.");
            }
        }

		/// <summary>
        /// Function called after the item was rendered. Used usually to display comments related to the property. User can inject an optional delegate to render 
        /// some custom code (usually used to revert some UI Layout).
		/// </summary>
        /// <param name="prePostRender">Optional delegate injected by user and executed just before the core code of the function.</param>
		virtual public void PostRender(Action prePostRender = null)
		{
            #pragma warning disable 162
            if (Settings.commentRenderPosition == Settings.RenderPosition.Below)
            {
                RenderComment();
            }
            #pragma warning restore 162

            if (prePostRender != null)
            {
                prePostRender();
            }
		}

		private void RenderComment()
		{
			if(!string.IsNullOrEmpty(comment))
			{
				EditorGUILayout.HelpBox(comment, UnityEditor.MessageType.Info, true);
			}
		}

        #region InspectorItemRenderer Creators
		/// <summary>
		/// Gets a renderer from field info.
		/// </summary>
		/// <returns>A renderer created from field info.</returns>
		/// <param name="fieldInfo">Field info.</param>
		static public InspectorItemRenderer GetRendererFromFieldInfo(FieldInfo fieldInfo, SerializedObject serializedObject, string propertyPath = "")
		{
			return GetRendererFromEntityInfo (new EntityInfo (fieldInfo, serializedObject, propertyPath));
		}

		/// <summary>
		/// Gets a renderer from method info and the object supposed to call it.
		/// </summary>
		/// <returns>A renderer created from method info.</returns>
		/// <param name="methodInfo">The method to render from.</param>
		/// <param name="caller">The object from which the method should be called.</param>
		static public InspectorItemRenderer GetRendererFromMethodInfo(MethodInfo methodInfo, object caller, SerializedObject serializedObject)
		{
            return GetRendererFromEntityInfo (new EntityInfo (methodInfo, caller, serializedObject));
		}

		/// <summary>
		/// Gets a renderer from a field/method info. Will look first for the renderer type specified in the field info attribute. If it was not specified, will look for
		/// the default renderer for this field/meothod info.
		/// </summary>
		/// <returns>The renderer generated from the entity info.</returns>
		/// <param name="entityInfo">Entity info.</param>
		/// <param name="caller">The object from which the method should be called if entity info is wrapping a methodInfo.</param>
        static private InspectorItemRenderer GetRendererFromEntityInfo(EntityInfo entityInfo)
        {
            InspectorItemRenderer renderer = null;
			InspectorAttribute inspectorAttribute = AttributeHelper.GetAttribute<InspectorAttribute> (entityInfo);

            if (inspectorAttribute != null)
            {
				if(inspectorAttribute.rendererType != null)
				{
                	Type renderClassType = Type.GetType("EasyEditor." + inspectorAttribute.rendererType);
                	renderer = InspectorItemRenderer.GetSpecifiedRendererFromEntityInfo(entityInfo, renderClassType);
				}
				else
				{
					renderer = InspectorItemRenderer.GetDefaultRendererFromEntityInfo(entityInfo);
				}
            }
            else
            {
				if(entityInfo.isField)
				{
					renderer = InspectorItemRenderer.GetDefaultRendererFromEntityInfo(entityInfo);
				}
            }

            return renderer;
        }

		/// <summary>
		/// Gets a renderer specified by a type and initialize it with an entity info (methodInfo or field info wrapper).
		/// </summary>
		/// <returns>The specified renderer from entity info.</returns>
		/// <param name="entityInfo">the entity info (methodInfo or fieldInfo wrapper).</param>
		/// <param name="type">The type of <c>InspectorItemRenderer<c/> to use.</param>
		/// <param name="caller">The object from which the method should be called if entity info is wrapping a methodInfo.</param>
		static private InspectorItemRenderer GetSpecifiedRendererFromEntityInfo(EntityInfo entityInfo, Type type)
		{
			InspectorItemRenderer result = CreateRenderer(type);
			result.InitializeFromEntityInfo(entityInfo);
			
			return result;
		}

		/// <summary>
		/// Gets a default renderer for the entity info. Called when no type for the renderer was specified in <c>InspectorAttribute</c> for example
		/// </summary>
	    /// <returns>The default renderer for the specified entity info.</returns>
		/// <param name="entityInfo">The entity info which needs a renderer.</param>
		/// <param name="caller">The object from which the method should be called if entity info is wrapping a methodInfo.</param>
		static private InspectorItemRenderer GetDefaultRendererFromEntityInfo(EntityInfo entityInfo)
		{
			InspectorItemRenderer renderer = null;
			Type rendererType = null;
			
			if (entityInfo.isMethod) 
			{
				rendererType = Type.GetType ("EasyEditor.ButtonRenderer");
			} 
			else if(entityInfo.isField)
			{
				rendererType = GetDefaultRendererTypeForField(entityInfo.fieldInfo);
			}
			
			renderer = InspectorItemRenderer.GetSpecifiedRendererFromEntityInfo(entityInfo, rendererType);
			return renderer;
		}

		static private Type GetDefaultRendererTypeForField(FieldInfo fieldInfo)
		{
			Type result = null;

			IEnumerable<Type> collection = Utils.FindSubClassesOf<InspectorItemRenderer>();
			foreach (Type inspectorItemRendererType in collection)
			{
				RenderTypeAttribute[] renderTypeAttributes = AttributeHelper.GetAttributes<RenderTypeAttribute>(inspectorItemRendererType);
				if (renderTypeAttributes != null)
				{
					foreach (RenderTypeAttribute renderTypeAttribute in renderTypeAttributes)
					{
						if (fieldInfo.FieldType == renderTypeAttribute.type || renderTypeAttribute.type.IsAssignableFrom(fieldInfo.FieldType))
						{
							result = inspectorItemRendererType;
							break;
						}
					}
				}
			}
			
			if (result == null && FieldInfoHelper.HasSerializableAttribute(fieldInfo.FieldType))
			{
				result = typeof(SerializedFieldRenderer);
			}
			
			if (result == null)
			{
				Debug.LogWarning("The field '" + fieldInfo.Name + "' does not fit any renderer. If you want this type of field to be rendered, ensure that " +
				                 "a class inheriting from InspectorItemRenderer have the attribute RenderType with the type of the field as argument OR " +
				                 "add [Inspector(rendererType = \" class name of the renderer to use\" on top of the field, Ex : [Inspector(rendererType = \"SerializedFieldRenderer\"].");
			}

			return result;
		}
        #endregion

        #region static helper functions
        /// <summary>
        /// Looks for renderer in the renderers list based on an id. The default id is the field or the method name of a renderer.
        /// But this id can be modified with Inspector attribute.
        /// </summary>
        /// <returns>The found renderer.</returns>
        /// <param name="rendererId">Renderer identifier.</param>
        /// <param name="rendererList">The list of Renderers to look in.</param>
        public static InspectorItemRenderer LookForRenderer(string rendererId, InspectorItemRenderer[] rendererList)
        {
            InspectorItemRenderer result = null;
            foreach (InspectorItemRenderer renderer in rendererList)
            {
                if (renderer.GetIdentifier() == rendererId)
                {
                    result = renderer;
                    break;
                }
            }
            
            return result;
        }
        #endregion
    }	
}