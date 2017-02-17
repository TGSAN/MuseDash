﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.Commons
{
    public class EditorUtils
    {
        public static Rect MakeObjectField(GameObject go, SerializedProperty property, string pptName, GUIContent contentName, Rect rect, float gap, float height, bool isEnum = false, GUIStyle style = null, params string[] others)
        {
            var names = new List<string>();
            var asType = typeof(string);
            if (go != null)
            {
                var allMono = go.GetComponents<MonoBehaviour>().ToList();
                allMono.ForEach(m =>
                {
                    var type = m.GetType();
                    var allField = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    names.AddRange(allField.Select(f =>
                    {
                        var field = f.IsPrivate ? "private/" : "public/";
                        return type.Name + "/" + field + f.Name;
                    }).ToArray());
                });
            }

            var allPpt = typeof(GameObject).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => !p.GetIndexParameters().Any<ParameterInfo>() && asType.IsAssignableFrom(p.PropertyType));
            names.AddRange(allPpt.Select(p => "GameObject/" + p.Name).ToArray());

            rect = EditorUtils.MakePopupField(property, pptName, contentName,
            names.ToArray(), rect, gap, height, isEnum, style, false, others);
            return rect;
        }

        public static Rect MakePopupField(SerializedProperty property, string pptName, GUIContent contentName, string[] strs, Rect rect, float gap, float height, bool isEnum = false, GUIStyle style = null, bool underline = false, params string[] others)
        {
            rect = new Rect(rect.x, rect.y + height + gap, rect.width, rect.height);

            var nameProperty = property.FindPropertyRelative(pptName);
            var idx = isEnum ? nameProperty.enumValueIndex : strs.ToList().FindIndex(s => s == nameProperty.stringValue);
            if (underline)
            {
                for (int i = 0; i < strs.Length; i++)
                {
                    strs[i] = strs[i].Replace("_", "/");
                }
            }
            var contents = EditorUtils.GetGUIContentArray(strs, others);
            idx = idx == -1 ? contents.Length - 1 : idx;

            var nameIdx = 0;
            nameIdx = style == null ? EditorGUI.Popup(rect, contentName, idx, contents) : EditorGUI.Popup(rect, contentName, idx, contents, style);

            if (nameIdx < contents.Length)
            {
                if (isEnum)
                {
                    nameProperty.enumValueIndex = nameIdx;
                }
                else
                {
                    if (underline)
                    {
                        foreach (GUIContent t in contents)
                        {
                            t.text = t.text.Replace("/", "_");
                        }
                    }
                    nameProperty.stringValue = contents[nameIdx].text;
                }
            }
            return rect;
        }

        public static Rect MakeLabelField(GUIContent content, Rect rect, float gap, float height, GUIStyle style = null)
        {
            rect = new Rect(rect.x, rect.y + height + gap, rect.width, rect.height);
            if (style != null)
            {
                EditorGUI.LabelField(rect, content, style);
            }
            else
            {
                EditorGUI.LabelField(rect, content);
            }

            return rect;
        }

        public static Rect MakePropertyField(string name, SerializedProperty mainProperty, Rect rect, float gap, float height, GUIContent content = null)
        {
            var property = mainProperty.FindPropertyRelative(name);
            return MakePropertyField(property, rect, gap, height, content);
        }

        public static Rect MakePropertyField(SerializedProperty property, Rect rect, float gap, float height, GUIContent content = null)
        {
            rect = new Rect(rect.x, rect.y + rect.height + gap, rect.width, height);
            EditorGUI.PropertyField(rect, property, content);
            return rect;
        }

        public static Rect MakePropertyField(string name, SerializedProperty mainProperty, Rect rect)
        {
            var property = mainProperty.FindPropertyRelative(name);
            EditorGUI.PropertyField(rect, property);
            return rect;
        }

        public static Rect MakeEnumPropertyField(string name, SerializedProperty mainProperty, Rect rect, GUIContent content = null, float gap = 0, float height = 0)
        {
            var property = mainProperty.FindPropertyRelative(name);
            rect = new Rect(rect.x, rect.y + gap + height, rect.width, height);
            property.enumValueIndex = EditorGUI.Popup(rect, content, property.enumValueIndex, EditorUtils.GetGUIContentArray(property.enumNames));
            return rect;
        }

        public static Rect MakeEventPropertyField(string name, SerializedProperty mainProperty, Rect rect, float gap, float height)
        {
            var eventPorperty = mainProperty.FindPropertyRelative(name);
            var eventCount = GetUnityEventCount(eventPorperty);
            var totalHeight = height * (eventCount == 0 ? 1 : eventCount);
            return MakePropertyField(name, mainProperty, rect, gap, totalHeight);
        }

        public static Rect MakeEventPropertyField(SerializedProperty property, Rect rect, float gap, float height = 43, float extraHeight = 30)
        {
            var enterEventCount = GetUnityEventCount(property);
            var totalHeight = height * (enterEventCount == 0 ? 1 : enterEventCount) + 30;
            return MakePropertyField(property, rect, gap, totalHeight);
        }

        public static int GetUnityEventCount(SerializedProperty property)
        {
            return property.FindPropertyRelative("m_PersistentCalls.m_Calls").arraySize;
        }

        public static bool ToggleButton(bool toggled, Rect rect, GUIContent content, GUIStyle guiStyle = null, params GUILayoutOption[] options)
        {
            var color = GUI.backgroundColor;
            GUI.backgroundColor = toggled ? Color.green : Color.white;
            if (GUI.Button(rect, content))
            {
                toggled = !toggled;
                GUI.changed = true;
            }
            GUI.backgroundColor = color;
            return toggled;
        }

        public static GUIContent[] GetGUIContentArray(string[] stringArray, params string[] other)
        {
            GUIContent[] guiContentArray = new GUIContent[stringArray.Length + other.Length];
            for (int i = 0; i < guiContentArray.Length; i++)
            {
                var str = string.Empty;
                str = i < stringArray.Length ? stringArray[i] : other[i - stringArray.Length];
                guiContentArray[i] = new GUIContent(str);
            }

            return guiContentArray;
        }

        public static void DrawCurve(Vector2 startPoint, Vector2 endPoint)
        {
            // Line properties
            float curveThickness = 5f;
            //Texture2D bezierTexture = DialogueEditorGUI.bezierTexture;
            Texture2D bezierTexture = null;

            // Create shadow start and end points
            Vector2 shadowStartPoint = new Vector2(startPoint.x + 1, startPoint.y + 2);
            Vector2 shadowEndPoint = new Vector2(endPoint.x + 1, endPoint.y + 2);

            /*
			// UNCHANGING
			// Calculate tangents based on distance from startPoint to endPoint, 60 being the max
			float tangent = 60;
			int check = (startPoint.x < endPoint.x) ? 1 : -1 ;
			Vector2 startTangent = new Vector2(startPoint.x + tangent, startPoint.y);
			Vector2 endTangent = new Vector2(endPoint.x - tangent, endPoint.y);
			Vector2 shadowStartTangent = new Vector2(shadowStartPoint.x + tangent, shadowStartPoint.y);
			Vector2 shadowEndTangent = new Vector2(shadowEndPoint.x - tangent, shadowEndPoint.y);
			*/

            /*
			// FLIPPY
			// Calculate tangents based on distance from startPoint to endPoint, 60 being the max
			float tangent = 60;
			tangent = (Vector2.Distance(startPoint, endPoint) < tangent) ? Vector2.Distance(startPoint, endPoint) : 60 ;
			int check = (startPoint.x < endPoint.x) ? 1 : -1 ;
			Vector2 startTangent = new Vector2(startPoint.x + (tangent * check), startPoint.y);
			Vector2 endTangent = new Vector2(endPoint.x - (tangent * check), endPoint.y);
			Vector2 shadowStartTangent = new Vector2(shadowStartPoint.x + (tangent * check), shadowStartPoint.y);
			Vector2 shadowEndTangent = new Vector2(shadowEndPoint.x - (tangent * check), shadowEndPoint.y);
			*/

            // Easing
            // Calculate tangents based on distance from startPoint to endPoint, 60 being the max
            float tangent = Mathf.Clamp((-1) * (startPoint.x - endPoint.x), -100, 100);
            Vector2 startTangent = new Vector2(startPoint.x + tangent, startPoint.y);
            Vector2 endTangent = new Vector2(endPoint.x - tangent, endPoint.y);
            Vector2 shadowStartTangent = new Vector2(shadowStartPoint.x + tangent, shadowStartPoint.y);
            Vector2 shadowEndTangent = new Vector2(shadowEndPoint.x - tangent, shadowEndPoint.y);

            /*
			// Easing 2
			// Calculate tangents based on distance from startPoint to endPoint, 60 being the max
			float tangent = Mathf.Clamp((-1)*((startPoint.x - endPoint.x)), -100, 100);
			Vector2 startTangent = new Vector2(startPoint.x + tangent, startPoint.y);
			Vector2 endTangent = new Vector2(endPoint.x - tangent, endPoint.y);
			Vector2 shadowStartTangent = new Vector2(shadowStartPoint.x + tangent, shadowStartPoint.y);
			Vector2 shadowEndTangent = new Vector2(shadowEndPoint.x - tangent, shadowEndPoint.y);
			*/

            /*
			// Easing with directional tangents
			// Calculate tangents based on distance from startPoint to endPoint, 100 being the max
			float tangent = Mathf.Clamp(Vector2.Distance(startPoint, endPoint), -100, 100);
			Vector2 startTangent = new Vector2(startPoint.x + tangent, startPoint.y);
			Vector2 endTangent = new Vector2(endPoint.x - tangent, endPoint.y - (tangent*0.5f));
			Vector2 shadowStartTangent = new Vector2(shadowStartPoint.x + tangent, shadowStartPoint.y);
			Vector2 shadowEndTangent = new Vector2(shadowEndPoint.x - tangent, shadowEndPoint.y - (tangent*0.5f));
			*/

            bool isPro = EditorGUIUtility.isProSkin;

            if (isPro)
            {
                // Draw the shadow first
                Handles.DrawBezier(
                    shadowStartPoint,
                    shadowEndPoint,
                    shadowStartTangent,
                    shadowEndTangent,
                    new Color(0, 0, 0, 0.25f),
                    bezierTexture,
                    curveThickness
                    );
            }
            Handles.DrawBezier(
                startPoint,
                endPoint,
                startTangent,
                endTangent,
                //new Color(0.8f,0.6f,0.3f,0.25f),
                (isPro) ? new Color(0.3f, 0.7f, 0.9f, 0.25f) : new Color(0f, 0.1f, 0.4f, 0.6f),
                bezierTexture,
                curveThickness
                );
        }
    }
}