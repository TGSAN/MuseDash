using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EasyEditor
{
    public class HideGroupEnnemy : MonoBehaviour
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

    	[HideInInspector]
    	public bool showAdvancedSetting = false;

    	[Inspector(group = "Advanced Settings")]
        public List<Bounds> listOfTarget;
        public List<Collider> BodyColliders;
        
    	[Inspector(group = "Game Designer Settings", order = 1)]
        public void GetIntoFuryState()
        {
            Debug.Log("Here start the fury state !!!");
            GetComponent<Animation>().Play();
        }
    }
}