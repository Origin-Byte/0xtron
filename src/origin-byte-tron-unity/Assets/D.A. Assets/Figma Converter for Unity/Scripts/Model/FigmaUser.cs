using System;

#if JSON_NET_EXISTS
using Newtonsoft.Json;
#endif

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public struct FigmaUser
    {
#if JSON_NET_EXISTS
        [JsonProperty("id")]
#endif
        public string Id;
#if JSON_NET_EXISTS
        [JsonProperty("email")]
#endif
        public string Email;
#if JSON_NET_EXISTS
        [JsonProperty("handle")]
#endif
        public string Handle;
#if JSON_NET_EXISTS
        [JsonProperty("img_url")]
#endif
        public string ImgUrl;
    }
}