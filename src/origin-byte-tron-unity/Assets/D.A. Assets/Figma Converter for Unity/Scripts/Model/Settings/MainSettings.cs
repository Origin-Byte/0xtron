using DA_Assets.FCU.Core;
using DA_Assets.Shared;
using DA_Assets.Shared.CodeHelpers;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [System.Serializable]
    public class MainSettings : ControllerHolder<FigmaConverterUnity>
    {
        [SerializeField] FigmaUser currentFigmaUser;
        [SerializeField] string token;
        [SerializeField] string projectUrl;
        [SerializeField] bool redownloadSprites;
        [SerializeField] ImageFormat imageFormat = ImageFormat.PNG;
        [SerializeField] ImageScale imageScale = ImageScale.X_4_0;
        [SerializeField] ImageComponent imageComponent = ImageComponent.UnityImage;
        [SerializeField] ShadowComponent shadowType;
        [SerializeField] TextComponent textComponent = TextComponent.UnityText;
        [SerializeField] ComponentDetectionType componentDetectionType = ComponentDetectionType.ById;
        [SerializeField] bool useI2Localization = false;
        [SerializeField] bool debugMode = false;
        public FigmaUser CurrentFigmaUser { get => currentFigmaUser; set => SetValue(ref currentFigmaUser, value); }
        public string Token { get => token; set => SetValue(ref token, value); }
        public bool RedownloadSprites { get => redownloadSprites; set => SetValue(ref redownloadSprites, value); }
        public ImageFormat ImageFormat { get => imageFormat; set => SetValue(ref imageFormat, value); }
        public ImageScale ImageScale { get => imageScale; set => SetValue(ref imageScale, value); }
        public bool UseI2Localization1 { get => useI2Localization; set => SetValue(ref useI2Localization, value); }
        public bool DebugMode { get => debugMode; set => SetValue(ref debugMode, value); }
        public ComponentDetectionType ComponentDetectionType { get => componentDetectionType; set => SetValue(ref componentDetectionType, value); }
        public ImageComponent ImageComponent
        {
            get
            {
                return imageComponent;
            }
            set
            {
                switch (value)
                {
                    case ImageComponent.MPImage:
#if MPUIKIT_EXISTS == false
                        Console.LogError(LocKey.log_asset_not_imported.Localize(nameof(ImageComponent.MPImage)));
                        SetValue(ref imageComponent, ImageComponent.UnityImage);
                        return;
#endif
                    case ImageComponent.ProceduralImage:
#if PUI_EXISTS == false
                        Console.LogError(LocKey.log_asset_not_imported.Localize(nameof(ImageComponent.ProceduralImage)));
                        SetValue(ref imageComponent, ImageComponent.UnityImage);
                        return;
#endif
                    default:
                        break;
                }

                SetValue(ref imageComponent, value);
            }
        }
        public ShadowComponent ShadowComponent
        {
            get
            {
                return shadowType;
            }
            set
            {
                switch (value)
                {
                    case ShadowComponent.TrueShadow:
#if TRUESHADOW_EXISTS == false
                        Console.LogError(LocKey.log_asset_not_imported.Localize(nameof(ShadowComponent.TrueShadow)));
                        SetValue(ref shadowType, ShadowComponent.Figma);
                        return;
#endif
                    default:
                        break;
                }

                SetValue(ref shadowType, value);
            }
        }
        public TextComponent TextComponent
        {
            get
            {
                return textComponent;
            }
            set
            {
                switch (value)
                {
                    case TextComponent.TextMeshPro:
#if TMPRO_EXISTS == false
                        Console.LogError(LocKey.log_asset_not_imported.Localize(nameof(TextComponent.TextMeshPro)));
                        textComponent = TextComponent.UnityText;
                        return;
#endif
                    default:
                        break;
                }

                SetValue(ref textComponent, value);
            }
        }
        public bool UseI2Localization
        {
            get
            {
                return useI2Localization;
            }
            set
            {
#if I2LOC_EXISTS
                SetValue(ref useI2Localization, value);
#else
                if (value == true)
                {
                    Console.LogError(LocKey.log_asset_not_imported.Localize("I2Localization"));
                    SetValue(ref useI2Localization, value);
                }
#endif
            }
        }
        public string ProjectUrl
        {
            get
            {
                return projectUrl;
            }
            set
            {
                string _value = value;

                try
                {
                    if ((_value?.Contains("/")).ToBool())
                    {
                        string[] splited = value.Split('/');
                        _value = splited[4];
                    }
                }
                catch
                {
                    Debug.LogError(LocKey.log_incorrent_project_url.Localize());
                }

                SetValue(ref projectUrl, _value);
            }
        }
    }
}