using DA_Assets.Shared;
using System;
using System.Collections.Generic;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public struct AssemblyConfig
    {
        public string Name;
        public AssetType AssetType;
        public string ScriptingDefineName;
        public UpdateBool Enabled;
        public List<string> Data;
    }
    public enum AssetType
    {
        JsonNET,
        TextMeshPro,
        TrueShadow,
        MPUIKit,
        ProceduralUIImage,
        I2Localization
    }
}