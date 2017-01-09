using UnityEngine;
using System.Collections;

namespace EasyEditor
{
    /// <summary>
    /// Class demonstrating ReadOnly attribute. This attribute is useful if you want to show to the user
    /// which data is handled by your monobehaviour but you don't want him to modify it.
    /// </summary>
    public class ReadOnlyExample : MonoBehaviour {

        [ReadOnly]
        [Message("The weapon list is initialized at start with weapon under this game object.")]
        [SerializeField] private Weapon[] weaponList;

        // Use this for initialization
        void Start () {
            weaponList = GetComponentsInChildren<Weapon>();
            foreach (Weapon weapon in weaponList)
            {
                weapon.transform.position = Vector3.zero;
            }
        }
    }
}