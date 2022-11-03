using DA_Assets.FCU.Core;
using System;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class ProceduralImageSettings : ControllerHolder<FigmaConverterUnity>
    {
        [SerializeField] UnityImageType type = UnityImageType.Simple;
        [SerializeField] bool raycastTarget = true;
        [SerializeField] ModifierType modifierType = ModifierType.Free;
        [SerializeField] float borderWidth = 0;
        [SerializeField] float falloffDistance = 1;
        public UnityImageType Type { get => type; set => SetValue(ref type, value); }
        public bool RaycastTarget { get => raycastTarget; set => SetValue(ref raycastTarget, value); }
        public ModifierType ModifierType { get => modifierType; set => SetValue(ref modifierType, value); }
        public float BorderWidth { get => borderWidth; set => SetValue(ref borderWidth, value); }
        public float FalloffDistance { get => falloffDistance; set => SetValue(ref falloffDistance, value); }
    }
}