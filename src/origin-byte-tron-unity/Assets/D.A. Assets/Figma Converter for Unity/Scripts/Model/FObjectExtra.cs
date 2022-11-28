using System.Collections.Generic;
using UnityEngine;

#if JSON_NET_EXISTS
using Newtonsoft.Json;
#endif

namespace DA_Assets.FCU.Model
{
    public class FObjectExtra
    {
        public string Hash { get; set; }
#if JSON_NET_EXISTS
        [JsonIgnore]
#endif
        public string Hierarchy { get; set; }
#if JSON_NET_EXISTS
        [JsonIgnore]
#endif
        public string FixedName { get; set; }
#if JSON_NET_EXISTS
        [JsonIgnore]
#endif
        public FCU_Meta Meta { get; set; }
#if JSON_NET_EXISTS
        [JsonIgnore]
#endif
        public FObject Parent { get; set; }
#if JSON_NET_EXISTS
        [JsonIgnore]
#endif
        public bool IsMutual { get; set; }

#if JSON_NET_EXISTS
        [JsonIgnore]
#endif
        public GameObject GameObject { get; set; }

#if JSON_NET_EXISTS
        [JsonIgnore]
#endif
        public List<FCU_Tag> Tags { get; set; }
#if JSON_NET_EXISTS
        [JsonIgnore]
#endif
        public string CustomTag { get; set; }
#if JSON_NET_EXISTS
        [JsonIgnore]
#endif
        public GameObject CustomPrefab { get; set; }
#if JSON_NET_EXISTS
        [JsonIgnore]
#endif
        public string FilePath { get; set; }
#if JSON_NET_EXISTS
        [JsonIgnore]
#endif
        public bool DownloadableFile { get; set; }
#if JSON_NET_EXISTS
        [JsonIgnore]
#endif
        public string AssetPath { get; set; }
#if JSON_NET_EXISTS
        [JsonIgnore]
#endif
        public string Link { get; set; }
#if JSON_NET_EXISTS
        [JsonIgnore]
#endif
        public bool ManualTagExists { get; set; }
    }
}