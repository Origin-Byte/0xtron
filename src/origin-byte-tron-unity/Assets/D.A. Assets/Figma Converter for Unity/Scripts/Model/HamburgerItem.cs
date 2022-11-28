using System;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class HamburgerItem
    {
        public string Id;
        public bool AddCheckBox;
        public bool State;
        public bool CheckBoxValue;
        public bool CheckBoxValueTemp;
        public GUIContent GUIContent;
        public bool DrawnInsideLayoutGroup;
        public Action Body;
        public Action<string, bool> CheckBoxValueChanged;
    }
}