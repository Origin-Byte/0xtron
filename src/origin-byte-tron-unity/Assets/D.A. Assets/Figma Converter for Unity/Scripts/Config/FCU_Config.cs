using DA_Assets.FCU.Core;
using DA_Assets.FCU.Model;
using DA_Assets.FCU.UI;
using DA_Assets.Shared;
using DA_Assets.Shared.CodeHelpers;
using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.FCU.Config
{
    public class FCU_Config : SingletoneScriptableObject<FCU_Config>
    {
        public const string ProductName = "Figma Converter for Unity";
        public const string Publisher = "D.A. Assets";

        #region FIELDS

        [SerializeField] string productVersion;

        [SerializeField] List<TagConfig> tagConfigs;
        [SerializeField] List<AssemblyConfig> assemblyConfigs;

        [Header("Prefabs")]
        [SerializeField] GameObject uiManagerPrefab;

        [Header("File's names")]
        [SerializeField] string importLogFileName;
        [SerializeField] string webLogFileName;
        [SerializeField] string localizationFileName;

        [Header("Limits")]
        [SerializeField] int maxCachedFilesCount;
        [SerializeField] int maxLogFilesCount;

        [Header("Formats")]
        [SerializeField] string dateTimeFormat1;
        [SerializeField] string dateTimeFormat2;


        [Header("GameObject's names")]
        [SerializeField] string eventSystemGameObjectName;
        [SerializeField] string canvasGameObjectName;
        [SerializeField] string i2LocGameObjectName;

        [Header("Values")]
        [SerializeField] float probabilityMatchingNames;
        [SerializeField] int gameObjectMaxNameLenght;
        [SerializeField] int chunkSizeDownloadingImages;

        [Header("Edit / Pereferences / General")]
        [Tooltip("Enable this setting to automatically compress Assets during import.")]
        [SerializeField] bool compressAssetsOnImport;

        [Header("TextureImporter Settings")]
        [SerializeField] bool crunchedCompression;
        [Tooltip("This value is used only when flag 'CrunchedCompression' is active.")]
        [SerializeField] int crunchedCompressionQuality;
#if UNITY_EDITOR
        [SerializeField] UnityEditor.TextureImporterType textureImporterType;
        [SerializeField] UnityEditor.TextureImporterCompression textureImporterCompression;
#endif

        [Header("Other")]
        [SerializeField] bool https;
        [SerializeField] bool useUiManager;
        [SerializeField] TextAsset localizations;
        [SerializeField] AssetLanguage language;
        [SerializeField] string[] assetLanguageNames;

        [SerializeField, HideInInspector] string tgLink = "https://t.me/da_assets_publisher";
        [SerializeField, HideInInspector] string compressAssetsOnImportPrefsKey = "kCompressTexturesOnImport";
        [SerializeField, HideInInspector] char realTagSeparator = '-';
        [SerializeField, HideInInspector] string apiLink = "https://api.figma.com/v1/files/{0}?geometry=paths";
        [SerializeField, HideInInspector] string clientId = "LaB1ONuPoY7QCdfshDbQbT";
        [SerializeField, HideInInspector] string clientSecret = "E9PblceydtAyE7Onhg5FHLmnvingDp";
        [SerializeField, HideInInspector] string redirectUri = "http://localhost:1923/";
        [SerializeField, HideInInspector] string authUrl = "https://www.figma.com/api/oauth/token?client_id={0}&client_secret={1}&redirect_uri={2}&code={3}&grant_type=authorization_code";
        [SerializeField, HideInInspector] string oAuthUrl = "https://www.figma.com/oauth?client_id={0}&redirect_uri={1}&scope=file_read&state={2}&response_type=code";
        [SerializeField, HideInInspector] float delay001 = 0.01f;
        [SerializeField, HideInInspector] float delay01 = 0.1f;
        [SerializeField, HideInInspector] float delay1 = 1f;

        #endregion
        #region PROPERTIES
        public string WebLogFileName { get => webLogFileName; }
        public string LocalizationFileName { get => localizationFileName; }
        public int MaxCachedFilesCount { get => maxCachedFilesCount; }
        public int MaxLogFilesCount { get => maxLogFilesCount; }
        public string DateTimeFormat { get => dateTimeFormat1; }
        public string EventSystemGameObjectName { get => eventSystemGameObjectName; }
        public string CanvasGameObjectName { get => canvasGameObjectName; }
        public string I2LocGameObjectName { get => i2LocGameObjectName; }
        public float ProbabilityMatchingNames { get => probabilityMatchingNames; }
        public int GameObjectMaxNameLenght { get => gameObjectMaxNameLenght; }
        public int GetImageLinksChunkSize { get => chunkSizeDownloadingImages; }
#if UNITY_EDITOR
        public bool CompressAssetsOnImport
        {
            get
            {
                return UnityEditor.EditorPrefs.GetBool(compressAssetsOnImportPrefsKey, false);
            }
            set
            {
                if (compressAssetsOnImport != value)
                {
                    compressAssetsOnImport = value;
                    UnityEditor.EditorPrefs.SetBool(compressAssetsOnImportPrefsKey, value);
                }
            }
        }
        public UnityEditor.TextureImporterType TextureImporterType { get => textureImporterType; set => textureImporterType = value; }
        public UnityEditor.TextureImporterCompression TextureImporterCompression { get => textureImporterCompression; set => textureImporterCompression = value; }
#endif
        public bool CrunchedCompression { get => crunchedCompression; set => crunchedCompression = value; }
        public int CrunchedCompressionQuality { get => crunchedCompressionQuality; set => crunchedCompressionQuality = value; }
        public List<AssemblyConfig> AssemblyConfigs { get => assemblyConfigs; set => assemblyConfigs = value; }
        public List<TagConfig> TagConfigs { get => tagConfigs; set => tagConfigs = value; }
        public string ImportLogFileName { get => importLogFileName; set => importLogFileName = value; }
        public string DateTimeFormat2 { get => dateTimeFormat2; set => dateTimeFormat2 = value; }
        public bool Https { get => https; set => https = value; }
        public bool Use_UIManager
        {
            get
            {
                return useUiManager;
            }
            set
            {
                if (useUiManager != value)
                {
                    useUiManager = value;

                    if (useUiManager)
                    {
                        if (UIManager.IsExistsOnScene() == false)
                        {
                            Instantiate(uiManagerPrefab);
                        }
                    }
                    else
                    {
                        if (UIManager.IsExistsOnScene())
                        {
                            UIManager.Instance.gameObject.Destroy();
                        }
                    }
                }
            }
        }
        /// <summary> 0.01f </summary>
        public float Delay001 { get => delay001; }
        /// <summary> 0.1f </summary>
        public float Delay01 { get => delay01; }
        /// <summary> 1f </summary>
        public float Delay1 { get => delay1; }
        public string TgLink { get => tgLink; }
        public string ProductVersion { get => productVersion; }
        public string ApiLink { get => apiLink; }
        public string ClientId { get => clientId; }
        public string ClientSecret { get => clientSecret; }
        public string RedirectUri { get => redirectUri; }
        public string AuthUrl { get => authUrl; }
        public string OAuthUrl { get => oAuthUrl; }
        public char RealTagSeparator { get => realTagSeparator; }
        /// <summary>
        /// EditorPrefs key to control the flag responsible for compressing assets when it's are imported into the project.
        /// <para>Edit / Pereferences / General / Compress Assets on Import</para>
        /// </summary>
        public string CompressAssetsOnImportPrefsKey { get => compressAssetsOnImportPrefsKey; }
        public TextAsset Localizations { get => localizations; set => localizations = value; }
        public AssetLanguage Language { get => language; set => language = value; }
        public string[] AssetLanguageNames { get => assetLanguageNames; set => assetLanguageNames = value; }

        #endregion
    }
}