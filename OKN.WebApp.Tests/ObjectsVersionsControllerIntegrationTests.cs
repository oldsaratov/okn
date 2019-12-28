using Newtonsoft.Json;
using OKN.Core.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace OKN.WebApp.Tests
{
    public class ObjectsVersionsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public ObjectsVersionsControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();

            _factory = factory;
        }

        [Fact]
        public async Task get_object_versions_list()
        {
            _factory.Runner.Import("okn", "objects_versions", "Data/record_versions.json", true);

            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync("/api/objects/5af2796e32522f798f822a41/versions");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<PagedList<VersionInfo>>(stringResponse);

            Assert.Equal(2, obj.Data.Count);
        }

        [Fact]
        public async Task get_object_version()
        {
            _factory.Runner.Import("okn", "objects", "Data/single_record.json", true);
            _factory.Runner.Import("okn", "objects_versions", "Data/record_versions.json", true);

            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync("/api/objects/5af2796e32522f798f822a41/versions/2");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<OknObject>(stringResponse);

            Assert.NotNull(obj.Version);
            Assert.Equal(2, obj.Version.VersionId);
        }
    }
}
