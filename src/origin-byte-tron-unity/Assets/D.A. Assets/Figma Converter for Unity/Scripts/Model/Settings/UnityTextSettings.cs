using DA_Assets.FCU.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class UnityTextSettings : ControllerHolder<FigmaConverterUnity>
    {
        [SerializeField] float fontLineSpacing = 1.0f;
        [SerializeField] HorizontalWrapMode horizontalWrapMode = HorizontalWrapMode.Wrap;
        [SerializeField] VerticalWrapMode verticalWrapMode = VerticalWrapMode.Truncate;
        [SerializeField] bool bestFit = true;
        public float FontLineSpacing { get => fontLineSpacing; set => SetValue(ref fontLineSpacing, value); }
        public HorizontalWrapMode HorizontalWrapMode { get => horizontalWrapMode; set => SetValue(ref horizontalWrapMode, value); }
        public VerticalWrapMode VerticalWrapMode { get => verticalWrapMode; set => SetValue(ref verticalWrapMode, value); }
        public bool BestFit { get => bestFit; set => SetValue(ref bestFit, value); }

        public List<Font> Fonts = new List<Font>();
    }
}
