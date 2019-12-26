using Newtonsoft.Json;
using OKN.Core.Models;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OKN.WebApp.Tests
{
    public class ObjectsControllerWithAuthIntegrationTests : IClassFixture<CustomWebApplicationFactoryWithAuth<Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactoryWithAuth<Startup> _factory;

        public ObjectsControllerWithAuthIntegrationTests(CustomWebApplicationFactoryWithAuth<Startup> factory)
        {
            _client = factory.CreateClient();

            _factory = factory;
        }

        [Fact]
        public async Task update_object()
        {
            _factory.Runner.Import("okn", "objects", "1.json", true);

            var content = new StringContent(File.ReadAllText("4.json"), Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync("/api/objects/5af2796e32522f798f822a41", content);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // The endpoint or route of the controller action.
            var httpResponse1 = await _client.GetAsync("/api/objects/5af2796e32522f798f822a41");
            httpResponse1.EnsureSuccessStatusCode();
            var stringResponse = await httpResponse1.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<OknObject>(stringResponse);

            Assert.Equal("5af2796e32522f798f822a41", obj.ObjectId);
            Assert.Equal("UPDATED TEST", obj.Name);
            Assert.Equal("UPDATED TEST DESCRIPTION", obj.Description);
            Assert.Equal("51,5391153", obj.Latitude.ToString());
            Assert.Equal("46,0091007", obj.Longitude.ToString());
            Assert.Equal(EObjectType.Regional, obj.Type);
        }
    }
}
