using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Scripts.Common;
using Assets.Scripts.Tools.Commons;
using LitJson;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.Managers
{
    public class ConfigManager : SingletonScriptObject<ConfigManager>
    {
        [SerializeField]
        public List<FileData> configs;

        public JsonData this[string idx]
        {
            get
            {
                var path = StringUtils.BeginBefore(configs.Find(c => c.fileName == idx).path, '.');
                var txt = ResourcesLoader.Load<TextAsset>(path);
                if (txt != null)
                {
                    var data = txt.text;
                    return JsonMapper.ToObject(data);
                }
                return null;
            }
        }

        public new static ConfigManager instance
        {
            get
            {
                m_Instance = m_Instance ?? (m_Instance = ResourcesLoader.Load<ConfigManager>(StringCommons.ConfigManagerInResources));
                return m_Instance;
            }
        }

        [MenuItem(StringCommons.ConfigManagerMenuItem)]
        public static void PackageJsonConfigs()
        {
            EditorSettings.serializationMode = SerializationMode.ForceText;
            string path = Application.dataPath;

            Debug.Log("Find all .json in " + path);
            var guid = AssetDatabase.AssetPathToGUID(path);
            var withoutExtensions = new List<string>() { ".json" };
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(s =>
            {
                var extension = Path.GetExtension(s);
                return extension != null && withoutExtensions.Contains(extension.ToLower());
            }).ToArray();
            var startIndex = 0;
            var fileData = new List<FileData>();
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
                        var pathInResources = file.Replace(path + "\\Resources\\", string.Empty);
                        pathInResources = pathInResources.Replace("\\", "/");
                        var jd = new FileData();
                        jd.fileName = fileName;
                        jd.path = pathInResources;
                        fileData.Add(jd);
                    }
                }

                startIndex++;
                if (isCancel || startIndex >= files.Length)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;
                    Debug.Log("Json Configs Search Finished");
                    m_Instance = ScriptableObject.CreateInstance<ConfigManager>();
                    m_Instance.configs = fileData;
                    if (!AssetDatabase.IsValidFolder(StringCommons.ResourcesPathInAssets + StringCommons.ConfigManagerConfigPoolPathInResources.Replace("/", string.Empty)))
                    {
                        AssetDatabase.CreateFolder(StringCommons.ResourcesPathInAssets.Substring(0, StringCommons.ResourcesPathInAssets.Length - 1),
                            StringCommons.ConfigManagerConfigPoolPathInResources.Replace("/", string.Empty));
                    }
                    AssetDatabase.CreateAsset(m_Instance, StringCommons.ConfigManagerPathInAssets);
                    Debug.Log("Json Configs Packaged");
                }
            };
        }

        private static string GetRelativeAssetsPath(string path)
        {
            return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
        }

        [Serializable]
        public class FileData
        {
            public string fileName;
            public string path;
        }
    }
}