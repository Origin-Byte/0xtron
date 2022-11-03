using DA_Assets.FCU.Config;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.CodeHelpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.FCU.Core
{
    public class Tools : ControllerHolder<FigmaConverterUnity>
    {
        [SerializeField] ResolutionData resolutionData;
        public ResolutionData ResolutionData { get => resolutionData; set => SetValue(ref resolutionData, value); }
        public void CacheResolutionData()
        {
            bool received = controller.Model.GetGameViewSize(out Vector2 gameViewSize);

            this.ResolutionData = new ResolutionData
            {
                GameViewSizeReceived = received,
                GameViewSize = gameViewSize
            };
        }
        public void RestoreResolutionData()
        {
            if (this.ResolutionData.GameViewSizeReceived)
            {
                controller.Model.SetGameViewSize(this.ResolutionData.GameViewSize);
            }
        }
        public IEnumerator DestroyCurrentCanvasMetas_OnClick()
        {
            FCU_Meta[] fcuOIs = MonoBehaviour.FindObjectsOfType<FCU_Meta>();

            int count = 0;

            for (int i = 0; i < fcuOIs.Length; i++)
            {
                if (fcuOIs[i].FigmaConverterUnity.GetInstanceID() == controller.GetInstanceID())
                {
                    count++;
                    fcuOIs[i].Destroy();
                }

                yield return null;
            }

            Console.WriteLine(LocKey.log_current_canvas_metas_destroy.Localize(
                controller.GetInstanceID(),
                count,
                nameof(FCU_Meta)));
        }
        public void DestroyCurrentCanvasChilds_OnClick()
        {
            int count = controller.transform.ClearChilds();

            Console.WriteLine(LocKey.log_current_canvas_childs_destroy.Localize(
                controller.GetInstanceID(),
                count));
        }
#if UNITY_EDITOR
        public IEnumerator SetImgTypeSprite(FObject fobject)
        {
            while (true)
            {
                bool success = SetTextureSettings(fobject);

                if (success)
                {
                    controller.Model.ImportedSpritesCount++;

                    if (controller.Model.ImportedSpritesCount % 10 == 0)
                    {
                        Console.WriteLine(LocKey.log_imported_sprites_count.Localize(controller.Model.ImportedSpritesCount, controller.Model.SpritesToImportCount));
                    }

                    break;
                }

                yield return new WaitForSecondsRealtime(FCU_Config.Instance.Delay01);
            }
        }
        private bool SetTextureSettings(FObject fobject)
        {
            try
            {
                UnityEditor.TextureImporter importer = UnityEditor.AssetImporter.GetAtPath(fobject.AssetPath) as UnityEditor.TextureImporter;

                if (importer.isReadable == true &&
                    importer.textureType == FCU_Config.Instance.TextureImporterType &&
                    importer.crunchedCompression == FCU_Config.Instance.CrunchedCompression)
                {
                    if (importer.crunchedCompression)
                    {
                        if (importer.compressionQuality == FCU_Config.Instance.CrunchedCompressionQuality)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }

                importer.isReadable = true;
                importer.textureType = FCU_Config.Instance.TextureImporterType;
                importer.crunchedCompression = FCU_Config.Instance.CrunchedCompression;
                importer.textureCompression = FCU_Config.Instance.TextureImporterCompression;

                if (importer.crunchedCompression)
                {
                    importer.compressionQuality = FCU_Config.Instance.CrunchedCompressionQuality;
                }

                importer.FixTextureSize();

                UnityEditor.AssetDatabase.WriteImportSettingsIfDirty(fobject.AssetPath);
                UnityEditor.AssetDatabase.Refresh();

                return false;
            }
            catch
            {
                return true;
            }
        }
        public IEnumerator GetSprite(FObject fobject, Return<Sprite> @return)
        {
            yield return controller.Model.DynamicCoroutine(SetImgTypeSprite(fobject));
            Sprite sprite = (Sprite)UnityEditor.AssetDatabase.LoadAssetAtPath(fobject.AssetPath, typeof(Sprite));

            @return.Invoke(new CoroutineResult<Sprite>
            {
                Result = sprite
            });
        }
#endif
        private bool breakPointActive;

        public IEnumerator BreakPoint()
        {
            if (controller.Model.MainSettings.DebugMode == false)
            {
                yield break;
            }

            breakPointActive = true;

            while (true)
            {
                if (breakPointActive == false)
                {
                    break;
                }

                yield return new WaitForSecondsRealtime(FCU_Config.Instance.Delay01);
            }
        }
        public void NextBreakPoint()
        {
            Debug.LogWarning("NextBreakPoint();");
            breakPointActive = false;
        }

        public void DestroyLastImportedFrames_OnClick()
        {
            foreach (var item in controller.ProjectParser.LastImportedFrames)
            {
                item.Destroy();
            }

            ClearLastImportedFrames();
        }
        public void ClearLastImportedFrames()
        {
            controller.ProjectParser.LastImportedFrames = new List<GameObject>();
        }
    }
    public struct ResolutionData
    {
        public bool GameViewSizeReceived;
        public Vector2 GameViewSize;
    }
}