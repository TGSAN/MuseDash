using UnityEngine;
using System.Collections;

namespace EasyEditor
{
    /// <summary>
    /// Example class introducing how inheritance can be used in EasyEditor. Two methods can be used.
    /// In the current case, SublimeArmor2 inherits from the abstract class ArmorBaseClass2. We generate
    /// an editor script directly from SublimeArmor2 by right-clicking on the script and selecting "Customize Interface".
    /// To allow other children of ArmorBaseClass2 to use a custome interface, we will need to generate an editor script
    /// for each of them. To avoid it, please check SublimeArmor1.cs summary.
    /// </summary>
    public class SublimeArmor2 : ArmorBaseClass2 {

        [Inspector(group = "Attributes")]
        public Color sublimeColor;

        [Inspector(group = "Methods")]
    	public override void HighlightArmor()
        {
            Debug.Log("SublimeArmor Highlight Armor");
        }
    }
}