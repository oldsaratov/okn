using System.Collections.Generic;
using Newtonsoft.Json;
using OKN.Core.Models;

namespace OKN.WebApp.Models.Objects
{
    public class UpdateObjectViewModel
	{
	    [JsonProperty("name")]
	    public string Name { get; set; }

	    [JsonProperty("description")]
	    public string Description { get; set; }
        
	    [JsonProperty("latitude")]
	    public string Latitude { get; set; }
        
	    [JsonProperty("longitude")]
	    public string Longitude { get; set; }
        
	    [JsonProperty("type")]
	    public EObjectType Type { get; set; }
	    
	    [JsonProperty("mainPhoto")]
	    public FileInfoViewModel MainPhoto { get; set; }
        
	    [JsonProperty("photos")]
	    public List<FileInfoViewModel> Photos { get; set; }
    }
}
