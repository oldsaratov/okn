using Newtonsoft.Json;

namespace OKN.Core.Models
{   
    public class FileInfo
    {
        [JsonProperty("fileId")]
        public string FileId { get; set; }
        
        [JsonProperty("url")]
        public string Url { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}