using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EasyEditor
{
    /// <summary>
    /// Example class showing how some custom editor script can be rendered inside the inspector between the fields that are rendered
    /// by default. You need to open CustomEditorEnnemyEditor.cs to see how it is done.
    /// </summary>
    public class CustomEditorEnnemy : MonoBehaviour
    {
        [Image]
        public string easyEditorImage = "Assets/EasyEditor/Examples/icon.png";

    	[Inspector(group = "Game Designer Settings")]
        public Color skinColor;
        public float maxSpeed;
        public float height = 3f;

    	[Inspector(group = "Basic Settings")]
        public bool usePhysic = true;
        public Vector3 initialPosition;

    	[Inspector(group = "Advanced Settings")]
        public List<Bounds> listOfTarget;
        public List<Collider> BodyColliders;
        
    	[Inspector(group = "Game Designer Settings", order = 1)]
        public void GetIntoFuryState()
        {
            Debug.Log("Here starts the fury state !!!");
            GetComponent<Animation>().Play();
        }
    }
}