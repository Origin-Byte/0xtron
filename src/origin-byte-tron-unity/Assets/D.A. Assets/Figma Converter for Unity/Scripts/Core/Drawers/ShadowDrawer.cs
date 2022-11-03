using DA_Assets.FCU.Model;
using System;
using System.Collections;
using DA_Assets.Shared;
using DA_Assets.FCU.Config;
using UnityEngine;
using DA_Assets.Shared.CodeHelpers;

#if TRUESHADOW_EXISTS
using LeTai.TrueShadow;
#endif

namespace DA_Assets.FCU.Core.Drawers
{
    [Serializable]
    public class ShadowDrawer : ControllerHolder<FigmaConverterUnity>
    {
        public IEnumerator Draw(FObject fobject)
        {
#if TRUESHADOW_EXISTS
            switch (controller.Model.MainSettings.ShadowComponent)
            {
                case ShadowComponent.TrueShadow:
                    yield return controller.Model.DynamicCoroutine(DrawTrueShadow(fobject));
                    break;
            }
#endif
            yield break;
        }
#if TRUESHADOW_EXISTS
        private IEnumerator DrawTrueShadow(FObject fobject)
        {
            TrueShadow[] oldShadows = fobject.GameObject.GetComponents<TrueShadow>();

            foreach (TrueShadow oldShadow in oldShadows)
            {
                oldShadow.Destroy();
            }

            foreach (Effect effect in fobject.Effects)
            {
                if (effect.Type.Contains("SHADOW"))
                {
                    TrueShadow trueShadow = fobject.GameObject.AddComponent<TrueShadow>();

                    if (trueShadow == null)
                    {
                        continue;
                    }

                    trueShadow.Offset.Set(effect.Offset.x, effect.Offset.y);
                    trueShadow.Color = effect.Color;
                    trueShadow.Size = effect.Radius;

                    if (effect.Type.Contains("DROP"))
                    {
                        trueShadow.Inset = false;
                    }
                    else
                    {
                        trueShadow.Inset = true;
                    }

                    if (effect.Visible.ToBool())
                    {
                        trueShadow.enabled = false;
                    }
                }
            }

            yield break;
        }
#endif
    }
}