using Newtonsoft.Json;

namespace OKN.WebApp.Models
{
    public class FileInfoViewModel
    {
        [JsonProperty("fileId")]
        public string FileId { get; set; }
        
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}