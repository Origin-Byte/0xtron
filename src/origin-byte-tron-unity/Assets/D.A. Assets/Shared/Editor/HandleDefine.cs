using DA_Assets.Shared.CodeHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace DA_Assets.Shared
{
    public static class DefineModifier
    {
        public static IEnumerator Add(params string[] addDefines)
        {
            yield return Modify(addDefines: addDefines);
        }
        public static IEnumerator Remove(params string[] removeDefines)
        {
            yield return Modify(removeDefines: removeDefines);
        }
        public static IEnumerator Modify(string[] addDefines = null, string[] removeDefines = null)
        {
            string[] currentDefs = DefineModifier.CurrentScriptingDefineSymbols;

            List<string> allDefines = new List<string>();
            allDefines.AddRange(currentDefs);

            if (addDefines != null)
            {
                allDefines.AddRange(addDefines);
            }

            List<string> finalDefines = new List<string>();

            foreach (var define in allDefines)
            {
                if (removeDefines != null)
                {
                    if (removeDefines.Contains(define))
                        continue;
                }

                finalDefines.Add(define);
            }

            finalDefines = finalDefines.Distinct().ToList();

            if (currentDefs.ContainsAll(finalDefines))
            {
                yield break;
            }

            DefineModifier.CurrentScriptingDefineSymbols = finalDefines.ToArray();
        }
        public static string[] CurrentScriptingDefineSymbols
        {
            get
            {
                string rawDefs = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

                if (string.IsNullOrWhiteSpace(rawDefs) == false)
                {
                    return rawDefs.Split(";");
                }

                return new string[] { };
            }
            set
            {
                string joinedDefs = string.Join(";", value);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, joinedDefs);
            }
        }
    }
}