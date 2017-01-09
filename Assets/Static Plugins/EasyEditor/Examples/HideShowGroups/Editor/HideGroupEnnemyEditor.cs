using UnityEditor;
using UnityEngine;
using System.Collections;

namespace EasyEditor
{
    [Groups("Game Designer Settings", "Basic Settings", "Advanced Settings")]
    [CustomEditor(typeof(HideGroupEnnemy))]
    public class HideGroupEnnemyEditor : EasyEditorBase
    {
    	[Inspector(group = "Basic Settings")]
    	private void ToggleDisplayAdvancedSettings()
    	{
    		HideGroupEnnemy monobehaviour = (HideGroupEnnemy)target;

    		monobehaviour.showAdvancedSetting = !monobehaviour.showAdvancedSetting;

    		if(monobehaviour.showAdvancedSetting)
    		{
    			ShowGroup("Advanced Settings");
    		}
    		else
    		{
    			HideGroup("Advanced Settings");
    		}
    	}
    }
}