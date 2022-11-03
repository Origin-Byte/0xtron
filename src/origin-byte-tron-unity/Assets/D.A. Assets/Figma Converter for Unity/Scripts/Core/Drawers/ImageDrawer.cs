using DA_Assets.FCU.Model;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using DA_Assets.FCU.Extensions;
using System;
using DA_Assets.Shared.CodeHelpers;

#if PUI_EXISTS
using UnityEngine.UI.ProceduralImage;
#endif

#if MPUIKIT_EXISTS
using MPUIKIT;
#endif

namespace DA_Assets.FCU.Core.Drawers
{
    [Serializable]
    public class ImageDrawer : ControllerHolder<FigmaConverterUnity>
    {
        public IEnumerator Draw(FObject source, FObject target)
        {
            switch (controller.Model.MainSettings.ImageComponent)
            {
#if PUI_EXISTS
                case ImageComponent.ProceduralImage:
                    yield return controller.Model.DynamicCoroutine(DrawProceduralUI_Image(source, target));
                    break;
#endif
#if MPUIKIT_EXISTS
                case ImageComponent.MPImage:
                    yield return controller.Model.DynamicCoroutine(DrawMPImage(source, target));
                    break;
#endif
                default:
                    yield return controller.Model.DynamicCoroutine(DrawDefaultImage(source, target));
                    break;
            }
        }
        private IEnumerator DrawDefaultImage(FObject source, FObject target)
        {
            if (source.Meta.IsDownloadable)
            {
                target.GameObject.TryAddGraphic(out Image img);
#if UNITY_EDITOR
                yield return controller.Tools.GetSprite(source, result => img.sprite = result.Result);
#endif
            }
            else
            {
                if (source.Fills.IsEmpty())
                {
                    yield break;
                }

                target.GameObject.TryAddGraphic(out Image img);

                if (source.Fills[0].Color == new Color(0, 0, 0, 0))
                {
                    if (source.Fills[0].GradientStops.IsEmpty() == false)
                    {
                        img.color = source.Fills[0].GradientStops[0].Color;

                        if (source.Fills[0].Visible != null)
                        {
                            img.enabled = source.Fills[0].Visible.ToBool();
                        }
                    }
                }
                else
                {
                    img.color = source.Fills[0].Color;

                    if (source.Fills[0].Visible != null)
                    {
                        img.enabled = source.Fills[0].Visible.ToBool();
                    }
                }
            }
        }
#if PUI_EXISTS
        private IEnumerator DrawProceduralUI_Image(FObject source, FObject target)
        {
            target.GameObject.TryAddGraphic(out ProceduralImage img);

            if (source.Type == "ELLIPSE")
            {
                target.GameObject.TryAddComponent(out RoundModifier roundModifier);
            }
            else
            {
                if (source.RectangleCornerRadius != null)
                {
                    target.GameObject.TryAddComponent(out FreeModifier freeModifier);

                    freeModifier.Radius = new Vector4
                    {
                        x = source.RectangleCornerRadius[0],
                        y = source.RectangleCornerRadius[1],
                        z = source.RectangleCornerRadius[2],
                        w = source.RectangleCornerRadius[3]
                    };
                }
                else
                {
                    target.GameObject.TryAddComponent(out UniformModifier uniformModifier);
                    uniformModifier.Radius = source.CornerRadius;
                }
            }

            if (source.Meta.IsDownloadable)
            {
#if UNITY_EDITOR
                yield return controller.Tools.GetSprite(source, result => img.sprite = result.Result);
#endif
            }
            else
            {
                bool anyFillVisible = false;

                foreach (Fill fill in source.Fills)
                {
                    if (fill.Type == "SOLID")
                    {
                        if (fill.Opacity != null)
                        {
                            Color _color = fill.Color;
                            _color.a = (float)fill.Opacity;
                            img.color = _color;
                        }
                        else
                        {
                            img.color = fill.Color;
                        }
                    }

                    if (fill.Visible == null || fill.Visible.ToBool() == true)
                    {
                        anyFillVisible = true;
                    }
                }

                img.enabled = anyFillVisible;
            }

            yield return null;
        }
#endif
#if MPUIKIT_EXISTS
        private IEnumerator DrawMPImage(FObject source, FObject target)
        {
            MPImage img = null;

            if (source.Meta.IsDownloadable)
            {
                target.GameObject.TryAddGraphic(out img);
#if UNITY_EDITOR
                yield return controller.Tools.GetSprite(source, result => img.sprite = result.Result);
#endif
            }
            else
            {
                if (source.Fills.IsEmpty())
                {
                    yield break;
                }

                target.GameObject.TryAddGraphic(out img);

                foreach (Fill fill in source.Fills)
                {
                    if (fill.Type == "SOLID")
                    {
                        if (fill.Opacity != null)
                        {
                            Color _color = fill.Color;
                            _color.a = (float)fill.Opacity;
                            img.color = _color;
                        }
                        else
                        {
                            img.color = fill.Color;
                        }

                        break;
                    }
                }

                if (source.Strokes != null && source.Strokes.Count() > 0)
                {
                    img.OutlineWidth = source.StrokeWeight;
                    img.OutlineColor = source.Strokes[0].Color;
                }

                if (source.Type == "RECTANGLE" || source.Type == "FRAME")
                {
                    img.DrawShape = DrawShape.Rectangle;

                    if (source.RectangleCornerRadius != null)
                    {
                        img.Rectangle = new Rectangle
                        {
                            CornerRadius = new Vector4
                            {
                                x = source.RectangleCornerRadius[3],
                                y = source.RectangleCornerRadius[2],
                                z = source.RectangleCornerRadius[1],
                                w = source.RectangleCornerRadius[0]
                            }
                        };
                    }
                    else if (source.CornerRadius != 0)
                    {
                        img.Rectangle = new Rectangle
                        {
                            CornerRadius = new Vector4
                            {
                                x = source.CornerRadius,
                                y = source.CornerRadius,
                                z = source.CornerRadius,
                                w = source.CornerRadius
                            }
                        };
                    }
                }
                else if (source.Type == "ELLIPSE")
                {
                    img.DrawShape = DrawShape.Circle;
                    img.Circle = new Circle
                    {
                        FitToRect = true
                    };
                }

                bool allFillsDisabled = source.Fills.Where(x => x.Visible != null && x.Visible.ToBool() == false).Count() == source.Fills.Count();

                if (allFillsDisabled)
                {
                    img.enabled = false;
                }
            }

            foreach (Fill fill in source.Fills)
            {
                if (fill.Type == "GRADIENT_LINEAR")
                {
                    Gradient gradient = new Gradient
                    {
                        mode = GradientMode.Blend,
                    };

                    List<GradientColorKey> gradientColorKeys = new List<GradientColorKey>();

                    foreach (GradientStop gradientStop in fill.GradientStops)
                    {
                        gradientColorKeys.Add(new GradientColorKey
                        {
                            color = gradientStop.Color,
                            time = gradientStop.Position
                        });
                    }

                    gradient.colorKeys = gradientColorKeys.ToArray();
                    img.GradientEffect = new GradientEffect
                    {
                        Enabled = true,
                        GradientType = GradientType.Linear,
                        Gradient = gradient
                    };

                    if (source.ContainsTag(FCU_Tag.Frame))
                    {
                        img.GradientEffect = new GradientEffect
                        {
                            Enabled = true,
                            GradientType = GradientType.Linear,
                            Gradient = gradient
                        };
                    }
                    else
                    {
                        img.GradientEffect = new GradientEffect
                        {
                            Enabled = false,
                            GradientType = GradientType.Linear,
                            Gradient = gradient
                        };
                    }

                    break;
                }
            }

            img?.Init();
        }
#endif
    }
}
