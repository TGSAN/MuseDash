using System;
using System.IO;
using System.Text;
using Assets.Scripts.Common;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Assets.Scripts.Tool.PRHelper
{
    public class PRNodeMaker
    {
        public static void MakerPRNode(string name)
        {
            string mainCodeStr = GenerateMainCode(name);
            CreateFile(mainCodeStr, StringCommon.PRNodePathInAsset + "/" + name + ".cs", false);

            string drawerCodeStr = GenerateDrawerCode(name);
            CreateFile(drawerCodeStr, StringCommon.PRNodeDrawerPathInAsset + "/" + name + "Drawer.cs", false);
            AssetDatabase.Refresh();
        }

        private static string GenerateMainCode(string nodeName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("namespace Assets.Scripts.Tool.PRHelper.Properties");
            sb.Append(Environment.NewLine);
            sb.Append("{");
            sb.Append(Environment.NewLine);
            sb.Append("    [Serializable]");
            sb.Append(Environment.NewLine);
            sb.Append("    public class " + nodeName);
            sb.Append(Environment.NewLine);
            sb.Append("    {");
            sb.Append(Environment.NewLine);
            sb.Append("        public void Play()");
            sb.Append(Environment.NewLine);
            sb.Append("        {");
            sb.Append(Environment.NewLine);
            sb.Append("        }");
            sb.Append(Environment.NewLine);
            sb.Append("    }");
            sb.Append(Environment.NewLine);
            sb.Append("}");

            string code = sb.ToString();

            return code;
        }

        private static string GenerateDrawerCode(string nodeName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("using UnityEditor;");
            sb.Append(Environment.NewLine);
            sb.Append("using UnityEngine;");
            sb.Append(Environment.NewLine);
            sb.Append("namespace Assets.Scripts.Tool.PRHelper.Properties.Editor");
            sb.Append(Environment.NewLine);
            sb.Append("{");
            sb.Append(Environment.NewLine);
            sb.Append("    [CustomPropertyDrawer(typeof(" + nodeName + "))]");
            sb.Append(Environment.NewLine);
            sb.Append("    public class " + nodeName + "Drawer : PropertyDrawer");
            sb.Append(Environment.NewLine);
            sb.Append("    {");
            sb.Append(Environment.NewLine);
            sb.Append("        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)");
            sb.Append(Environment.NewLine);
            sb.Append("        {");
            sb.Append(Environment.NewLine);
            sb.Append("        }");
            sb.Append(Environment.NewLine);
            sb.Append("        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)");
            sb.Append(Environment.NewLine);
            sb.Append("        {");
            sb.Append(Environment.NewLine);
            sb.Append("            return 0;");
            sb.Append(Environment.NewLine);
            sb.Append("        }");
            sb.Append(Environment.NewLine);
            sb.Append("    }");
            sb.Append(Environment.NewLine);
            sb.Append("}");
            string code = sb.ToString();

            return code;
        }

        private static void CreateFile(string str, string pathName, bool isCover = true)
        {
            string path = Application.dataPath + pathName;
            if (!File.Exists(path))
            {
                StreamWriter strWriter = File.CreateText(path);
                strWriter.Close();

                if (!isCover)
                {
                    File.WriteAllText(path, str);
                }
            }

            if (!isCover)
            {
                return;
            }
            File.WriteAllText(path, str);
        }
    }
}