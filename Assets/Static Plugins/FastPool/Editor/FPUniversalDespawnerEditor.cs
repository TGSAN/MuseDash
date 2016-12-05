using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FPUniversalDespawner))]
public class FPUniversalDespawnerEditor : Editor
{
    SerializedProperty despawnDelayed;
    SerializedProperty delay;
    SerializedProperty despawnOnParticlesDead;
    SerializedProperty despawnOnAudioSourceStop;

    bool isExpanded;
    bool haveParticles;
    bool haveAudioSources;

    void OnEnable()
    {
        //find components. GetComponentsInChildren used beacause it works on prefabs too.
        haveParticles = ((Component)target).GetComponentsInChildren<ParticleSystem>(true).Length > 0;
        haveAudioSources = ((Component)target).GetComponentsInChildren<AudioSource>(true).Length > 0;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        #region Serialized properties
        SerializedProperty targetPoolID = serializedObject.FindProperty("targetPoolID");
        SerializedProperty despawnDelayed = serializedObject.FindProperty("despawnDelayed");
        SerializedProperty delay = serializedObject.FindProperty("delay");
        SerializedProperty despawnOnParticlesDead = serializedObject.FindProperty("despawnOnParticlesDead");
        SerializedProperty resetParticleSystem = serializedObject.FindProperty("resetParticleSystem");
        SerializedProperty despawnOnAudioSourceStop = serializedObject.FindProperty("despawnOnAudioSourceStop");
        #endregion

        GUILayout.Space(10);

        EditorGUILayout.PropertyField(targetPoolID, new GUIContent("Target Pool ID", "ID of the pool to despawn this game object into"));
        GUILayout.Space(5); 
        GUILayout.Label("Despawn conditions [OR]", EditorStyles.objectFieldThumb);
        
        EditorGUILayout.BeginVertical("helpBox");
        EditorGUILayout.PropertyField(despawnDelayed, new GUIContent("Delay", "Despawn the object after the specified period of time"));

        if (despawnDelayed.boolValue)
        {
            EditorGUILayout.PropertyField(delay, new GUIContent("(seconds)", "Despawn the object after the specified period of time"));
            if (delay.floatValue < 0)
                delay.floatValue = 0;

            GUILayout.Space(5);
        }

        EditorGUILayout.PropertyField(despawnOnParticlesDead, new GUIContent("Particles Dead", "Despawn the object after the particles on this GameObject will die"));

        if (despawnOnParticlesDead.boolValue)
        {
            EditorGUILayout.PropertyField(resetParticleSystem, new GUIContent("Reset On Despawn", "Reset particle system on despawn. Useful when the game object will be despawned before than all particles will die"));
            GUILayout.Space(5);
        }

        EditorGUILayout.PropertyField(despawnOnAudioSourceStop, new GUIContent("AudioSource Stop", "Despawn the object after the AudioSource on this GameObject will stop playing"));


        #region Help messages
        //if (sourcePrefab.objectReferenceValue == null)
        //    EditorGUILayout.HelpBox("Source Prefab is null. Please provide the same prefab as you set in your pool in the 'SourcePrefab' field", MessageType.Error);
        //if (!initedFromPool.boolValue)
        //    EditorGUILayout.HelpBox("Don't add this component manually. Use the 'Add Auto Despawner' button in the Pool inspector instead.", MessageType.Error);

        if (despawnDelayed.boolValue && Mathf.Approximately(delay.floatValue, 0))
            EditorGUILayout.HelpBox("Delay is set to '0' so gameObject will be despawned immediately after spawn!", MessageType.Warning);

        if (despawnOnParticlesDead.boolValue && !haveParticles)
            EditorGUILayout.HelpBox("You don't have any particle system on current or child GameObjects", MessageType.Error);

        if (despawnOnAudioSourceStop.boolValue && !haveAudioSources)
            EditorGUILayout.HelpBox("You don't have any AudioSource on current or child GameObjects", MessageType.Error);
        #endregion

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
