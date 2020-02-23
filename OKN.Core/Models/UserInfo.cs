using Newtonsoft.Json;

namespace OKN.Core.Models
{
    public class UserInfo
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("name")]
        public string UserName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}