using System;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public struct FindedTag
    {
        public FCU_Tag Tag;
        public string CustomTag;
        public GameObject CustomPrefab;
    }
}