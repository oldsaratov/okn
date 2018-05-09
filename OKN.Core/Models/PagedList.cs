using System.Collections.Generic;
using Newtonsoft.Json;

namespace OKN.Core.Models
{
    public class PagedList<T> where T : class
	{
		[JsonProperty("data")]
        public List<T> Data { get; set; }

		[JsonProperty("page")]
		public long Page { get; set; }
		
        [JsonProperty("perPage")]
		public long PerPage { get; set; }
		
        [JsonProperty("total")]
        public long Total { get; set; }
    }
}
