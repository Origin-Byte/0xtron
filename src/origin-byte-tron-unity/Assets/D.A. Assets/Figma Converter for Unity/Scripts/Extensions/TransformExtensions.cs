using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.CodeHelpers;
using UnityEngine;

namespace DA_Assets.FCU.Extensions
{
    public static class TransformExtensions
    {
        public static bool CanBeRotated(this FObject fobject)
        {
            return fobject.HasParentTag() || fobject.ContainsTag(FCU_Tag.Text) || fobject.Meta.IsDownloadable == false;
        }
        public static bool HasRotation(this FObject fobject)
        {
            return fobject.GetFigmaRotationAngle() != 0;
        }
        public static void SetFigmaRotation(this FObject fobject, FigmaConverterUnity fcu)
        {
            float rotationAngle;

            RectTransform rect = fobject.GameObject.GetComponent<RectTransform>();

            if (fobject.CanBeRotated())
            {
                rotationAngle = fobject.GetFigmaRotationAngle();
            }
            else
            {
                rotationAngle = 0;
            }

            rect.SetRotation(rotationAngle);
        }
        public static float GetFigmaRotationAngle(this FObject fobject)
        {
            Vector2 fangle = new Vector2(fobject.RelativeTransform[0][0], fobject.RelativeTransform[0][1]);

            if (fangle != new Vector2(1.0f, 0.0f))
            {
                float rotationAngle = Mathf.Atan2(fangle.y, fangle.x) * (180 / Mathf.PI);
                return rotationAngle;
            }
            else
            {
                return 0;
            }
        }
        public static Vector3 GetSize(this FObject fobject, FigmaConverterUnity fcu)
        {
            float width;
            float height;

            bool renderBoundsIsNull = fobject.AbsoluteRenderBounds.Width == null || fobject.AbsoluteRenderBounds.Height == null;

            Vector2 boundingBox = new Vector2(fobject.AbsoluteBoundingBox.Width.ToFloat(), fobject.AbsoluteBoundingBox.Height.ToFloat());
            Vector2 renderBox = new Vector2(fobject.AbsoluteRenderBounds.Width.ToFloat(), fobject.AbsoluteRenderBounds.Height.ToFloat());

            if (fobject.HasRotation() && fobject.CanBeRotated())
            {
                width = fobject.Size.x;
                height = fobject.Size.y;
            }
            else if (boundingBox != renderBox && fobject.Meta.IsDownloadable)
            {
                width = renderBox.x;
                height = renderBox.y;
            }
            else if (renderBoundsIsNull || fobject.Meta.IsDownloadable == false || fobject.ContainsTag(FCU_Tag.Container) || fobject.Parent.ContainsTag(FCU_Tag.Frame))
            {
                width = boundingBox.x;
                height = boundingBox.y;
            }
            else
            {
                width = renderBox.x;
                height = renderBox.y;
            }

            Vector3 size = new Vector3(width, height, 0);
            return size;
        }
        public static Vector3 GetPosition(this FObject fobject, FigmaConverterUnity fcu)
        {
            float x;
            float y;

            bool renderBoundsIsNull = fobject.AbsoluteRenderBounds.X == null || fobject.AbsoluteRenderBounds.Y == null;

            Vector2 boundingBox = new Vector2(fobject.AbsoluteBoundingBox.X.ToFloat(), fobject.AbsoluteBoundingBox.Y.ToFloat());
            Vector2 renderBox = new Vector2(fobject.AbsoluteRenderBounds.X.ToFloat(), fobject.AbsoluteRenderBounds.Y.ToFloat());

            if (boundingBox != renderBox && fobject.Meta.IsDownloadable)
            {
                x = renderBox.x;
                y = -renderBox.y;
            }
            else if (renderBoundsIsNull || fobject.Meta.IsDownloadable == false || fobject.ContainsTag(FCU_Tag.Container) || fobject.Parent.ContainsTag(FCU_Tag.Frame))
            {
                x = boundingBox.x;
                y = -boundingBox.y;
            }
            else
            {
                x = renderBox.x;
                y = -renderBox.y;
            }

            return new Vector3(x, y, 0);
        }
        internal static TextAnchor GetChildAligment(this FObject fobject)
        {
            bool upperLeft =
                fobject.CounterAxisSizingMode == "FIXED" &&
                fobject.PrimaryAxisSizingMode == "FIXED" &&
                fobject.CounterAxisAlignItems == null &&
                fobject.PrimaryAxisAlignItems == null;

            bool upperCenter =
                fobject.CounterAxisSizingMode == "FIXED" &&
                fobject.PrimaryAxisSizingMode == "FIXED" &&
                fobject.CounterAxisAlignItems == "CENTER" &&
                fobject.PrimaryAxisAlignItems == null;

            bool upperRight =
                fobject.CounterAxisSizingMode == "FIXED" &&
                fobject.PrimaryAxisSizingMode == "FIXED" &&
                fobject.CounterAxisAlignItems == "MAX" &&
                fobject.PrimaryAxisAlignItems == null;

            bool middleLeft =
                fobject.CounterAxisSizingMode == "FIXED" &&
                fobject.PrimaryAxisSizingMode == "FIXED" &&
                fobject.CounterAxisAlignItems == null &&
                fobject.PrimaryAxisAlignItems == "CENTER";

            bool middleCenter =
                fobject.CounterAxisSizingMode == "FIXED" &&
                fobject.PrimaryAxisSizingMode == "FIXED" &&
                fobject.CounterAxisAlignItems == "CENTER" &&
                fobject.PrimaryAxisAlignItems == "CENTER";

            bool middleRight =
                fobject.CounterAxisSizingMode == "FIXED" &&
                fobject.PrimaryAxisSizingMode == "FIXED" &&
                fobject.CounterAxisAlignItems == "MAX" &&
                fobject.PrimaryAxisAlignItems == "CENTER";

            bool lowerLeft =
                fobject.CounterAxisSizingMode == "FIXED" &&
                fobject.PrimaryAxisSizingMode == "FIXED" &&
                fobject.CounterAxisAlignItems == null &&
                fobject.PrimaryAxisAlignItems == "MAX";

            bool lowerCenter =
                fobject.CounterAxisSizingMode == "FIXED" &&
                fobject.PrimaryAxisSizingMode == "FIXED" &&
                fobject.CounterAxisAlignItems == "CENTER" &&
                fobject.PrimaryAxisAlignItems == "MAX";

            bool lowerRight =
                fobject.CounterAxisSizingMode == "FIXED" &&
                fobject.PrimaryAxisSizingMode == "FIXED" &&
                fobject.CounterAxisAlignItems == "MAX" &&
                fobject.PrimaryAxisAlignItems == "MAX";

            if (upperLeft)
                return TextAnchor.UpperLeft;
            else if (upperCenter)
                return TextAnchor.UpperCenter;
            else if (upperRight)
                return TextAnchor.UpperRight;
            else if (middleLeft)
                return TextAnchor.MiddleLeft;
            else if (middleCenter)
                return TextAnchor.MiddleCenter;
            else if (middleRight)
                return TextAnchor.MiddleRight;
            else if (lowerLeft)
                return TextAnchor.LowerLeft;
            else if (lowerCenter)
                return TextAnchor.LowerCenter;
            else if (lowerRight)
                return TextAnchor.LowerRight;
            return TextAnchor.MiddleCenter;
        }

        public static AnchorType GetFigmaAnchor(this FObject fobject)
        {
            string anchor = fobject.Constraints.Vertical + " " + fobject.Constraints.Horizontal;

            AnchorType anchorPreset;

            switch (anchor)
            {
                ////////////////LEFT////////////////
                case "TOP LEFT":
                    anchorPreset = AnchorType.TopLeft;
                    break;
                case "BOTTOM LEFT":
                    anchorPreset = AnchorType.BottomLeft;
                    break;
                case "TOP_BOTTOM LEFT":
                    anchorPreset = AnchorType.VertStretchLeft;
                    break;
                case "CENTER LEFT":
                    anchorPreset = AnchorType.MiddleLeft;
                    break;
                case "SCALE LEFT":
                    anchorPreset = AnchorType.VertStretchLeft;
                    break;
                ////////////////RIGHT////////////////
                case "TOP RIGHT":
                    anchorPreset = AnchorType.TopRight;
                    break;
                case "BOTTOM RIGHT":
                    anchorPreset = AnchorType.BottomRight;
                    break;
                case "TOP_BOTTOM RIGHT":
                    anchorPreset = AnchorType.VertStretchRight;
                    break;
                case "CENTER RIGHT":
                    anchorPreset = AnchorType.MiddleRight;
                    break;
                case "SCALE RIGHT":
                    anchorPreset = AnchorType.VertStretchRight;
                    break;
                ////////////////LEFT_RIGHT////////////////
                case "TOP LEFT_RIGHT":
                    anchorPreset = AnchorType.HorStretchTop;
                    break;
                case "BOTTOM LEFT_RIGHT":
                    anchorPreset = AnchorType.HorStretchBottom;
                    break;
                case "TOP_BOTTOM LEFT_RIGHT":
                    anchorPreset = AnchorType.StretchAll;
                    break;
                case "CENTER LEFT_RIGHT":
                    anchorPreset = AnchorType.HorStretchMiddle;
                    break;
                case "SCALE LEFT_RIGHT":
                    anchorPreset = AnchorType.HorStretchMiddle;
                    break;
                ////////////////CENTER////////////////
                case "TOP CENTER":
                    anchorPreset = AnchorType.TopCenter;
                    break;
                case "BOTTOM CENTER":
                    anchorPreset = AnchorType.BottomCenter;
                    break;
                case "TOP_BOTTOM CENTER":
                    anchorPreset = AnchorType.VertStretchCenter;
                    break;
                case "CENTER CENTER":
                    anchorPreset = AnchorType.MiddleCenter;
                    break;
                case "SCALE CENTER":
                    anchorPreset = AnchorType.StretchAll;
                    break;
                ////////////////SCALE////////////////
                case "TOP SCALE":
                    anchorPreset = AnchorType.HorStretchTop;
                    break;
                case "BOTTOM SCALE":
                    anchorPreset = AnchorType.HorStretchBottom;
                    break;
                case "TOP_BOTTOM SCALE":
                    anchorPreset = AnchorType.VertStretchCenter;
                    break;
                case "CENTER SCALE":
                    anchorPreset = AnchorType.StretchAll;
                    break;
                case "SCALE SCALE":
                    anchorPreset = AnchorType.StretchAll;
                    break;
                ////////////////DEFAULT////////////////
                default:
                    anchorPreset = AnchorType.MiddleCenter;
                    break;
            }

            return anchorPreset;
        }
    }
}