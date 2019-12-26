using Newtonsoft.Json;
using OKN.Core.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace OKN.WebApp.Tests
{
    public class ObjectsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public ObjectsControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();

            _factory = factory;
        }

        [Fact]
        public async Task get_object()
        {
            _factory.Runner.Import("okn", "objects", "1.json", true);

            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync("/api/objects/5af2796e32522f798f822a41");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<OknObject>(stringResponse);

            Assert.Equal("5af2796e32522f798f822a41", obj.ObjectId);
            Assert.Equal("TEST", obj.Name);
            Assert.Equal("TEST DESCRIPTION", obj.Description);
            Assert.Equal("51,5391153", obj.Latitude.ToString());
            Assert.Equal("46,0091007", obj.Longitude.ToString());
            Assert.Equal(EObjectType.Federal, obj.Type);
        }

        [Fact]
        public async Task get_invalid_object_return_404()
        {
            _factory.Runner.Import("okn", "objects", "1.json", true);

            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync("/api/objects/5af2796e32");

            // Must be successful.
            Assert.Equal(System.Net.HttpStatusCode.NotFound, httpResponse.StatusCode);
        }

        [Fact]
        public async Task get_list_of_object()
        {
            _factory.Runner.Import("okn", "objects", "3.json", true);
            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync("/api/objects");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var objects = JsonConvert.DeserializeObject<PagedList<OknObject>>(stringResponse);

            Assert.Equal(1, objects.Page);
            Assert.Equal(100, objects.PerPage);
            Assert.NotEmpty(objects.Data);
            Assert.Equal(9, objects.Data.Count);
        }

        [Fact]
        public async Task get_list_of_object_with_paging()
        {
            _factory.Runner.Import("okn", "objects", "3.json", true);
            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync("/api/objects?page=2&perPage=2");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var objects = JsonConvert.DeserializeObject<PagedList<OknObject>>(stringResponse);

            Assert.Equal(2, objects.Page);
            Assert.Equal(2, objects.PerPage);
            Assert.NotEmpty(objects.Data);
            Assert.Equal(2, objects.Data.Count);
            Assert.Equal(9, objects.Total);
        }
    }
}
