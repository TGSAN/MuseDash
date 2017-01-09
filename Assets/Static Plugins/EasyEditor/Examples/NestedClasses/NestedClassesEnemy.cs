using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EasyEditor
{
    /// <summary>
    /// Serializable class and struct can be rendered in the inspector with EasyEditor rendering features.
    /// To let EE knows that he has to handle the inline renderering of the serializable class or struct,
    /// you need to specify the parameter 'rendererType' of Inspector attribute as "InlineClassRenderer".
    /// There is one limitation, if they are rendered in a list, then Unity default way of rendering will be used.
    /// </summary>
    public class NestedClassesEnemy : MonoBehaviour
    {

        [Image]
        public string easyEditorImage = "Assets/EasyEditor/Examples/icon.png";
        
        [Inspector(rendererType = "InlineClassRenderer")]
        public Bag mainBag;
        
        public Bag[] otherBagsList;

        [Inspector(rendererType = "InlineClassRenderer")]
        public EasyEditorEnemy2 enemy;

        #region definition of generic classes

        [System.Serializable]
        public class Weapon
        {
            [BeginHorizontal]
            public string name = "";
            [EndHorizontal]
            public float strength = 0f;
        }

        [Groups("Basic Settings")]
        [System.Serializable]
        public class Bag
        {
            [Range(1, 10)]
            public int weight;

            [Inspector( rendererType = "InlineClassRenderer")]
            public Weapon mainWeapon;

            public List<Weapon> otherWeapons;
        }

        #endregion
    }
}
