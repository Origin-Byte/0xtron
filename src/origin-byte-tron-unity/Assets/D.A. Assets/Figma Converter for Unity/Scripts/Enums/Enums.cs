using UnityEngine;

namespace DA_Assets.FCU.Model
{
    public enum ImageFormat
    {
        PNG,
        JPG
    }
    public enum ImageScale
    {
        X_0_5,
        X_0_75,
        X_1_0,
        X_1_5,
        X_2_0,
        X_3_0,
        X_4_0
    }
    public enum ButtonState
    {
        Default = 0,
        Hover = 2,
        Pressed = 3,
        Selected = 4,
        Disabled = 5,
    }
    public enum RectAxis
    {
        Hor,
        Vert
    }
    public enum TextComponent
    {
        UnityText,
        TextMeshPro
    }
    public enum ShadowComponent
    {
        Figma,
        TrueShadow
    }
    public enum ComponentDetectionType
    {
        ById,
        ByName
    }
    public enum AnchorType
    {
        TopLeft,
        TopCenter,
        TopRight,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,

        BottomLeft,
        BottomCenter,
        BottomRight,
        BottomStretch,

        VertStretchLeft,
        VertStretchRight,
        VertStretchCenter,

        HorStretchTop,
        HorStretchMiddle,
        HorStretchBottom,

        StretchAll,
    }
    public enum PivotType
    {
        TopLeft,
        TopCenter,
        TopRight,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,

        BottomLeft,
        BottomCenter,
        BottomRight,
    }
    public struct AnchorMinMax
    {
        public Vector2 Min;
        public Vector2 Max;
    }
    public enum ModifierType
    {
        Free,
        OnlyOneEdge,
        Round,
        Uniform
    }
    public enum UnityImageType
    {
        Simple,
        Filled
    }
    public enum ImageComponent
    {
        UnityImage,
        MPImage,
        ProceduralImage
    }
    public enum ButtonComponent
    {
        Default,
        FCU_Button
    }
}