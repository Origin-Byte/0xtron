using DA_Assets.FCU.Config;
using DA_Assets.FCU.Core.Drawers;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.FCU.UI;
using DA_Assets.Shared.CodeHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#pragma warning disable IDE0003

namespace DA_Assets.FCU.Core
{
    [Serializable]
    public class CanvasDrawer : ControllerHolder<FigmaConverterUnity>
    {
        [SerializeField] ImageDrawer imageDrawer;
        [SerializeField] TextDrawer textDrawer;
        [SerializeField] LayoutGroupDrawer layoutGroupDrawer;
        [SerializeField] MaskDrawer maskDrawer;
        [SerializeField] ButtonDrawer buttonDrawer;
        [SerializeField] InputFieldDrawer inputFieldDrawer;
        [SerializeField] I2LocalizationDrawer i2LocalizationDrawer;
        [SerializeField] ShadowDrawer shadowDrawer;
        [SerializeField] CanvasGroupDrawer canvasGroupDrawer;
        [SerializeField] int drawnObjectsCount;

        public void Init() { }

        public IEnumerator DrawToCanvas(List<FObject> fobjects)
        {
            drawnObjectsCount = 0;

            controller.Model.ImportedSpritesCount = 0;
            controller.Model.SpritesToImportCount = fobjects.Where(x => x.AssetPath != null).Count();

            this.ButtonDrawer.Init();
            this.InputFieldDrawer.Init();
            this.TextDrawer.Init();
            this.MaskDrawer.Init();

            if (controller.Model.MainSettings.UseI2Localization)
            {
                yield return this.I2LocalizationDrawer.Draw();
            }

            foreach (FObject fobject in fobjects)
            {
                controller.Model.DynamicCoroutine(Draw(fobject));
            }

            while (true)
            {
                if (drawnObjectsCount >= fobjects.Count())
                {
                    break;
                }

                yield return new WaitForSecondsRealtime(FCU_Config.Instance.Delay1);
            }

            yield return this.ButtonDrawer.SetTargetGraphics();
            yield return this.InputFieldDrawer.SetTargetGraphics();
            yield return this.MaskDrawer.DestroyFigmaMasksImages();

            if (controller.Model.MainSettings.UseI2Localization)
            {
                yield return this.I2LocalizationDrawer.AddI2Localizes();
            }
        }
        public IEnumerator Draw(FObject fobject)
        {
            foreach (FCU_Tag tag in fobject.Tags)
            {
                switch (tag)
                {
                    case FCU_Tag.InputField:
                        yield return this.InputFieldDrawer.Draw(fobject);
                        break;
                    case FCU_Tag.Button:
                        yield return this.ButtonDrawer.Draw(fobject);
                        break;
                    case FCU_Tag.GridLayoutGroup:
                    case FCU_Tag.VertLayoutGroup:
                    case FCU_Tag.HorLayoutGroup:
                    case FCU_Tag.LayoutElement:
                        yield return this.LayoutGroupDrawer.Draw(fobject);
                        break;
                    case FCU_Tag.Placeholder:
                    case FCU_Tag.Text:
                        yield return this.TextDrawer.Draw(fobject);
                        break;
                    case FCU_Tag.Mask:
                        yield return this.MaskDrawer.Draw(fobject);
                        break;
                    case FCU_Tag.Container:
                        break;
                    case FCU_Tag.VertScrollView:
                    case FCU_Tag.HorScrollView:
                    case FCU_Tag.VertHorScrollView:
                        break;
                    case FCU_Tag.Shadow:
                        yield return this.ShadowDrawer.Draw(fobject);
                        break;
                    case FCU_Tag.CanvasGroup:
                        yield return this.CanvasGroupDrawer.Draw(fobject);
                        break;
                    default:
                        yield return this.ImageDrawer.Draw(fobject, fobject);
                        break;
                }
            }

            drawnObjectsCount++;
        }
        public IEnumerator SetFigmaTransform(List<FObject> fobjects)
        {
            var fobjectsByFrame = fobjects
                .GroupBy(x => x.Meta.RootFrame)
                .Select(g => (g.Key, g.Select(x => x)));

            foreach (var child in fobjectsByFrame)
            {
                yield return new WaitForSecondsRealtime(FCU_Config.Instance.Delay01);

                controller.Model.SetGameViewSize(child.Key.Size);
                controller.GetComponent<CanvasScaler>().referenceResolution = child.Key.Size;

                yield return new WaitForSecondsRealtime(FCU_Config.Instance.Delay01);

                foreach (FObject fobject in child.Item2)
                {
                    RectTransform rect = fobject.GameObject.GetComponent<RectTransform>();

                    fobject.GameObject.transform.localScale = Vector3.one;

                    if (fobject.Visible != null)
                    {
                        fobject.GameObject.SetActive(fobject.Visible.ToBool());
                    }

                    fobject.SetFigmaRotation(controller);
                    rect.SetAnchorSmart(AnchorType.TopLeft);
                    rect.SetSmartPivot(PivotType.TopLeft);

                    rect.position = fobject.GetPosition(controller);
                }
            }

            foreach (FObject fobject in fobjects)
            {
                if (fobject.HasRotation() && fobject.CanBeRotated())
                {
                    RectTransform rect = fobject.GameObject.GetComponent<RectTransform>();
                    rect.localPosition = new Vector3(fobject.RelativeTransform[0][2], -fobject.RelativeTransform[1][2], 0);
                }
            }

            foreach (FObject fobject in fobjects)
            {
                RectTransform rect = fobject.GameObject.GetComponent<RectTransform>();

                rect.sizeDelta = fobject.GetSize(controller);
                rect.SetSmartPivot(PivotType.MiddleCenter);

                if (fobject.ContainsTag(FCU_Tag.Frame) == false)
                {
                    rect.SetAnchorSmart(fobject.GetFigmaAnchor());
                }
            }

            if (controller.Model.MainSettings.DebugMode == false)
            {
                foreach (FObject fobject in fobjects)
                {
                    if (fobject.ContainsTag(FCU_Tag.Frame))
                    {
                        RectTransform rect = fobject.GameObject.GetComponent<RectTransform>();
                        rect.SetAnchorSmart(AnchorType.StretchAll);
                        rect.offsetMin = new Vector2(0, 0);
                        rect.offsetMax = new Vector2(0, 0);
                        rect.localScale = Vector3.one;
                    }
                }
            }
        }
        public static void TryInstantiateCanvas(Vector2 refRes)
        {
            GameObject go = UnityCodeHelpers.CreateEmptyGameObject();

            go.TryAddComponent(out FigmaConverterUnity fcu);
            go.name = string.Format(FCU_Config.Instance.CanvasGameObjectName, go.GetInstanceID().ToString().Replace("-", ""));

            TryInstantiateCanvas(fcu.gameObject, refRes);
        }
        public void TryInstantiateCanvas()
        {
            controller.Model.GetGameViewSize(out Vector2 gameViewSize);
            TryInstantiateCanvas(controller.gameObject, gameViewSize);
        }
        private static void TryInstantiateCanvas(GameObject gameObject, Vector2 refRes)
        {
            bool cExists = gameObject.TryAddComponent(out Canvas c);

            if (cExists)
            {
                Canvas[] canvases = UnityEngine.Object.FindObjectsOfType<Canvas>();
                c.renderMode = RenderMode.ScreenSpaceOverlay;

                int sortingOrder = canvases
                    .Where(x => x.TryGetComponent(out UIManager uiManager) == false)
                    .Select(x => x.sortingOrder)
                    .Max();

                if (sortingOrder < 32767)
                {
                    c.sortingOrder = sortingOrder + 1;
                }
            }

            bool csExists = gameObject.TryAddComponent(out CanvasScaler cs);

            if (csExists)
            {
                cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                cs.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                cs.matchWidthOrHeight = 1f;
                cs.referencePixelsPerUnit = 100f;
                cs.referenceResolution = refRes;
            }

            gameObject.TryAddComponent(out GraphicRaycaster gr);
        }
        public static void TryInstantiateEventSystem()
        {
            EventSystem[] findedES = MonoBehaviour.FindObjectsOfType<EventSystem>();

            if (findedES.Length == 0)
            {
                GameObject _gameObject = UnityCodeHelpers.CreateEmptyGameObject();
                _gameObject.AddComponent<EventSystem>();
                _gameObject.AddComponent<StandaloneInputModule>();
                _gameObject.name = FCU_Config.Instance.EventSystemGameObjectName;
            }
        }
        public ImageDrawer ImageDrawer
        {
            get
            {
                return imageDrawer.SetController(controller);
            }
        }
        public TextDrawer TextDrawer
        {
            get
            {
                return textDrawer.SetController(controller);
            }
        }
        public LayoutGroupDrawer LayoutGroupDrawer
        {
            get
            {
                return layoutGroupDrawer.SetController(controller);
            }
        }
        public MaskDrawer MaskDrawer
        {
            get
            {
                return maskDrawer.SetController(controller);
            }
        }
        public ButtonDrawer ButtonDrawer
        {
            get
            {
                return buttonDrawer.SetController(controller);
            }
        }
        public InputFieldDrawer InputFieldDrawer
        {
            get
            {
                return inputFieldDrawer.SetController(controller);
            }
        }
        public I2LocalizationDrawer I2LocalizationDrawer
        {
            get
            {
                return i2LocalizationDrawer.SetController(controller);
            }
        }
        public ShadowDrawer ShadowDrawer
        {
            get
            {
                return shadowDrawer.SetController(controller);
            }
        }
        public CanvasGroupDrawer CanvasGroupDrawer
        {
            get
            {
                return canvasGroupDrawer.SetController(controller);
            }
        }
    }
}