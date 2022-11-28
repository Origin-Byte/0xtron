using DA_Assets.FCU.Config;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.CodeHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.FCU.Core.Drawers
{
    [Serializable]
    public class MaskDrawer : ControllerHolder<FigmaConverterUnity>
    {
        private List<FObject> figmaMasksToDestroy;
        public void Init()
        {
            figmaMasksToDestroy = new List<FObject>();
        }
        public IEnumerator Draw(FObject fobject)
        {
            FObject target;

            if (fobject.ClipsContent.ToBool())
            {
                target = fobject;
            }
            else
            {
                target = fobject.Parent;

                yield return controller.CanvasDrawer.ImageDrawer.Draw(fobject, target);
                figmaMasksToDestroy.Add(fobject);
                yield return controller.CanvasDrawer.ImageDrawer.Draw(target, target);
            }

            InstantiateUnityMask(target);
        }
        private Mask InstantiateUnityMask(FObject unityMaskImage)
        {
            unityMaskImage.GameObject.TryAddComponent(out Mask unityMask);
            return unityMask;
        }
        public IEnumerator DestroyFigmaMasksImages()
        {
            foreach (FObject figmaMask in figmaMasksToDestroy)
            {
                figmaMask.GameObject.Destroy();
                yield return new WaitForSecondsRealtime(FCU_Config.Instance.Delay001);
            }
        }
    }
}
