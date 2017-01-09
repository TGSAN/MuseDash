using UnityEditor;
using UnityEngine;
using System.Collections;
using EasyEditor;

namespace EasyEditor
{
    [Groups("Description", "Skills", "Personality")]
    [CustomEditor(typeof(TabInterfaceEnemy))]
    public class EnemyEditor : EasyEditorBase
    {
        new public void OnEnable()
        {
            base.OnEnable();
            Description();
        }

        [BeginHorizontal]
        [Inspector]
        private void Description()
        {
            ShowGroup("Description");
            HideGroup("Skills");
            HideGroup("Personality");
        }
        
        
        [Inspector]
        private void Skills()
        {
            HideGroup("Description");
            ShowGroup("Skills");
            HideGroup("Personality");
        }
        
        [EndHorizontal]
        [Inspector]
        private void Personality()
        {
            HideGroup("Description");
            HideGroup("Skills");
            ShowGroup("Personality");
        }
    }
}