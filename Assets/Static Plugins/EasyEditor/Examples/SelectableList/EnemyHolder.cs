using UnityEngine;
using System.Collections;

namespace EasyEditor
{
    /// <summary>
    /// Example class introducing how elements of a list can be selected and modified directly
    /// in the same inspector. Ideal for composition !
    /// You need to click in a very specific area of the list to select an element : in the left border that contains
    /// the element drag handles.
    /// Special thanks to Duffer123 and TonanBora of Unity forum who shared their ideas and enthusiasm for this feature.
    /// </summary>
    public class EnemyHolder : MonoBehaviour {

        [Selectable]
        public EasyEditorEnemy2[] enemies;
    }
}
