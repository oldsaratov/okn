using Newtonsoft.Json;

namespace OKN.WebApp.Models.Objects
{
    public class UpdateObjectViewModel
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }
    }
}
