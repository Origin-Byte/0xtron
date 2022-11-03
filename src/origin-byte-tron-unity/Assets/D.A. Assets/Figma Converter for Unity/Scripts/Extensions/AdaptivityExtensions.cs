﻿using DA_Assets.FCU.Model;
using UnityEngine;

namespace DA_Assets.FCU.Extensions
{
    public static class AdaptivityExtensions
    {
        public static void SetSmartPivot(this RectTransform rect, PivotType pivotType)
        {
            Vector2 pivot = GetPivot(pivotType);

            SetSmartPivot(rect, pivot.x, 0, true, false);
            SetSmartPivot(rect, pivot.y, 1, true, false);
        }
        private static void SetSmartPivot(RectTransform rect, float value, int axis, bool smart, bool parentSpace)
        {
            Vector3 cornerBefore = GetRectReferenceCorner(rect, !parentSpace);

            Vector2 rectPivot = rect.pivot;
            rectPivot[axis] = value;
            rect.pivot = rectPivot;

            if (smart)
            {
                Vector3 cornerAfter = GetRectReferenceCorner(rect, !parentSpace);
                Vector3 cornerOffset = cornerAfter - cornerBefore;
                rect.anchoredPosition -= (Vector2)cornerOffset;

                Vector3 pos = rect.transform.position;
                pos.z -= cornerOffset.z;
                rect.transform.position = pos;
            }
        }
        private static Vector3 GetRectReferenceCorner(RectTransform gui, bool worldSpace)
        {
            Vector3[] s_Corners = new Vector3[4];
            if (worldSpace)
            {
                Transform t = gui.transform;
                gui.GetWorldCorners(s_Corners);
                if (t.parent)
                    return t.parent.InverseTransformPoint(s_Corners[0]);
                else
                    return s_Corners[0];
            }
            return (Vector3)gui.rect.min + gui.transform.localPosition;
        }
        public static void SetRotation(this RectTransform source, float value)
        {
            source.localRotation = Quaternion.Euler(
                source.localRotation.x, 
                source.localRotation.y, 
                value);
        }
        public static AnchorMinMax GetAnchor(AnchorType anchorPreset)
        {
            AnchorMinMax minMax = new AnchorMinMax();
            switch (anchorPreset)
            {
                case AnchorType.TopLeft:
                    {
                        minMax.Min = new Vector2(0, 1);
                        minMax.Max = new Vector2(0, 1);
                        break;
                    }
                case AnchorType.TopCenter:
                    {
                        minMax.Min = new Vector2(0.5f, 1);
                        minMax.Max = new Vector2(0.5f, 1);
                        break;
                    }
                case AnchorType.TopRight:
                    {
                        minMax.Min = new Vector2(1, 1);
                        minMax.Max = new Vector2(1, 1);
                        break;
                    }

                case AnchorType.MiddleLeft:
                    {
                        minMax.Min = new Vector2(0, 0.5f);
                        minMax.Max = new Vector2(0, 0.5f);
                        break;
                    }
                case AnchorType.MiddleCenter:
                    {
                        minMax.Min = new Vector2(0.5f, 0.5f);
                        minMax.Max = new Vector2(0.5f, 0.5f);
                        break;
                    }
                case AnchorType.MiddleRight:
                    {
                        minMax.Min = new Vector2(1, 0.5f);
                        minMax.Max = new Vector2(1, 0.5f);
                        break;
                    }

                case AnchorType.BottomLeft:
                    {
                        minMax.Min = new Vector2(0, 0);
                        minMax.Max = new Vector2(0, 0);
                        break;
                    }
                case AnchorType.BottomCenter:
                    {
                        minMax.Min = new Vector2(0.5f, 0);
                        minMax.Max = new Vector2(0.5f, 0);
                        break;
                    }
                case AnchorType.BottomRight:
                    {
                        minMax.Min = new Vector2(1, 0);
                        minMax.Max = new Vector2(1, 0);
                        break;
                    }

                case AnchorType.HorStretchTop:
                    {
                        minMax.Min = new Vector2(0, 1);
                        minMax.Max = new Vector2(1, 1);
                        break;
                    }
                case AnchorType.HorStretchMiddle:
                    {
                        minMax.Min = new Vector2(0, 0.5f);
                        minMax.Max = new Vector2(1, 0.5f);
                        break;
                    }
                case AnchorType.HorStretchBottom:
                    {
                        minMax.Min = new Vector2(0, 0);
                        minMax.Max = new Vector2(1, 0);
                        break;
                    }

                case AnchorType.VertStretchLeft:
                    {
                        minMax.Min = new Vector2(0, 0);
                        minMax.Max = new Vector2(0, 1);
                        break;
                    }
                case AnchorType.VertStretchCenter:
                    {
                        minMax.Min = new Vector2(0.5f, 0);
                        minMax.Max = new Vector2(0.5f, 1);
                        break;
                    }
                case AnchorType.VertStretchRight:
                    {
                        minMax.Min = new Vector2(1, 0);
                        minMax.Max = new Vector2(1, 1);
                        break;
                    }

                case AnchorType.StretchAll:
                    {
                        minMax.Min = new Vector2(0, 0);
                        minMax.Max = new Vector2(1, 1);
                        break;
                    }
            }

            return minMax;
        }
        public static Vector2 GetPivot(PivotType preset)
        {
            switch (preset)
            {
                case PivotType.TopLeft:
                    {
                        return new Vector2(0, 1);
                    }
                case PivotType.TopCenter:
                    {
                        return new Vector2(0.5f, 1);
                    }
                case PivotType.TopRight:
                    {
                        return new Vector2(1, 1);
                    }
                case PivotType.MiddleLeft:
                    {
                        return new Vector2(0, 0.5f);
                    }
                case PivotType.MiddleCenter:
                    {
                        return new Vector2(0.5f, 0.5f);
                    }
                case PivotType.MiddleRight:
                    {
                        return new Vector2(1, 0.5f);
                    }
                case PivotType.BottomLeft:
                    {
                        return new Vector2(0, 0);
                    }
                case PivotType.BottomCenter:
                    {
                        return new Vector2(0.5f, 0);
                    }
                case PivotType.BottomRight:
                    {
                        return new Vector2(1, 0);
                    }
                default:
                    return GetPivot(PivotType.MiddleCenter);
            }
        }
        public static void SetAnchorSmart(this RectTransform rect, AnchorType anchorType)
        {
            AnchorMinMax anchor = GetAnchor(anchorType);

            rect.SetAnchorSmart(RectAxis.Hor, anchor.Min.x, false);
            rect.SetAnchorSmart(RectAxis.Hor, anchor.Max.x, true);

            rect.SetAnchorSmart(RectAxis.Vert, anchor.Min.y, false);
            rect.SetAnchorSmart(RectAxis.Vert, anchor.Max.y, true);
        }
        private static void SetAnchorSmart(this RectTransform rect, RectAxis rectAxis, float value, bool isMax)
        {
            bool smart = true;
            int _axis = (int)rectAxis;

            RectTransform parent = null;

            if (rect.transform.parent == null)
            {
                smart = false;
            }
            else
            {
                parent = rect.transform.parent.GetComponent<RectTransform>();

                if (parent == null)
                {
                    smart = false;
                }
            }

            float offsetSizePixels = 0;
            float offsetPositionPixels = 0;
            if (smart)
            {
                float oldValue = isMax ? rect.anchorMax[_axis] : rect.anchorMin[_axis];

                offsetSizePixels = (value - oldValue) * parent.rect.size[_axis];

                float roundingDelta = 0;

                Canvas canvas = rect.gameObject.GetComponentInParent<Canvas>();
                bool shouldDoIntSnapping = canvas != null && canvas.renderMode != RenderMode.WorldSpace;

                if (shouldDoIntSnapping)
                    roundingDelta = Mathf.Round(offsetSizePixels) - offsetSizePixels;
                offsetSizePixels += roundingDelta;

                offsetPositionPixels = (isMax ? offsetSizePixels * rect.pivot[_axis] : (offsetSizePixels * (1 - rect.pivot[_axis])));
            }

            if (isMax)
            {
                Vector2 rectAnchorMax = rect.anchorMax;
                rectAnchorMax[_axis] = value;
                rect.anchorMax = rectAnchorMax;

                Vector2 other = rect.anchorMin;

                rect.anchorMin = other;
            }
            else
            {
                Vector2 rectAnchorMin = rect.anchorMin;
                rectAnchorMin[_axis] = value;
                rect.anchorMin = rectAnchorMin;

                Vector2 other = rect.anchorMax;
                rect.anchorMax = other;
            }

            if (smart)
            {
                Vector2 rectPosition = rect.anchoredPosition;
                rectPosition[_axis] -= offsetPositionPixels;
                rect.anchoredPosition = rectPosition;

                Vector2 rectSizeDelta = rect.sizeDelta;
                rectSizeDelta[_axis] += offsetSizePixels * (isMax ? -1 : 1);
                rect.sizeDelta = rectSizeDelta;
            }
        } 
    }
}