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
using System.Reflection;

namespace EasyEditor
{
    /// <summary>
    /// Render a field that can be serialized by unity by default (object deriving from IList are handled by <c>SimpleListRenderer</c>
    /// </summary>
    [RenderType(typeof(int))]
    [RenderType(typeof(float))]
    [RenderType(typeof(string))]
    [RenderType(typeof(bool))]
    [RenderType(typeof(UEObject))]
    [RenderType(typeof(Vector2))]
    [RenderType(typeof(Vector3))]
    [RenderType(typeof(Vector4))]
    [RenderType(typeof(Quaternion))]
    [RenderType(typeof(Matrix4x4))]
    [RenderType(typeof(Bounds))]
    [RenderType(typeof(Rect))]
    [RenderType(typeof(LayerMask))]
    [RenderType(typeof(System.Enum))]
    [RenderType(typeof(Color))]
    [RenderType(typeof(Color32))]
    [RenderType(typeof(AnimationCurve))]
    [RenderType(typeof(Gradient))]

    public class SerializedFieldRenderer : FieldRenderer
    {
        public override void CreateAsset(string path)
        {
            Utils.CreateAssetFrom<SerializedFieldRenderer>(this, "SerializedField_" + label, path);
        }

        public override void Render(Action preRender = null)
        {
            base.Render(preRender);

            directParentSerializedObject.Update();
            EditorGUILayout.PropertyField(serializedProperty, new GUIContent(label), true, GUILayout.MinWidth(30f)); 
            directParentSerializedObject.ApplyModifiedProperties();
        }
	}
}