using Microsoft.Extensions.Caching.Memory;

namespace AspNet.Security.OAuth.Oldsaratov
{
    internal class TokenMemoryCache
    {
        public MemoryCache Cache { get; set; }

        public TokenMemoryCache()
        {
            Cache = new MemoryCache(new MemoryCacheOptions());
        }
    }
}