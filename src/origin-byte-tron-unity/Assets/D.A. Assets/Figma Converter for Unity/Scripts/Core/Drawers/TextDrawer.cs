using DA_Assets.FCU.Model;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using DA_Assets.FCU.Config;
using DA_Assets.Shared;
using System.Collections;
using DA_Assets.FCU.Extensions;
using DA_Assets.Shared.CodeHelpers;

#if TMPRO_EXISTS
using TMPro;
#endif

namespace DA_Assets.FCU.Core.Drawers
{
    [Serializable]
    public class TextDrawer : ControllerHolder<FigmaConverterUnity>
    {
        public List<FObject> texts;
        public void Init()
        {
            texts = new List<FObject>();
        }
        public IEnumerator Draw(FObject fobject)
        {
            switch (controller.Model.MainSettings.TextComponent)
            {
#if TMPRO_EXISTS
                case TextComponent.TextMeshPro:
                    InstantiateTextMeshPro(fobject);
                    break;
#endif
                default:
                    InstantiateDefaultText(fobject);
                    break;
            }

            texts.Add(fobject);

            yield break;
        }
        private Text InstantiateDefaultText(FObject fobject)
        {
            fobject.GameObject.TryAddGraphic(out Text text);

            SetDefaultTextStyle(text, fobject);
            SetFigmaFont(text, fobject);
            SetDefaultTextFontStyle(text, fobject);
            SetDefaultTextAligment(text, fobject);

            return text;
        }
        private void SetDefaultTextStyle(Text text, FObject fobject)
        {
            text.resizeTextForBestFit = controller.Model.UnityTextSettings.BestFit;
            text.text = fobject.Characters;
            text.resizeTextMinSize = 1;

            text.resizeTextMaxSize = Convert.ToInt32(fobject.Style.FontSize);
            text.fontSize = Convert.ToInt32(fobject.Style.FontSize);

            text.verticalOverflow = controller.Model.UnityTextSettings.VerticalWrapMode;
            text.horizontalOverflow = controller.Model.UnityTextSettings.HorizontalWrapMode;
            text.lineSpacing = controller.Model.UnityTextSettings.FontLineSpacing;

            if (fobject.Fills[0].GradientStops != null)
            {
                text.color = fobject.Fills[0].GradientStops[0].Color;
            }
            else
            {
                text.color = fobject.Fills[0].Color;
            }
        }
        private void SetDefaultTextFontStyle(Text text, FObject fobject)
        {
            string fontStyleRaw = fobject.Style.FontPostScriptName;

            if (fontStyleRaw != null)
            {
                if (fontStyleRaw.Contains(FontStyle.Bold.ToString()))
                {
                    if (fobject.Style.Italic)
                    {
                        text.fontStyle = FontStyle.BoldAndItalic;
                    }
                    else
                    {
                        text.fontStyle = FontStyle.Bold;
                    }
                }
                else if (fobject.Style.Italic)
                {
                    text.fontStyle = FontStyle.Italic;
                }
                else
                {
                    text.fontStyle = FontStyle.Normal;
                }
            }
        }
        private void SetDefaultTextAligment(Text text, FObject fobject)
        {
            string textAligment = fobject.Style.TextAlignVertical + " " + fobject.Style.TextAlignHorizontal;

            switch (textAligment)
            {
                case "BOTTOM CENTER":
                    text.alignment = TextAnchor.LowerCenter;
                    break;
                case "BOTTOM LEFT":
                    text.alignment = TextAnchor.LowerLeft;
                    break;
                case "BOTTOM RIGHT":
                    text.alignment = TextAnchor.LowerRight;
                    break;
                case "CENTER CENTER":
                    text.alignment = TextAnchor.MiddleCenter;
                    break;
                case "CENTER LEFT":
                    text.alignment = TextAnchor.MiddleLeft;
                    break;
                case "CENTER RIGHT":
                    text.alignment = TextAnchor.MiddleRight;
                    break;
                case "TOP CENTER":
                    text.alignment = TextAnchor.UpperCenter;
                    break;
                case "TOP LEFT":
                    text.alignment = TextAnchor.UpperLeft;
                    break;
                case "TOP RIGHT":
                    text.alignment = TextAnchor.UpperRight;
                    break;
                default:
                    text.alignment = TextAnchor.MiddleCenter;
                    break;
            }
        }
        private void SetFigmaFont(Text text, FObject fobject)
        {
            string defaultFontName = "Arial.ttf";

            List<Font> fonts = controller.Model.UnityTextSettings.Fonts;

            if (fonts == null || fonts.Count() < 1)
            {
                text.font = Resources.GetBuiltinResource<Font>(defaultFontName);
                return;
            }

            List<SimFont> simFonts = new List<SimFont>();

            foreach (Font font in fonts)
            {
                if (font == null)
                    continue;

                string fcuFontName = font.name.ReplaceSeparatorChars().ToLower();

                List<string> figmaFontNames = new List<string>();

                if (string.IsNullOrWhiteSpace(fobject.Style.FontPostScriptName) == false)
                {
                    figmaFontNames.Add(fobject.Style.FontPostScriptName.ReplaceSeparatorChars().ToLower());
                }

                if (string.IsNullOrWhiteSpace(fobject.Style.FontFamily) == false)
                {
                    foreach (string weight in fobject.Style.FontWeight.ToStringWeightNames())
                    {
                        string fname = fobject.Style.FontFamily + weight;
                        figmaFontNames.Add(fname.ReplaceSeparatorChars().ToLower());
                    }
                }

                foreach (var item in figmaFontNames)
                {
                    float sim = item.CalculateSimilarity(fcuFontName);

                    simFonts.Add(new SimFont
                    {
                        Similarity = sim,
                        Font = font
                    });
                }
            }

            SimFont maxSimFont = simFonts.MaxBy(x => x.Similarity);

            if (maxSimFont.Similarity >= FCU_Config.Instance.ProbabilityMatchingNames)
            {
                text.font = maxSimFont.Font;
            }
            else
            {
                text.font = Resources.GetBuiltinResource<Font>(defaultFontName);
            }
        }
#if TMPRO_EXISTS
        private TextMeshProUGUI InstantiateTextMeshPro(FObject fobject)
        {
            fobject.GameObject.TryAddGraphic(out TextMeshProUGUI text);

            SetTextMeshProStyle(text, fobject);
            SetTextMeshProAligment(text, fobject);
            SetFigmaFont(text, fobject);

            return text;
        }
        public void SetTextMeshProStyle(TextMeshProUGUI text, FObject fobject)
        {
            text.text = fobject.Characters;
            text.fontSize = fobject.Style.FontSize;

            if (fobject.Fills[0].GradientStops != null)
            {
                text.color = fobject.Fills[0].GradientStops[0].Color;
            }
            else
            {
                text.color = fobject.Fills[0].Color;
            }

            text.overrideColorTags = controller.Model.TextMeshSettings.OverrideTags;
            text.enableAutoSizing = controller.Model.TextMeshSettings.AutoSize;
            text.enableWordWrapping = controller.Model.TextMeshSettings.Wrapping;
            text.richText = controller.Model.TextMeshSettings.RichText;
            text.raycastTarget = controller.Model.TextMeshSettings.RaycastTarget;
            text.parseCtrlCharacters = controller.Model.TextMeshSettings.ParseEscapeCharacters;
            text.useMaxVisibleDescender = controller.Model.TextMeshSettings.VisibleDescender;
            text.enableKerning = controller.Model.TextMeshSettings.Kerning;
            text.extraPadding = controller.Model.TextMeshSettings.ExtraPadding;
            text.overflowMode = controller.Model.TextMeshSettings.Overflow;
            text.horizontalMapping = controller.Model.TextMeshSettings.HorizontalMapping;
            text.verticalMapping = controller.Model.TextMeshSettings.VerticalMapping;
            text.geometrySortingOrder = controller.Model.TextMeshSettings.GeometrySorting;
        }
        public void SetTextMeshProAligment(TextMeshProUGUI text, FObject fobject)
        {
            string textAligment = fobject.Style.TextAlignVertical + " " + fobject.Style.TextAlignHorizontal;

            switch (textAligment)
            {
                case "BOTTOM CENTER":
                    text.alignment = TextAlignmentOptions.Bottom;
                    break;
                case "BOTTOM LEFT":
                    text.alignment = TextAlignmentOptions.BottomLeft;
                    break;
                case "BOTTOM RIGHT":
                    text.alignment = TextAlignmentOptions.BottomRight;
                    break;
                case "CENTER CENTER":
                    text.alignment = TextAlignmentOptions.Center;
                    break;
                case "CENTER LEFT":
                    text.alignment = TextAlignmentOptions.Left;
                    break;
                case "CENTER RIGHT":
                    text.alignment = TextAlignmentOptions.Right;
                    break;
                case "TOP CENTER":
                    text.alignment = TextAlignmentOptions.Top;
                    break;
                case "TOP LEFT":
                    text.alignment = TextAlignmentOptions.TopLeft;
                    break;
                case "TOP RIGHT":
                    text.alignment = TextAlignmentOptions.TopRight;
                    break;
                default:
                    text.alignment = TextAlignmentOptions.Center;
                    break;
            }
        }
        private void SetFigmaFont(TextMeshProUGUI text, FObject fobject)
        {
            List<TMP_FontAsset> fonts = controller.Model.TextMeshSettings.Fonts;

            if (fonts == null || fonts.Count() < 1)
            {
                return;
            }

            List<SimFont> simFonts = new List<SimFont>();

            foreach (TMP_FontAsset font in fonts)
            {
                if (font == null)
                    continue;

                string fcuFontName = font.name.Replace(" SDF", "").ReplaceSeparatorChars().ToLower();

                List<string> figmaFontNames = new List<string>();

                if (string.IsNullOrWhiteSpace(fobject.Style.FontPostScriptName) == false)
                {
                    figmaFontNames.Add(fobject.Style.FontPostScriptName.ReplaceSeparatorChars().ToLower());
                }

                if (string.IsNullOrWhiteSpace(fobject.Style.FontFamily) == false)
                {
                    foreach (string weight in fobject.Style.FontWeight.ToStringWeightNames())
                    {
                        string fname = fobject.Style.FontFamily + weight;
                        figmaFontNames.Add(fname.ReplaceSeparatorChars().ToLower());
                    }
                }

                foreach (var item in figmaFontNames)
                {
                    float sim = item.CalculateSimilarity(fcuFontName);

                    simFonts.Add(new SimFont
                    {
                        Similarity = sim,
                        TMP_FontAsset = font
                    });
                }
            }

            SimFont maxSimFont = simFonts.MaxBy(x => x.Similarity);

            if (maxSimFont.Similarity >= FCU_Config.Instance.ProbabilityMatchingNames)
            {
                text.font = maxSimFont.TMP_FontAsset;
                text.fontSharedMaterial = maxSimFont.TMP_FontAsset.material;
            }
        }
#endif
    }

    public struct SimFont
    {
        public Font Font;
#if TMPRO_EXISTS
        public TMP_FontAsset TMP_FontAsset;
#endif
        public float Similarity;
    }
}
