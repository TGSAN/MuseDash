using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EasyEditor;

/// <summary>
/// Example class showing how list inserting and removing element events can be handled. Please open EnemyListenListEventEditor.cs to have
/// a better understanding of how it is done.
/// </summary>
public class EnemyListenListEvent : MonoBehaviour {

    [Comment("Bounds will be assigned a default value when added into the list." +
        " A debug message will be displayed when removing the bounds from the list." +
        " Check the corresponing editor script for more information")]
    public List<Bounds> listOfTarget;
}
