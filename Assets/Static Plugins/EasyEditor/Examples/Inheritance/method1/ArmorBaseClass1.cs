using UnityEngine;
using System.Collections;

namespace EasyEditor
{
    public abstract class ArmorBaseClass1 : MonoBehaviour {

        [Inspector(group = "Attributes")]
        public GameObject armor;
        public float protection;

        [Inspector(group = "Methods")]
        public virtual void HighlightArmor()
        {
            Debug.Log("BaseClass Highlight Armor");
        }

        [Inspector(group = "Methods")]
        public void DisplayBaseClassName()
        {
            Debug.Log("ArmorBaseClass");
        }
    }
}
