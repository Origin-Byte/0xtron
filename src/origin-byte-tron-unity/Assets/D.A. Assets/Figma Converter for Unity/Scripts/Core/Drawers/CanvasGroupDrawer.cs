using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.CodeHelpers;
using System;
using System.Collections;
using UnityEngine;

namespace DA_Assets.FCU.Core.Drawers
{
    [Serializable]
    public class CanvasGroupDrawer : ControllerHolder<FigmaConverterUnity>
    {
        public IEnumerator Draw(FObject fobject)
        {
            fobject.GameObject.TryAddComponent(out CanvasGroup canvasGroup);
            canvasGroup.alpha = (float)fobject.Opacity;
            yield break;
        }
    }
}