using System;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public struct TagConfig
    {
        [SerializeField] string label;
        [SerializeField] FCU_Tag fcuTag;
        [SerializeField] string figmaTag;
        [SerializeField] bool isParent;
        [SerializeField] bool isDownloadable;
        [SerializeField] bool canBeInsideSingleImage;
        public string Label { get => label; set => label = value; }
        public FCU_Tag FCU_Tag { get => fcuTag; set => fcuTag = value; }
        public string FigmaTag { get => figmaTag; set => figmaTag = value; }
        public bool IsParent { get => isParent; set => isParent = value; }
        public bool IsDownloadable { get => isDownloadable; set => isDownloadable = value; }
        public bool CanBeInsideSingleImage { get => canBeInsideSingleImage; set => canBeInsideSingleImage = value; }
    }
}