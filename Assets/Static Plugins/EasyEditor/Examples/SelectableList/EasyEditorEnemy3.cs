using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EasyEditor
{
    /// <summary>
    /// Example class introducing how fields can be organized into foldable groups in Easy Editor. Please, note that groups
    /// need to be declared in the corresponding editor script (EasyEditorEnnemyEditor.cs) with the attribute
    /// [Groups("group 1", "group 2", ...)].
    /// </summary>
    public class EasyEditorEnemy3 : EasyEditorEnemy2
    {
        [InspectorAttribute(group = "Game Designer Settings", order = 101)]
        public float waterSpeed;
    }
}