#if JSON_NET_EXISTS
using Newtonsoft.Json;
#endif

namespace DA_Assets.FCU.Model
{
    public class FigmaError
    {
#if JSON_NET_EXISTS
        [JsonProperty("status")]
#endif
        public int Status { get; set; }
#if JSON_NET_EXISTS
        [JsonProperty("err")]
#endif
        public string Error { get; set; }
    }
}