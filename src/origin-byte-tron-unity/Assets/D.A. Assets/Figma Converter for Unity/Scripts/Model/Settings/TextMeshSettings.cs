using UnityEngine;
using System;
using DA_Assets.FCU.Core;
using System.Collections.Generic;

#if TMPRO_EXISTS
using TMPro;
#endif

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class TextMeshSettings : ControllerHolder<FigmaConverterUnity>
    {
#if TMPRO_EXISTS
        [SerializeField] bool autoSize = false;
        [SerializeField] bool overrideTags = false;
        [SerializeField] bool wrapping = true;
        [SerializeField] bool richText = true;
        [SerializeField] bool raycastTarget = true;
        [SerializeField] bool parseEscapeCharacters = true;
        [SerializeField] bool visibleDescender = true;
        [SerializeField] bool kerning = true;
        [SerializeField] bool extraPadding = false;
        [SerializeField] TextOverflowModes overflow = TextOverflowModes.Overflow;
        [SerializeField] TextureMappingOptions horizontalMapping = TextureMappingOptions.Character;
        [SerializeField] TextureMappingOptions verticalMapping = TextureMappingOptions.Character;
        [SerializeField] VertexSortingOrder geometrySorting = VertexSortingOrder.Normal;

        public bool AutoSize { get => autoSize; set => SetValue(ref autoSize, value); }
        public bool OverrideTags { get => overrideTags; set => SetValue(ref overrideTags, value); }
        public bool Wrapping { get => wrapping; set => SetValue(ref wrapping, value); }
        public bool RichText { get => richText; set => SetValue(ref richText, value); }
        public bool RaycastTarget { get => raycastTarget; set => SetValue(ref raycastTarget, value); }
        public bool ParseEscapeCharacters { get => parseEscapeCharacters; set => SetValue(ref parseEscapeCharacters, value); }
        public bool VisibleDescender { get => visibleDescender; set => SetValue(ref visibleDescender, value); }
        public bool Kerning { get => kerning; set => SetValue(ref kerning, value); }
        public bool ExtraPadding { get => extraPadding; set => SetValue(ref extraPadding, value); }
        public TextOverflowModes Overflow { get => overflow; set => SetValue(ref overflow, value); }
        public TextureMappingOptions HorizontalMapping { get => horizontalMapping; set => SetValue(ref horizontalMapping, value); }
        public TextureMappingOptions VerticalMapping { get => verticalMapping; set => SetValue(ref verticalMapping, value); }
        public VertexSortingOrder GeometrySorting { get => geometrySorting; set => SetValue(ref geometrySorting, value); }

        public List<TMP_FontAsset> Fonts = new List<TMP_FontAsset>();
#endif
    }
}
