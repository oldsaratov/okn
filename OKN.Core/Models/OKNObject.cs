using Newtonsoft.Json;

namespace OKN.Core.Models
{
    public class OKNObject
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("latitude")]
        public decimal Latitude { get; set; }
        
        [JsonProperty("longitude")]
        public decimal Longitude { get; set; }
        
        [JsonProperty("type")]
        public EObjectType Type { get; set; }
    }
}