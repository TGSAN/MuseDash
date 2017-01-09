using UnityEngine;
using System.Collections;

namespace EasyEditor
{
    /// <summary>
    /// Example class introducing how inheritance can be used in EasyEditor. Two methods can be used.
    /// In the current case, SublimeArmor1 inherits from the abstract class ArmorBaseClass1. We generate
    /// an editor script from ArmorBaseClass1 by right-clicking on the script and selecting "Customize Interface".
    /// Then in the editor script ArmorBaseClass1Editor.cs, we set the parameter editorForChildClasses to true in the
    /// attribute CustomEditor. The other method is described in SublimeArmor2.cs summary.
    /// </summary>
    public class SublimeArmor1 : ArmorBaseClass1 {

        [Inspector(group = "Attributes")]
        public Color sublimeColor;

        [Inspector(group = "Methods")]
    	public override void HighlightArmor()
        {
            Debug.Log("SublimeArmor Highlight Armor");
        }
    }
}