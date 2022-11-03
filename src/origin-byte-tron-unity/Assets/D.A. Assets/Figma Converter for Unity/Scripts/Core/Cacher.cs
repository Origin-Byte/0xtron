using DA_Assets.FCU.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using DA_Assets.Shared;
using Console = DA_Assets.Shared.Console;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Config;
using DA_Assets.Shared.CodeHelpers;

#if JSON_NET_EXISTS
using Newtonsoft.Json;
#endif

namespace DA_Assets.FCU.Core
{
    [Serializable]
    public class Cacher : ControllerHolder<FigmaConverterUnity>
    {
        public IEnumerator TryRestoreProjectFromCache(string projectId = null)
        {
            controller.Log($"AssetCache | temporaryCachePath: {Application.temporaryCachePath}");

            if (projectId == null && controller.Model.CurrentProject.IsEmpty() == false)
            {
                controller.Log($"AssetCache | if (fcu.Model.CurrentProject.IsEmpty() == false)");
                yield break;
            }

            List<CacheMeta> cache = GetAll();

            if (cache.Count() < 1)
            {
                controller.Log($"AssetCache | if (cache.Count() < 1)");
                yield break;
            }

            CacheMeta cachedFileMeta = default;

            if (projectId != null)
            {
                var clist = cache.Where(x => x.Url == projectId);

                if (clist.Count() > 0)
                {
                    cachedFileMeta = clist.First();
                    controller.Log($"AssetCache | if (projectId != null)");
                }
            }

            if (cachedFileMeta.Equals(default(CacheMeta)))
            {
                var clist = cache.Where(x => x.Url == controller.Model.MainSettings.ProjectUrl);

                if (clist.Count() > 0)
                {
                    cachedFileMeta = clist.First();
                    controller.Log($"AssetCache | (x => x.Url == fcu.Model.MainSettings.ProjectUrl)");
                }
            }

            if (cachedFileMeta.Equals(default(CacheMeta)))
            {
                cachedFileMeta = cache.OrderByDescending(x => x.DateTime).First();
            }

            bool get = GetCachePath(cachedFileMeta.FileName).TryReadAllText(out string json);

            if (get == false)
            {
                controller.Log($"AssetCache | if (get == false) | {GetCachePath(cachedFileMeta.FileName)}");
                yield break;
            }

            bool parsed = json.TryParseJson(out FigmaProject figmaProject);

            if (parsed)
            {
                if (figmaProject.IsEmpty())
                {
                    controller.Log($"AssetCache | if (figmaProject.IsEmpty())");
                    yield break;
                }

                controller.Model.CurrentProject = figmaProject;
                controller.FillSelectableFramesArray(fromCache: true);

                Console.WriteLine($"{controller.Model.CurrentProject.Document.Children.Count()} pages restored from cache.");
            }

            yield return null;
        }
        public void Cache<T>(T requestResult, string json)
        {
            controller.Model.DynamicCoroutine(controller.Cacher.Cache(json, requestResult, controller.Model.MainSettings.ProjectUrl));
        }
        public IEnumerator Cache<T>(string json, T @object, string projectUrl)
        {
            if (typeof(T) == typeof(FigmaProject))
            {
                FigmaProject figmaProject = (FigmaProject)Convert.ChangeType(@object, typeof(FigmaProject));

                CacheMeta projectMeta = new CacheMeta
                {
                    Url = projectUrl,
                    Name = figmaProject.Name,
                    DateTime = DateTime.Now
                };

                List<CacheMeta> cache = GetAll();

                ClearCacheIfLarge(cache);
                RemoveOldCacheItem(cache, projectMeta);

                string safeFileName = projectMeta.Name.ReplaceInvalidFileNameChars();
                string universalFileName = $"{DateTime.Now.ToString(DA_Assets.FCU.Config.FCU_Config.Instance.DateTimeFormat)}_{safeFileName}";

                string projectFileName = universalFileName + $".{CacheType.Json.ToLower()}";
                string metaFileName = universalFileName + $".{CacheType.FCache.ToLower()}";

                string projectFilePath = GetCachePath(projectFileName);
                string metaFilePath = GetCachePath(metaFileName);

                projectMeta.FileName = projectFileName;

#if JSON_NET_EXISTS
                string metaJson = JsonConvert.SerializeObject(projectMeta, JsonExtensions.JsonSerializerSettings);
                File.WriteAllText(metaFilePath, metaJson);
#endif
                File.WriteAllText(projectFilePath, json);

            }
            else
            {
                yield break;
            }
        }
        public List<CacheMeta> GetAll()
        {
            FileInfo[] fileInfos = new DirectoryInfo(Application.temporaryCachePath).GetFiles($"*.{CacheType.FCache.ToLower()}");
            List<CacheMeta> metas = new List<CacheMeta>();

            foreach (FileInfo fileInfo in fileInfos)
            {
                string json = File.ReadAllText(fileInfo.FullName);

                bool parsed = json.TryParseJson(out CacheMeta meta);

                if (parsed)
                {
                    metas.Add(meta);
                }
            }

            return metas;
        }
        private void RemoveOldCacheItem(List<CacheMeta> cache, CacheMeta newCache)
        {
            foreach (var item in cache)
            {
                if (item.Url == newCache.Url && item.DateTime.Equals(newCache.DateTime) == false)
                {
                    RemoveCacheItem(item);
                    break;
                }
            }
        }

        private void RemoveCacheItem(CacheMeta cacheMeta)
        {
            string filenameWithoutEx = Path.GetFileNameWithoutExtension(cacheMeta.FileName);

            string fullPathJson = GetCachePath($"{filenameWithoutEx}.{CacheType.Json.ToLower()}");
            string fullPathCache = GetCachePath($"{filenameWithoutEx}.{CacheType.FCache.ToLower()}");

            if (File.Exists(fullPathJson))
                File.Delete(fullPathJson);

            if (File.Exists(fullPathCache))
                File.Delete(fullPathCache);
        }
        private void ClearCacheIfLarge(List<CacheMeta> metas)
        {
            if (metas.Count() > FCU_Config.Instance.MaxCachedFilesCount)
            {
                metas = metas.OrderByDescending(x => x.DateTime).ToList();

                List<CacheMeta> notremove = metas.GetRange(0, FCU_Config.Instance.MaxCachedFilesCount);
                List<CacheMeta> toremove = metas.Exclude(notremove, i => i.Name).ToList();

                foreach (CacheMeta cacheMeta in toremove)
                {
                    RemoveCacheItem(cacheMeta);
                }
            }
        }
        private enum CacheType
        {
            Json,
            FCache,
            Log
        }
        private string GetCachePath(string fileName)
        {
            return Path.Combine(Application.temporaryCachePath, fileName);
        }
    }

    public struct CacheMeta
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public string FileName { get; set; }
    }
}