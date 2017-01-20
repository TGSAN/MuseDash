using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(BlurBehind))]
[CanEditMultipleObjects]
public class BlurBehindEditor : Editor {

    SerializedProperty blurShaderProp;

    SerializedProperty modeProp;
    SerializedProperty radiusProp;
    SerializedProperty settingsProp;
    SerializedProperty downsampleProp;
    SerializedProperty iterationsProp;
    SerializedProperty cropRectProp;
    SerializedProperty pixelOffsetProp;

    bool nonfullscreen;
    Rect storedCropRect;
    Rect storedPixelOffset;

    void OnEnable()
    {
        blurShaderProp = serializedObject.FindProperty("blurShader");
        modeProp = serializedObject.FindProperty("mode");
        radiusProp = serializedObject.FindProperty("radius");
        settingsProp = serializedObject.FindProperty("settings");
        downsampleProp = serializedObject.FindProperty("downsample");
        iterationsProp = serializedObject.FindProperty("iterations");
        cropRectProp = serializedObject.FindProperty("cropRect");
        pixelOffsetProp = serializedObject.FindProperty("pixelOffset");

        // Default values
        storedCropRect = new Rect(0f, 0f, 1f, 1f);
        storedPixelOffset = new Rect(0f, 0f, 0f, 0f);

        nonfullscreen = cropRectProp.rectValue != storedCropRect || pixelOffsetProp.rectValue != storedPixelOffset;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(blurShaderProp, new GUIContent("Blur Shader"));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(modeProp, new GUIContent("Mode"));

        int mode;
        if (modeProp.hasMultipleDifferentValues)
        {
            mode = -1;
        }
        else
        {
            mode = modeProp.enumValueIndex;
        }

        EditorGUILayout.PropertyField(radiusProp, new GUIContent("Blur Radius" + (mode == 0 ? " (px)" : (mode == 1 ? " (%)" : string.Empty))));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(settingsProp, new GUIContent("Settings"));

        GUI.enabled = !settingsProp.hasMultipleDifferentValues && settingsProp.enumValueIndex == 2;

        EditorGUILayout.PropertyField(downsampleProp, new GUIContent("Downsample" + (mode == 0 ? " By" : (mode == 1 ? " To" : string.Empty))));
        EditorGUILayout.PropertyField(iterationsProp, new GUIContent("Blur Iterations"));

        GUI.enabled = true;

        EditorGUILayout.Space();

        if (EditorGUILayout.ToggleLeft("Crop", nonfullscreen) != nonfullscreen)
        {
            if (nonfullscreen)
            {
                nonfullscreen = false;
                storedCropRect = cropRectProp.rectValue;
                storedPixelOffset = pixelOffsetProp.rectValue;
                cropRectProp.rectValue = new Rect(0f, 0f, 1f, 1f);
                pixelOffsetProp.rectValue = new Rect(0f, 0f, 0f, 0f);
            }
            else
            {
                nonfullscreen = true;
                cropRectProp.rectValue = storedCropRect;
                pixelOffsetProp.rectValue = storedPixelOffset;
            }
        }

        if (nonfullscreen)
        {
            EditorGUILayout.PropertyField(cropRectProp, new GUIContent("Normalized Rect"));
            EditorGUILayout.PropertyField(pixelOffsetProp, new GUIContent("Pixel Offsets"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
