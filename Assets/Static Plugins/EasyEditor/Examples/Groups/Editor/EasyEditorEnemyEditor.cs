using UnityEditor;
using UnityEngine;
using System.Collections;

namespace EasyEditor
{
    /// <summary>
    /// Easy editor ennemy editor. Groups in the inspector are rendered in the same order are they are declared in
    /// the attribute [Groups]
    /// </summary>
    [Groups("Game Designer Settings", "Basic Settings", "Advanced Settings")]
    [CustomEditor(typeof(EasyEditorEnemy))]
    public class EasyEditorEnemyEditor : EasyEditorBase
    {

    }
}