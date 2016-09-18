using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HutongGames.PlayMakerEditor
{
    /// <summary>
    /// Adds Playmaker defines to project
    /// Other tools can now use #if PLAYMAKER
    /// Package as source code so user can remove or modify
    /// </summary>
    [InitializeOnLoad]
    public class PlayMakerDefines
    {
        static PlayMakerDefines()
        {
            AddScriptingDefineSymbolToAllTargets("PLAYMAKER");

            AddScriptingDefineSymbolToAllTargets("PLAYMAKER_1_8");
            AddScriptingDefineSymbolToAllTargets("PLAYMAKER_1_8_2");
            AddScriptingDefineSymbolToAllTargets("PLAYMAKER_1_8_OR_NEWER");
            
            RemoveScriptingDefineSymbolFromAllTargets("PLAYMAKER_1_8_0");
            RemoveScriptingDefineSymbolFromAllTargets("PLAYMAKER_1_8_1");
        }

        public static void AddScriptingDefineSymbolToAllTargets(string defineSymbol)
        {
            foreach (BuildTargetGroup group in Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (!IsValidBuildTargetGroup(group)) continue;

                var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';').Select(d => d.Trim()).ToList();
                if (!defineSymbols.Contains(defineSymbol))
                {
                    defineSymbols.Add(defineSymbol);
                    try
                    {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defineSymbols.ToArray()));
                    }
                    catch (Exception)
                    {
                        Debug.Log("Could not set PLAYMAKER defines for build target group: " + group);
                        throw;
                    }
                    
                }
            }
        }

        public static void RemoveScriptingDefineSymbolFromAllTargets(string defineSymbol)
        {
            foreach (BuildTargetGroup group in Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (!IsValidBuildTargetGroup(group)) continue;

                var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';').Select(d => d.Trim()).ToList();
                if (defineSymbols.Contains(defineSymbol))
                {
                    defineSymbols.Remove(defineSymbol);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defineSymbols.ToArray()));
                }
            }
        }

        private static bool IsValidBuildTargetGroup(BuildTargetGroup group)
        {
            if (group == BuildTargetGroup.Unknown || IsObsolete(group)) return false;

            // Checking Obsolete attribute should be enough, 
            // but sometimes Unity versions are missing attributes
            // so keeping these checks around just in case:

#if UNITY_5_3_0 // Unity 5.3.0 had tvOS in enum but throws error if used
            if ((int)(object)group == 25) return false;
#endif

#if UNITY_5_4 || UNITY_5_5 // Unity 5.4+ doesn't like Wp8 and Blackberry any more
            if ((int)(object)group == 15) return false;
            if ((int)(object)group == 16) return false;
#endif

            return true;
        }

        private static bool IsObsolete(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (ObsoleteAttribute[]) fi.GetCustomAttributes(typeof(ObsoleteAttribute), false);
            return attributes.Length > 0;
        }
    }
}

