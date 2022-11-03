using System.Collections.Generic;
using UnityEngine;

#if JSON_NET_EXISTS
using Newtonsoft.Json;
#endif

namespace DA_Assets.FCU.Model
{
    public struct FigmaProject
    {
#if JSON_NET_EXISTS
        [JsonProperty("document")]
#endif
        public readonly FObject Document;
#if JSON_NET_EXISTS
        [JsonProperty("name")]
#endif
        public readonly string Name;
    }
    public class FObject : FObjectExtra
    {
#if JSON_NET_EXISTS
        [JsonProperty("id")]
#endif
        public readonly string Id;

#if JSON_NET_EXISTS
        [JsonProperty("name")]
#endif
        public readonly string Name;

#if JSON_NET_EXISTS
        [JsonProperty("type")]
#endif
        public readonly string Type;

#if JSON_NET_EXISTS
        [JsonProperty("children")]
#endif
        public List<FObject> Children;

#if JSON_NET_EXISTS
        [JsonProperty("backgroundColor")]
#endif
        public readonly Color BackgroundColor;

#if JSON_NET_EXISTS
        [JsonProperty("prototypeStartNodeID")]
#endif
        public readonly object PrototypeStartNodeID;

#if JSON_NET_EXISTS
        [JsonProperty("prototypeDevice")]
#endif
        public readonly PrototypeDevice PrototypeDevice;

#if JSON_NET_EXISTS
        [JsonProperty("blendMode")]
#endif
        public readonly string BlendMode;

#if JSON_NET_EXISTS
        [JsonProperty("absoluteBoundingBox")]
#endif
        public readonly BoundingBox AbsoluteBoundingBox;
#if JSON_NET_EXISTS
        [JsonProperty("absoluteRenderBounds")]
#endif
        public readonly BoundingBox AbsoluteRenderBounds;

#if JSON_NET_EXISTS
        [JsonProperty("preserveRatio")]
#endif
        public readonly bool PreserveRatio;

#if JSON_NET_EXISTS
        [JsonProperty("constraints")]
#endif
        public readonly Constraints Constraints;

#if JSON_NET_EXISTS
        [JsonProperty("relativeTransform")]
#endif
        public readonly List<List<float>> RelativeTransform;

#if JSON_NET_EXISTS
        [JsonProperty("size")]
#endif
        public readonly Vector2 Size;

#if JSON_NET_EXISTS
        [JsonProperty("fills")]
#endif
        public readonly List<Fill> Fills;

#if JSON_NET_EXISTS
        [JsonProperty("fillGeometry")]
#endif
        public readonly List<Geometry> FillGeometry;

#if JSON_NET_EXISTS
        [JsonProperty("strokes")]
#endif
        public readonly List<Stroke> Strokes;

#if JSON_NET_EXISTS
        [JsonProperty("strokeWeight")]
#endif
        public readonly float StrokeWeight;

#if JSON_NET_EXISTS
        [JsonProperty("strokeAlign")]
#endif
        public readonly string StrokeAlign;

#if JSON_NET_EXISTS
        [JsonProperty("strokeGeometry")]
#endif
        public readonly List<Geometry> StrokeGeometry;

#if JSON_NET_EXISTS
        [JsonProperty("effects")]
#endif
        public readonly List<Effect> Effects;
#if JSON_NET_EXISTS
        [JsonProperty("arcData")]
#endif
        public readonly ArcData ArcData;

#if JSON_NET_EXISTS
        [JsonProperty("clipsContent")]
#endif
        public readonly bool? ClipsContent;

#if JSON_NET_EXISTS
        [JsonProperty("background")]
#endif
        public readonly List<Background> Background;

#if JSON_NET_EXISTS
        [JsonProperty("exportSettings")]
#endif
        public readonly List<ExportSetting> ExportSettings;

#if JSON_NET_EXISTS
        [JsonProperty("componentId")]
#endif
        public readonly string ComponentId;

#if JSON_NET_EXISTS
        [JsonProperty("cornerRadius")]
#endif
        public readonly float CornerRadius;

#if JSON_NET_EXISTS
        [JsonProperty("rectangleCornerRadii")]
#endif
        public readonly List<float> RectangleCornerRadius;

#if JSON_NET_EXISTS
        [JsonProperty("styles")]
#endif
        public readonly Styles Styles;

#if JSON_NET_EXISTS
        [JsonProperty("visible")]
#endif
        public readonly bool? Visible;

#if JSON_NET_EXISTS
        [JsonProperty("opacity")]
#endif
        public readonly float? Opacity;

#if JSON_NET_EXISTS
        [JsonProperty("layoutGrids")]
#endif
        public readonly List<object> LayoutGrids;

#if JSON_NET_EXISTS
        [JsonProperty("layoutMode")]
#endif
        public readonly string LayoutMode;

#if JSON_NET_EXISTS
        [JsonProperty("itemSpacing")]
#endif
        public readonly float ItemSpacing;

#if JSON_NET_EXISTS
        [JsonProperty("paddingLeft")]
#endif
        public readonly float PaddingLeft;

#if JSON_NET_EXISTS
        [JsonProperty("paddingRight")]
#endif
        public readonly float PaddingRight;

#if JSON_NET_EXISTS
        [JsonProperty("paddingTop")]
#endif
        public readonly float PaddingTop;

#if JSON_NET_EXISTS
        [JsonProperty("paddingBottom")]
#endif
        public readonly float PaddingBottom;

#if JSON_NET_EXISTS
        [JsonProperty("characters")]
#endif
        public readonly string Characters;

#if JSON_NET_EXISTS
        [JsonProperty("style")]
#endif
        public readonly Style Style;

#if JSON_NET_EXISTS
        [JsonProperty("characterStyleOverrides")]
#endif
        public readonly List<object> CharacterStyleOverrides;

#if JSON_NET_EXISTS
        [JsonProperty("styleOverrideTable")]
#endif
        public readonly object StyleOverrideTable;

#if JSON_NET_EXISTS
        [JsonProperty("strokeCap")]
#endif
        public readonly string StrokeCap;

#if JSON_NET_EXISTS
        [JsonProperty("strokeJoin")]
#endif
        public readonly string StrokeJoin;

#if JSON_NET_EXISTS
        [JsonProperty("strokeDashes")]
#endif
        public readonly List<float> StrokeDashes;

#if JSON_NET_EXISTS
        [JsonProperty("strokeMiterAngle")]
#endif
        public readonly float? StrokeMiterAngle;

#if JSON_NET_EXISTS
        [JsonProperty("layoutAlign")]
#endif
        public readonly string LayoutAlign;

#if JSON_NET_EXISTS
        [JsonProperty("layoutGrow")]
#endif
        public readonly float LayoutGrow;

#if JSON_NET_EXISTS
        [JsonProperty("isMask")]
#endif
        public readonly bool IsMask;

#if JSON_NET_EXISTS
        [JsonProperty("counterAxisSizingMode")]
#endif
        public readonly string CounterAxisSizingMode;
#if JSON_NET_EXISTS
        [JsonProperty("primaryAxisSizingMode")]
#endif
        public readonly string PrimaryAxisSizingMode;
#if JSON_NET_EXISTS
        [JsonProperty("counterAxisAlignItems")]
#endif
        public readonly string CounterAxisAlignItems;
#if JSON_NET_EXISTS
        [JsonProperty("primaryAxisAlignItems")]
#endif
        public readonly string PrimaryAxisAlignItems;
#if JSON_NET_EXISTS
        [JsonProperty("overflowDirection")]
#endif
        public readonly string OverflowDirection;
    }
    public struct BoundingBox
    {

#if JSON_NET_EXISTS
        [JsonProperty("x")]
#endif
        public readonly float? X;

#if JSON_NET_EXISTS
        [JsonProperty("y")]
#endif
        public readonly float? Y;

#if JSON_NET_EXISTS
        [JsonProperty("width")]
#endif
        public readonly float? Width;

#if JSON_NET_EXISTS
        [JsonProperty("height")]
#endif
        public readonly float? Height;
    }
    public struct Constraints
    {

#if JSON_NET_EXISTS
        [JsonProperty("vertical")]
#endif
        public readonly string Vertical;

#if JSON_NET_EXISTS
        [JsonProperty("horizontal")]
#endif
        public readonly string Horizontal;
    }
    public struct GradientStop
    {

#if JSON_NET_EXISTS
        [JsonProperty("color")]
#endif
        public readonly Color Color;

#if JSON_NET_EXISTS
        [JsonProperty("position")]
#endif
        public readonly float Position;
    }
    public struct Fill
    {
#if JSON_NET_EXISTS
        [JsonProperty("blendMode")]
#endif
        public readonly string BlendMode;

#if JSON_NET_EXISTS
        [JsonProperty("opacity")]
#endif
        public readonly float? Opacity;

#if JSON_NET_EXISTS
        [JsonProperty("type")]
#endif
        public readonly string Type;

#if JSON_NET_EXISTS
        [JsonProperty("scaleMode")]
#endif
        public readonly string ScaleMode;

#if JSON_NET_EXISTS
        [JsonProperty("imageRef")]
#endif
        public readonly string ImageRef;

#if JSON_NET_EXISTS
        [JsonProperty("color")]
#endif
        public readonly Color Color;

#if JSON_NET_EXISTS
        [JsonProperty("visible")]
#endif
        public readonly bool? Visible;

#if JSON_NET_EXISTS
        [JsonProperty("gradientHandlePositions")]
#endif
        public readonly List<Vector2> GradientHandlePositions;

#if JSON_NET_EXISTS
        [JsonProperty("gradientStops")]
#endif
        public readonly List<GradientStop> GradientStops;
    }
    public struct Geometry
    {
#if JSON_NET_EXISTS
        [JsonProperty("path")]
#endif
        public readonly string Path;

#if JSON_NET_EXISTS
        [JsonProperty("windingRule")]
#endif
        public readonly string WindingRule;
    }
    public struct Effect
    {
#if JSON_NET_EXISTS
        [JsonProperty("type")]
#endif
        public readonly string Type;

#if JSON_NET_EXISTS
        [JsonProperty("visible")]
#endif
        public readonly bool? Visible;

#if JSON_NET_EXISTS
        [JsonProperty("color")]
#endif
        public readonly Color Color;

#if JSON_NET_EXISTS
        [JsonProperty("blendMode")]
#endif
        public readonly string BlendMode;

#if JSON_NET_EXISTS
        [JsonProperty("offset")]
#endif
        public readonly Vector2 Offset;

#if JSON_NET_EXISTS
        [JsonProperty("radius")]
#endif
        public readonly float Radius;
    }
    public struct ArcData
    {
#if JSON_NET_EXISTS
        [JsonProperty("startingAngle")]
#endif
        public readonly float StartingAngle;
#if JSON_NET_EXISTS
        [JsonProperty("endingAngle")]
#endif
        public readonly float EndingAngle;
#if JSON_NET_EXISTS
        [JsonProperty("innerRadius")]
#endif
        public readonly float InnerRadius;
    }
    public struct Stroke
    {

#if JSON_NET_EXISTS
        [JsonProperty("blendMode")]
#endif
        public readonly string BlendMode;

#if JSON_NET_EXISTS
        [JsonProperty("type")]
#endif
        public readonly string Type;

#if JSON_NET_EXISTS
        [JsonProperty("color")]
#endif
        public readonly Color Color;
    }
    public struct Styles
    {
#if JSON_NET_EXISTS
        [JsonProperty("fill")]
#endif
        public readonly string Fill;

#if JSON_NET_EXISTS
        [JsonProperty("effect")]
#endif
        public readonly string Effect;

#if JSON_NET_EXISTS
        [JsonProperty("stroke")]
#endif
        public readonly string Stroke;
    }
    public struct Background
    {

#if JSON_NET_EXISTS
        [JsonProperty("blendMode")]
#endif
        public readonly string BlendMode;

#if JSON_NET_EXISTS
        [JsonProperty("visible")]
#endif
        public readonly bool Visible;

#if JSON_NET_EXISTS
        [JsonProperty("type")]
#endif
        public readonly string Type;

#if JSON_NET_EXISTS
        [JsonProperty("color")]
#endif
        public readonly Color Color;
    }
    public struct Style
    {

#if JSON_NET_EXISTS
        [JsonProperty("fontFamily")]
#endif
        public readonly string FontFamily;

#if JSON_NET_EXISTS
        [JsonProperty("fontPostScriptName")]
#endif
        public readonly string FontPostScriptName;

#if JSON_NET_EXISTS
        [JsonProperty("italic")]
#endif
        public readonly bool Italic;

#if JSON_NET_EXISTS
        [JsonProperty("fontWeight")]
#endif
        public readonly int FontWeight;

#if JSON_NET_EXISTS
        [JsonProperty("textAutoResize")]
#endif
        public readonly string TextAutoResize;

#if JSON_NET_EXISTS
        [JsonProperty("fontSize")]
#endif
        public readonly float FontSize;

#if JSON_NET_EXISTS
        [JsonProperty("textAlignHorizontal")]
#endif
        public readonly string TextAlignHorizontal;

#if JSON_NET_EXISTS
        [JsonProperty("textAlignVertical")]
#endif
        public readonly string TextAlignVertical;

#if JSON_NET_EXISTS
        [JsonProperty("letterSpacing")]
#endif
        public readonly float LetterSpacing;

#if JSON_NET_EXISTS
        [JsonProperty("lineHeightPx")]
#endif
        public readonly float LineHeightPx;

#if JSON_NET_EXISTS
        [JsonProperty("lineHeightPercent")]
#endif
        public readonly float LineHeightPercent;

#if JSON_NET_EXISTS
        [JsonProperty("lineHeightUnit")]
#endif
        public readonly string LineHeightUnit;
    }

    public struct Constraint
    {

#if JSON_NET_EXISTS
        [JsonProperty("type")]
#endif
        public readonly string Type;

#if JSON_NET_EXISTS
        [JsonProperty("value")]
#endif
        public readonly float Value;
    }
    public struct ExportSetting
    {

#if JSON_NET_EXISTS
        [JsonProperty("suffix")]
#endif
        public readonly string Suffix;

#if JSON_NET_EXISTS
        [JsonProperty("format")]
#endif
        public readonly string Format;

#if JSON_NET_EXISTS
        [JsonProperty("constraint")]
#endif
        public readonly Constraint Constraint;
    }
    public struct FigmaSize
    {

#if JSON_NET_EXISTS
        [JsonProperty("width")]
#endif
        public readonly float Width;

#if JSON_NET_EXISTS
        [JsonProperty("height")]
#endif
        public readonly float Height;
    }
    public struct PrototypeDevice
    {

#if JSON_NET_EXISTS
        [JsonProperty("type")]
#endif
        public readonly string Type;

#if JSON_NET_EXISTS
        [JsonProperty("size")]
#endif
        public readonly FigmaSize Size;

#if JSON_NET_EXISTS
        [JsonProperty("presetIdentifier")]
#endif
        public readonly string PresetIdentifier;

#if JSON_NET_EXISTS
        [JsonProperty("rotation")]
#endif
        public readonly string Rotation;
    }
}