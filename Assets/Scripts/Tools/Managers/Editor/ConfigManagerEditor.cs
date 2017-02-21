using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Scripts.Common;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.Managers.Editor
{
    public class ConfigManagerEditor : MonoBehaviour
    {
        [MenuItem(StringCommons.ConfigManagerMenuItem)]
        public static void PackageJsonConfigs()
        {
            EditorSettings.serializationMode = SerializationMode.ForceText;
            string path = Application.dataPath + "/Resources";

            Debug.Log("Find all .json in " + path);
            var guid = AssetDatabase.AssetPathToGUID(path);
            var withoutExtensions = new List<string>() { ".json" };
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(s =>
            {
                var extension = Path.GetExtension(s);
                return extension != null && withoutExtensions.Contains(extension.ToLower());
            }).ToArray();
            var startIndex = 0;
            var fileData = new List<ConfigManager.FileData>();
            EditorApplication.update = delegate ()
            {
                var file = files[startIndex];

                var isCancel = EditorUtility.DisplayCancelableProgressBar("Finding Json Configs", file, (float)startIndex / (float)files.Length);
                if (Regex.IsMatch(File.ReadAllText(file), guid))
                {
                    Debug.Log(file + " Found");
                    var fileRawName = StringUtils.LastAfter(file, '\\');
                    var fileName = StringUtils.BeginBefore(fileRawName, '.');
                    if (fileData.Exists(j => j.fileName == fileName))
                    {
                        Debug.Log("Same Name With:" + fileName);
                    }
                    else
                    {
                        var pathInResources = file.Replace(path + "\\", string.Empty);
                        pathInResources = pathInResources.Replace("\\", "/");
                        pathInResources = pathInResources.Replace(".json", string.Empty);
                        var txt = Resources.Load<TextAsset>(pathInResources);
                        if (txt != null)
                        {
                            // 带spine内容的直接当它骨骼动画过滤掉
                            if (!txt.text.Contains("spine"))
                            {
                                var jd = new ConfigManager.FileData();
                                jd.fileName = fileName;
                                jd.path = pathInResources;
                                fileData.Add(jd);
                            }
                        }
                    }
                }

                startIndex++;
                if (isCancel || startIndex >= files.Length)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;
                    Debug.Log("Json Configs Search Finished");
                    var configManager = ScriptableObject.CreateInstance<ConfigManager>();
                    configManager.configs = fileData;
                    if (!AssetDatabase.IsValidFolder(StringCommons.ResourcesPathInAssets + StringCommons.ConfigManagerConfigPoolPathInResources.Replace("/", string.Empty)))
                    {
                        AssetDatabase.CreateFolder(StringCommons.ResourcesPathInAssets.Substring(0, StringCommons.ResourcesPathInAssets.Length - 1),
                            StringCommons.ConfigManagerConfigPoolPathInResources.Replace("/", string.Empty));
                    }
                    AssetDatabase.CreateAsset(configManager, StringCommons.ConfigManagerPathInAssets);
                    Debug.Log("Json Configs Packaged");
                }
            };
        }

        private static string GetRelativeAssetsPath(string path)
        {
            return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
        }
    }
}