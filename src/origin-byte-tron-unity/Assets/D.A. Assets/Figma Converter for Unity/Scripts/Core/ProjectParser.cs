using DA_Assets.FCU.Config;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.CodeHelpers;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace DA_Assets.FCU.Core
{
    [Serializable]
    public class ProjectParser : ControllerHolder<FigmaConverterUnity>
    {
        [SerializeField] List<GameObject> lastImportedFrames = new List<GameObject>();
        [SerializeField] FCU_Meta[] fcuMetas = new FCU_Meta[] { };
        public List<GameObject> LastImportedFrames { get => lastImportedFrames; set => SetValue(ref lastImportedFrames, value); }
        public FCU_Meta[] FcuMetas { get => fcuMetas; set => fcuMetas = value; }

        public IEnumerator SetTags(FObject page)
        {
            UpdateMetasArray();

            yield return controller.Model.DynamicCoroutine(SetTags1(page));
            yield return controller.Model.DynamicCoroutine(SetTags2(page));
        }
        public IEnumerator DestroyMissing(List<FObject> fobjects)
        {
            UpdateMetasArray();

            ConcurrentBag<FCU_Meta> toDestroy = new ConcurrentBag<FCU_Meta>();

            Parallel.ForEach(fcuMetas, (meta) =>
            {
                bool find = false;

                foreach (FObject fobject in fobjects)
                {
                    if (meta.Id == fobject.GetMetaId(controller))
                    {
                        find = true;
                        break;
                    }
                }

                if (find == false)
                {
                    toDestroy.Add(meta);
                }
            });

            foreach (FCU_Meta meta in toDestroy)
            {
                meta.gameObject.Destroy();
            }

            yield return null;
        }
        private IEnumerator SetTags1(FObject parent)
        {
            foreach (FObject child in parent.Children)
            {
                child.SetFixedName();

                child.Tags = new List<FCU_Tag>();

                FindedTag manualTag = GetManualTag(child);

                if (manualTag.Tag != FCU_Tag.None)
                {
                    child.AddTag(manualTag.Tag);
                    child.ManualTagExists = true;
                }
                else if (manualTag.CustomTag != null)
                {
                    child.CustomTag = manualTag.CustomTag;
                    child.CustomPrefab = manualTag.CustomPrefab;
                    child.ManualTagExists = true;

                    child.AddTag(FCU_Tag.Container);
                }
                else
                {
                    if (parent.ContainsTag(FCU_Tag.Page))
                    {
                        child.AddTag(FCU_Tag.Frame);
                    }

                    if (child.LayoutMode == "VERTICAL")
                    {
                        child.AddTag(FCU_Tag.VertLayoutGroup);
                    }
                    else if (child.LayoutMode == "HORIZONTAL")
                    {
                        child.AddTag(FCU_Tag.HorLayoutGroup);
                    }

                    if (parent.ContainsTag(FCU_Tag.HorLayoutGroup) ||
                        parent.ContainsTag(FCU_Tag.VertLayoutGroup) ||
                        parent.ContainsTag(FCU_Tag.GridLayoutGroup))
                    {
                        child.AddTag(FCU_Tag.LayoutElement);
                    }

                    if (child.IsMask || child.ClipsContent.ToBool() == true)
                    {
                        child.AddTag(FCU_Tag.Mask);
                    }

                    if (child.Type == "TEXT")
                    {
                        child.AddTag(FCU_Tag.Text);
                    }
                    else if (child.Type == "VECTOR")
                    {
                        child.AddTag(FCU_Tag.Vector);
                    }
                    else if (child.Fills.IsEmpty() == false)
                    {
                        child.AddTag(FCU_Tag.Image);
                    }

                    foreach (Effect effect in child.Effects)
                    {
                        if (effect.Type.Contains("SHADOW"))
                        {
                            child.AddTag(FCU_Tag.Shadow);
                            break;
                        }
                    }

                    if (child.Opacity != null && child.Opacity != 1)
                    {
                        child.AddTag(FCU_Tag.CanvasGroup);
                    }
                }

                if (child.Children.IsEmpty())
                {
                    continue;
                }

                yield return SetTags1(child);
            }
        }
        private IEnumerator SetTags2(FObject parent, bool findRawTag = false)
        {
            foreach (FObject child in parent.Children)
            {
                findRawTag = false;

                if (child.ManualTagExists)
                {
                    if (child.ContainsTag(FCU_Tag.Container))
                    {
                        findRawTag = true;
                    }

                    controller.Log($"GetFigmaType | ManualTagExists | Name: {child.FixedName}");
                }
                else
                {
                    bool renderBoundsIsNull = child.AbsoluteRenderBounds.Width == null || child.AbsoluteRenderBounds.Height == null;

                    /*if (false)
                    {
                        Vector2 v1 = new Vector2(child.AbsoluteBoundingBox.Width.ToFloat(), child.AbsoluteBoundingBox.Height.ToFloat());
                        Vector2 v2 = new Vector2(child.AbsoluteRenderBounds.Width.ToFloat(), child.AbsoluteRenderBounds.Height.ToFloat());

                        if (child.FixedName.Contains("toolbar"))
                        {
                            Debug.LogError($"{v1} | {v2}");
                        }

                        if (v1.Equals(v2) == false)
                        {
                            child.AddTag(FCU_Tag.Container);
                        }
                    }*/

                    bool isContainsVectorsOnly = IsContainsVectorsOnly(child);

                    if (isContainsVectorsOnly)
                    {
                        controller.Log($"GetFigmaType | isContainsVectorsOnly true | Name: {child.FixedName}");
                        child.AddTag(FCU_Tag.Image);
                    }
                    else
                    {
                        controller.Log($"GetFigmaType | isContainsVectorsOnly false | Name: {child.FixedName}");
                    }

                    bool isRootVector = false;

                    if (child.Tags.Contains(FCU_Tag.Vector))
                    {
                        if (parent.ContainsTag(FCU_Tag.Frame))
                        {
                            child.AddTag(FCU_Tag.Image);
                            isRootVector = true;
                        }
                    }

                    if (findRawTag == false && (isContainsVectorsOnly || isRootVector))
                    {
                        controller.Log($"GetFigmaType | can't be parent: {isContainsVectorsOnly} {isRootVector}");
                    }
                    else
                    {
                        if (child.Children.IsEmpty() == false)
                        {
                            child.AddTag(FCU_Tag.Container);
                        }
                    }
                }

                if (child.Children.IsEmpty())
                {
                    continue;
                }

                yield return SetTags2(child, findRawTag);
            }
        }

        public IEnumerator InstantiateGameObjects(FObject parent, List<FObject> list, FCU_Meta rootFrame)
        {
            if (parent.Hierarchy == null)
            {
                parent.Hierarchy = parent.FixedName;
            }

            foreach (FObject child in parent.Children)
            {
                child.Hierarchy = $"{parent.Hierarchy}/{child.FixedName}";
                child.Parent = parent;

                bool exists = IsExistsInScene(child, out FCU_Meta meta);

                if (exists)
                {
                    child.Meta = meta;
                    child.GameObject = meta.gameObject;
                    controller.Log($"InstantiateGameObjects | exists | {child.Name} | child.CustomTag: {child.CustomTag} | parent.CustomTag: {parent.CustomTag}");
                }
                else if (child.CustomTag != null)
                {
#if UNITY_EDITOR
                    child.GameObject = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(child.CustomPrefab);
                    child.Meta = child.GameObject.AddComponent<FCU_Meta>();
                    controller.Log($"InstantiateGameObjects | (child.CustomTag != null) | {child.Name} | child.CustomTag: {child.CustomTag} | parent.CustomTag: {parent.CustomTag} | {parent.Name}");
#endif
                }
                else if (parent.CustomTag != null)
                {
                    continue;
                }
                else
                {
                    child.GameObject = UnityCodeHelpers.CreateEmptyGameObject();
                    child.Meta = child.GameObject.AddComponent<FCU_Meta>();
                    controller.Log($"InstantiateGameObjects | (parent.CustomTag == null) | {child.Name} | child.CustomTag: {child.CustomTag} | parent.CustomTag: {parent.CustomTag}");
                }

                child.GameObject.TryAddComponent(out RectTransform img);

                RectTransform rect = child.GameObject.GetComponent<RectTransform>();

                if (child.ContainsTag(FCU_Tag.Frame))
                {
                    rootFrame = child.Meta;
                    child.GameObject.transform.transform.SetParent(controller.transform);
                    lastImportedFrames.Add(child.GameObject);
                }
                else
                {
                    child.GameObject.transform.transform.SetParent(parent.GameObject.transform);
                }

                child.Meta.RootFrame = rootFrame;
                child.GameObject.name = child.FixedName;
                child.Meta.SetData(child, controller);

                list.Add(child);

                if (child.Children.IsEmpty())
                {
                    continue;
                }

                if (child.HasParentTag() == false)
                {
                    continue;
                }

                yield return InstantiateGameObjects(child, list, rootFrame);
            }
        }

        private bool IsExistsInScene(FObject fobject, out FCU_Meta meta)
        {
            FCU_Meta result = null;

            int instanceId = controller.GetInstanceID();

            foreach (FCU_Meta fcuMeta in fcuMetas)
            {
                if (fcuMeta.FigmaConverterUnity.GetInstanceID() != instanceId)
                {
                    continue;
                }

                if (fcuMeta.Id == fobject.GetMetaId(controller))
                {
                    result = fcuMeta;
                    break;
                }
            }

            meta = result;
            return result != null;
        }

        private void UpdateMetasArray()
        {
#if UNITY_2020_1_OR_NEWER
            fcuMetas = MonoBehaviour.FindObjectsOfType<FCU_Meta>(true);
#else
            fcuMetas = Resources.FindObjectsOfTypeAll<FCU_Meta>();
#endif
        }
        private FindedTag GetManualTag(FObject fobject)
        {
            if (fobject.FixedName.Contains(FCU_Config.Instance.RealTagSeparator) == false)
            {
                return default;
            }

            IEnumerable<FCU_Tag> fcuTags = Enum.GetValues(typeof(FCU_Tag))
               .Cast<FCU_Tag>()
               .Where(x => x != FCU_Tag.None);

            string splited = fobject.FixedName.Split(FCU_Config.Instance.RealTagSeparator)[0];
            string noSpaces = splited.Replace(" ", "");

            foreach (FCU_Tag fcuTag in fcuTags)
            {
                bool tagFind = controller.ProjectParser.CheckForTag(noSpaces, fcuTag.GetTagConfig().FigmaTag);

                if (tagFind)
                {
                    return new FindedTag
                    {
                        Tag = fcuTag
                    };
                }
            }

            foreach (CustomPrefab customPrefab in controller.Model.CustomPrefabs)
            {
                bool tagFind = controller.ProjectParser.CheckForTag(noSpaces, customPrefab.Tag);

                if (tagFind)
                {
                    return new FindedTag
                    {
                        CustomTag = customPrefab.Tag,
                        CustomPrefab = customPrefab.Prefab
                    };
                }
            }

            return default;
        }
        private bool CheckForTag(string fobjectName, string figmaTag)
        {
            if (string.IsNullOrWhiteSpace(figmaTag))
            {
                return false;
            }

            string[] nameParts = fobjectName.ToLower().Replace(" ", "").Split(FCU_Config.Instance.RealTagSeparator);

            if (nameParts.Length >= 1)
            {
                string tagPart = nameParts[0];

                if (tagPart == figmaTag)
                {
                    controller.Log($"CheckForTag | GetFigmaType | fobject.Name: {fobjectName.SubstringSafe(Config.FCU_Config.Instance.GameObjectMaxNameLenght)} | tag: {figmaTag}");
                    return true;
                }

                float sim = tagPart.CalculateSimilarity(figmaTag);

                if (sim >= FCU_Config.Instance.ProbabilityMatchingNames)
                {
                    controller.Log($"CheckForTag | GetFigmaType | ProbabilityMatchingNames | fobject.Name: {fobjectName.SubstringSafe(Config.FCU_Config.Instance.GameObjectMaxNameLenght)} | tag: {figmaTag}");
                    return true;
                }
            }

            return false;
        }
        private bool IsContainsVectorsOnly(FObject fobject)
        {
            List<bool> values = new List<bool>();

            IsContainsVectorsOnlyRecursive(fobject, values);

            if (values.Contains(false))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void IsContainsVectorsOnlyRecursive(FObject fobject, List<bool> values)
        {
            if (fobject.CantBeInsideSingleImage())
            {
                values.Add(false);
                return;
            }

            if (fobject.Children.IsEmpty())
            {
                return;
            }

            foreach (FObject child in fobject.Children)
            {
                if (child.Tags.Contains(FCU_Tag.Vector))
                {
                    values.Add(true);
                }

                IsContainsVectorsOnlyRecursive(child, values);
            }
        }

        /// <summary>
        /// Prevents duplication of downloaded sprites.
        /// </summary>
        public IEnumerator SetMutualFlag(List<FObject> fobjects)
        {
            foreach (var item in fobjects)
            {
                item.Hash = item.GetHash();
            }

            var fobjectsByFrame = fobjects.GroupBy(x => x.Meta.RootFrame);

            Dictionary<FCU_Meta, List<string>> hashedNoDubsInFrames = new Dictionary<FCU_Meta, List<string>>();

            foreach (var item in fobjectsByFrame)
            {
                var noDubsInFrame = item
                    .GroupBy(x => x.Hash)
                    .Select(x => x.First().Hash);

                hashedNoDubsInFrames.Add(item.Key, noDubsInFrame.ToList());
            }

            var hashesDuplicates = hashedNoDubsInFrames.GetDuplicates().ToList();

            foreach (var item in fobjects)
            {
                if (hashesDuplicates.Contains(item.Hash))
                {
                    item.IsMutual = true;
                }

                item.FixedName += $" {item.Hash}";
            }

            yield break;
        }

        private string GetAssetPath(FObject fobject, bool full)
        {
            string name = $"{fobject.FixedName}.{controller.Model.MainSettings.ImageFormat.ToString().ToLower()}";
            string spriteDir;

            if (fobject.IsMutual)
            {
                spriteDir = "Mutual";
            }
            else
            {
                spriteDir = fobject.Meta.RootFrame.FixedName;
            }

            string spritesPath = Path.Combine(Application.dataPath, "Sprites", spriteDir);

            DirectoryInfo dinfo = Directory.CreateDirectory(spritesPath);

            string fullPath = Path.Combine(dinfo.FullName, name);
            string shortPath = Path.Combine("Assets", "Sprites", spriteDir, name);

            if (full)
            {
                return fullPath;
            }
            else
            {
                return shortPath;
            }
        }
        public IEnumerator SetAssetPaths(List<FObject> fobjects)
        {
            for (int i = 0; i < fobjects.Count(); i++)
            {
                if (fobjects[i].ContainsTag(FCU_Tag.Frame))
                {
                    continue;
                }

                string assetPath = GetAssetPath(fobjects[i], false);
                string filePath = GetAssetPath(fobjects[i], true);

                if (fobjects[i].Meta.IsDownloadable)
                {
                    if (controller.Model.MainSettings.RedownloadSprites)
                    {
                        fobjects[i].DownloadableFile = true;
                    }
                    else
                    {
                        bool imageFileExists = File.Exists(filePath);

                        if (imageFileExists)
                        {
                            fobjects[i].DownloadableFile = false;
                        }
                        else
                        {
                            fobjects[i].DownloadableFile = true;
                        }
                    }

                    fobjects[i].AssetPath = assetPath;
                    fobjects[i].FilePath = filePath;
                }
                else
                {
                    fobjects[i].DownloadableFile = false;
                }
            }

            yield return null;
        }
        public bool IsDownloadable(FObject fobject)
        {
            if (fobject.CustomTag != null)
            {
                controller.Log($"IsDownloadable | false | (fobject.CustomTag != null) | {fobject.FixedName} | {fobject.Hierarchy}");
                return false;
            }

            if (fobject.Visible != null && fobject.Visible == false)
            {
                controller.Log($"IsDownloadable | false | not visible | {fobject.FixedName} | {fobject.Hierarchy}");
                return false;
            }

            if (fobject.AbsoluteBoundingBox.Width == 0 || fobject.AbsoluteBoundingBox.Height == 0)
            {
                controller.Log($"IsDownloadable | false | {fobject.FixedName} | {fobject.Hierarchy} | absolute width or height == 0");
                return false;
            }

            if (fobject.IsImageOnly())
            {
                return IsDownloadableImage(fobject);
            }

            List<string> dTags = new List<string>();
            List<string> ndTags = new List<string>();

            foreach (FCU_Tag fcuTag in fobject.Tags)
            {
                TagConfig tc = fcuTag.GetTagConfig();

                if (tc.IsDownloadable)
                {
                    dTags.Add(tc.FCU_Tag.ToString());
                }
                else
                {
                    ndTags.Add(tc.FCU_Tag.ToString());
                }
            }

            if (ndTags.Count() > 0)
            {
                controller.Log($"IsDownloadable | false | (ndCount > 0) | {fobject.FixedName} | {fobject.Hierarchy}\nndTags: {string.Join(", ", ndTags)}\ndTags: {string.Join(", ", dTags)}");
                return false;
            }
            else
            {
                controller.Log($"IsDownloadable | true | {fobject.FixedName} | {fobject.Hierarchy}\nndTags: {string.Join(", ", ndTags)}\ndTags: {string.Join(", ", dTags)}");
                return true;
            }
        }
        private bool IsDownloadableImage(FObject fobject)
        {
            if (fobject.IsFilled())
            {
                controller.Log($"IsDownloadableImage | true | {fobject.FixedName} | {fobject.Hierarchy} | (fobject.IsFilled())");
                return true;
            }

            if (fobject.Fills.IsEmpty())
            {
                controller.Log($"IsDownloadableImage | true | {fobject.FixedName} | {fobject.Hierarchy} | fobject.Fills.IsEmpty()");
                return true;
            }

            bool solidFills = fobject.Fills.IsSolidFillsOnly();
            bool linearFills = fobject.Fills.ContainsLinearGradients();

            if (solidFills == false)
            {
                controller.Log($"IsDownloadableImage | true | {fobject.FixedName} | {fobject.Hierarchy} | (solidFills == false)");
                return true;
            }

            if (controller.Model.MainSettings.ImageComponent == ImageComponent.UnityImage)
            {
                if (fobject.Type != "RECTANGLE")
                {
                    controller.Log($"IsDownloadableImage | true | Image | {fobject.FixedName} | {fobject.Hierarchy} | (fobject.Type != 'RECTANGLE')");
                    return true;
                }

                if (fobject.CornerRadius > 0)
                {
                    controller.Log($"IsDownloadableImage | true | Image | {fobject.FixedName} | {fobject.Hierarchy} | (fobject.CornerRadius > 0)");
                    return true;
                }

                if ((fobject.RectangleCornerRadius?.Any(radius => radius > 0)).ToBool())
                {
                    return true;
                }

                controller.Log($"IsDownloadableImage | false | Image | {fobject.FixedName} | {fobject.Hierarchy}");
                return false;
            }
#if MPUIKIT_EXISTS
            else if (controller.Model.MainSettings.ImageComponent == ImageComponent.MPImage)
            {
                if (fobject.Type != "RECTANGLE" && fobject.Type != "ELLIPSE")
                {
                    controller.Log($"IsDownloadableImage | true | MPImage | {fobject.FixedName} | {fobject.Hierarchy} | (fobject.Type != 'RECTANGLE' && fobject.Type != 'ELLIPSE')");
                    return true;
                }

                if (controller.Model.MainSettings.ShadowComponent == ShadowComponent.Figma && fobject.Tags.Contains(FCU_Tag.Shadow))
                {
                    controller.Log($"IsDownloadableImage | true | MPImage | {fobject.FixedName} | {fobject.Hierarchy} | (fobject.Tags.Contains(FCU_Tag.Shadow))");
                    return true;
                }

                controller.Log($"IsDownloadableImage | false | MPImage | {fobject.FixedName} | {fobject.Hierarchy}");
                return false;
            }
#endif
#if PUI_EXISTS
            else if (controller.Model.MainSettings.ImageComponent == ImageComponent.ProceduralImage)
            {
                if (fobject.Type != "RECTANGLE" && fobject.Type != "ELLIPSE")
                {
                    controller.Log($"IsDownloadableImage | true | ProceduralImage | {fobject.FixedName} | {fobject.Hierarchy} | (fobject.Type != 'RECTANGLE' && fobject.Type != 'ELLIPSE')");
                    return true;
                }

                if (fobject.Strokes.Count() > 0)
                {
                    controller.Log($"IsDownloadableImage | true | ProceduralImage | {fobject.FixedName} | {fobject.Hierarchy} | (fobject.Type == 'ELLIPSE' && fobject.StrokeWeight > 0f)");
                    return true;
                }

                controller.Log($"IsDownloadableImage | false | ProceduralImage | {fobject.FixedName} | {fobject.Hierarchy} | else");
                return false;
            }
#endif
            controller.Log($"IsDownloadableImage | true | unknown | {fobject.FixedName} | {fobject.Hierarchy}");
            return true;
        }
    }
    public struct Uhashed
    {
        public string RootFrameId;
        public int ObjectHash;
    }
}