using DA_Assets.FCU.Model;
using DA_Assets.Shared.CodeHelpers;
using UnityEngine;

namespace DA_Assets.FCU.Extensions
{
    public static class TextExtensions
    {
        /// <summary>
        /// https://www.figma.com/widget-docs/api/type-FontWeight/
        /// </summary>
        public static string[] ToStringWeightNames(this int fontWeight)
        {
            switch (fontWeight)
            {
                case 100:
                    return new string[] { "thin" };
                case 200:
                    return new string[] { "extra-light", "ultra-light" };
                case 300:
                    return new string[] { "light" };
                case 400:
                    return new string[] { "normal", "regular", "demi-light" };
                case 500:
                    return new string[] { "medium" };
                case 600:
                    return new string[] { "semi-bold", "demi-bold" };
                case 700:
                    return new string[] { "bold" };
                case 800:
                    return new string[] { "extra-bold", "ultra-bold" };
                case 900:
                    return new string[] { "black", "heavy" };
                default:
                    return new string[] { "normal" };
            }
        }
        public static Color GetTextColor(this FObject text)
        {
            if (text.Fills.IsEmpty() == false)
            {
                if (text.Fills[0].Type.Contains("GRADIENT"))
                {
                    return text.Fills[0].GradientStops[0].Color;
                }
                else
                {
                    return text.Fills[0].Color;
                }
            }

            return default;
        }
    }
}