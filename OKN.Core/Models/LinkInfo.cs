using Newtonsoft.Json;

namespace OKN.Core.Models
{
    public class LinkInfo
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}