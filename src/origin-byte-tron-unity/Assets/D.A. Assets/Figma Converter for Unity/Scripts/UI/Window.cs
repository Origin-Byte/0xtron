using DA_Assets.Shared;
using DA_Assets.Shared.CodeHelpers;
using System;
using UnityEngine;

namespace DA_Assets.FCU.UI
{
    public class Window : MonoBehaviour
    {
        public IdNameInstanceId IdNameInstanceId;
        public Canvas Canvas;
        public void Awake()
        {
            gameObject.TryAddComponent(out Canvas canvas);
            canvas.overrideSorting = true;
            this.Canvas = canvas;
        }
    }
    [Serializable]
    public struct IdNameInstanceId
    {
        public string Id;
        public string Name;
        public int InstanceId;
    }
}