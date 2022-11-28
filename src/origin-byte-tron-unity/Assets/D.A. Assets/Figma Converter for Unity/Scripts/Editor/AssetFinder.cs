using DA_Assets.FCU.Config;
using DA_Assets.FCU.Core;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DA_Assets.FCU
{
    public class AssetFinder : ControllerHolder<FigmaConverterUnity>
    {
        public IEnumerator Init()
        {
            List<AssemblyConfig> importedAssets = new List<AssemblyConfig>();
            List<AssemblyConfig> notImportedAssets = new List<AssemblyConfig>();

            yield return controller.Model.DynamicCoroutine(GetAssets(i => importedAssets = i, ni => notImportedAssets = ni));

            string[] toAddDefines = importedAssets.Select(x => x.ScriptingDefineName).ToArray();
            string[] toRemoveDefines = notImportedAssets.Select(x => x.ScriptingDefineName).ToArray();

            yield return controller.Model.DynamicCoroutine(DefineModifier.Modify(toAddDefines, toRemoveDefines));
        }
        public void Refresh(AssemblyConfig ac)
        {
            if (ac.Enabled.Value != ac.Enabled.Temp)
            {
                ac.Enabled.Temp = ac.Enabled.Value;

                IEnumerator defineEvent;

                if (ac.Enabled.Value)
                {
                    defineEvent = DefineModifier.Add(ac.ScriptingDefineName);
                }
                else
                {
                    defineEvent = DefineModifier.Remove(ac.ScriptingDefineName);
                }

                controller.Model.DynamicCoroutine(defineEvent);
            }
        }

        private IEnumerator GetAssets(Action<List<AssemblyConfig>> callback1, Action<List<AssemblyConfig>> callback2)
        {
            string[] currentDefinesList = DefineModifier.CurrentScriptingDefineSymbols;
            List<string> currentAssemblyNames = GetInProjectAssemblyNames();

            List<AssemblyConfig> importedAssets = new List<AssemblyConfig>();
            List<AssemblyConfig> notImportedAssets = new List<AssemblyConfig>();

            yield return controller.Model.DynamicCoroutine(SlowCycles.ForEach(FCU_Config.Instance.AssemblyConfigs, FCU_Config.Instance.Delay01, assemblyConfig =>
            {
                foreach (string name in assemblyConfig.Data)
                {
                    if (currentAssemblyNames.Contains(name))
                    {
                        assemblyConfig.Enabled.Value = true;
                        importedAssets.Add(assemblyConfig);
                    }
                    else
                    {
                        assemblyConfig.Enabled.Value = false;
                        notImportedAssets.Add(assemblyConfig);
                    }
                }
            }));

            callback1.Invoke(importedAssets);
            callback2.Invoke(notImportedAssets);
        }
        private List<string> GetInProjectAssemblyNames()
        {
            ConcurrentBag<string> names = new ConcurrentBag<string>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Parallel.ForEach(assemblies, assembly =>
            {
                IEnumerable<string> namespaces = assembly
                         .GetTypes()
                         .Select(t => t.Namespace)
                         .Distinct();

                Parallel.ForEach(namespaces, @namespace =>
                {
                    if (string.IsNullOrWhiteSpace(@namespace))
                    {
                        return;
                    }

                    names.Add(@namespace);
                });

                string name = assembly.GetName().Name;

                if (string.IsNullOrWhiteSpace(name))
                {
                    return;
                }

                names.Add(assembly.GetName().Name);
            });

            return names.ToList();
        }
    }
}