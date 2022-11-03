using DA_Assets.FCU.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.FCU
{
    public class AssetPatcher : AssetPostprocessor
    {
        private static string[] mpImageScripts = new string[]
        {
            "MPImage.cs"
        };
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            int patchedFilesCount = 0;

            foreach (string filePath in importedAssets)
            {
                string fileName = Path.GetFileName(filePath);
                string fullPath = Path.Combine(Application.dataPath, filePath.Substring(7));

                if (mpImageScripts.Contains(fileName))
                {
                    Path_MPImage(fullPath);
                    patchedFilesCount++;
                }
            }

            if (patchedFilesCount > 0)
            {
                Debug.Log($"{patchedFilesCount} files pathed.");
            }
        }
        public static void Patch(AssemblyConfig ac)
        {
            List<string> filePathes = new List<string>();

            switch (ac.AssetType)
            {
                case AssetType.MPUIKit:
                    FilePathSearch(Application.dataPath, mpImageScripts, filePathes);

                    foreach (string fullPath in filePathes)
                    {
                        Path_MPImage(fullPath);
                    }
                    break;
            }

            AssetDatabase.Refresh();

            if (filePathes.Count() > 0)
            {
                Debug.Log($"{filePathes.Count()} files pathed.");
            }
        }
        private static void Path_MPImage(string fullPath)
        {
            ReplaceText(fullPath, ("protected void Init()", "public void Init()"));
        }
        private static void ReplaceText(string fullPath, params (string, string)[] replacePairs)
        {
            string contents = File.ReadAllText(fullPath);

            foreach (var item in replacePairs)
            {
                contents = contents.Replace(item.Item1, item.Item2);
            }

            File.SetAttributes(fullPath, FileAttributes.Normal);
            File.WriteAllText(fullPath, contents);
        }
        private static void FilePathSearch(string searchDir, string[] searchFiles, List<string> filePathes)
        {
            if (searchFiles.Length == filePathes.Count())
            {
                return;
            }

            foreach (string dirPath in Directory.GetDirectories(searchDir))
            {
                foreach (string fullPath in Directory.GetFiles(dirPath))
                {
                    string fileName = Path.GetFileName(fullPath);

                    if (searchFiles.Contains(fileName))
                    {
                        filePathes.Add(fullPath);
                    }
                }

                FilePathSearch(dirPath, searchFiles, filePathes);
            }
        }
    }
}
