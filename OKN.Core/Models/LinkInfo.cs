using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OKN.Core.Models
{
    public class LinkInfo
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ELinkTypes Type { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}