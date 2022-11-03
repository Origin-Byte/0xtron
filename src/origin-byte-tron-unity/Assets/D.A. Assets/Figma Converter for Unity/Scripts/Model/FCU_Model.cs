using DA_Assets.FCU.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class FCU_Model : ControllerHolder<FigmaConverterUnity>
    {
        [SerializeField] MainSettings mainSettings;
        [SerializeField] ProceduralImageSettings puiSettings;
        [SerializeField] string guid;
        [SerializeField] List<SelectableItem> selectableFrames = new List<SelectableItem>();
        [SerializeField] int spritesToImportCount;
        [SerializeField] int importedSpritesCount;

        public TextMeshSettings textMeshSettings;
        public UnityTextSettings unityTextSettings;
        public List<HamburgerItem> SelectableHamburgerItems = new List<HamburgerItem>();
        public List<CustomPrefab> CustomPrefabs = new List<CustomPrefab>();

        public List<SelectableItem> SelectableFrames { get => selectableFrames; set => SetValue(ref selectableFrames, value); }
        #region SETTINGS
        public MainSettings MainSettings { get => mainSettings.SetController(controller); }
        public ProceduralImageSettings PuiSettings { get => puiSettings.SetController(controller); }
        public TextMeshSettings TextMeshSettings { get => textMeshSettings.SetController(controller); }
        public UnityTextSettings UnityTextSettings { get => unityTextSettings.SetController(controller); }
        #endregion
        public int SpritesToImportCount { get => spritesToImportCount; set => SetValue(ref spritesToImportCount, value); }
        public int ImportedSpritesCount { get => importedSpritesCount; set => SetValue(ref importedSpritesCount, value); }
        /// <summary>
        /// Allows running coroutines in playmode/build using 'MonoBehaviour.StartCoroutine' method,
        /// and in editor using 'EditorCoroutine.StartCoroutine' method.
        /// </summary>
        public DynamicCoroutine DynamicCoroutine { get; set; }
        public SetGameViewSize SetGameViewSize { get; set; }
        public GetGameViewSize GetGameViewSize { get; set; }
        /// <summary>
        /// It's necessary that when adding a DEFINE, after reloading the editor, 
        /// if something went wrong, re-add or remove the DEFINE. 
        /// The event will be executed until the DEFINE is added or removed.
        /// </summary>
        public object AssetRepainter { get; set; }
        public object EditorGUI { get; set; }

        public FigmaProject CurrentProject { get; set; }
        public List<object> CoroutinesPool = new List<object>();
        public IEnumerator StopAllCoroutines { get; set; }
        public string Guid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(guid))
                {
                    SetValue(ref guid, System.Guid.NewGuid().ToString().Split('-')[0]);
                }

                return guid;
            }
        }
    }
}