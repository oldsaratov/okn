using Newtonsoft.Json;
using OKN.Core.Models;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
            _factory.Runner.Import("okn", "objects", "Data/single_record.json", true);

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
        public async Task get_last_object_version()
        {
            _factory.Runner.Import("okn", "objects", "Data/single_record.json", true);
            _factory.Runner.Import("okn", "objects_versions", "Data/record_versions.json", true);

            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync("/api/objects/5af2796e32522f798f822a41");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<OknObject>(stringResponse);

            Assert.NotNull(obj.Version);
            Assert.Equal(3, obj.Version.VersionId);
        }

        [Fact]
        public async Task get_invalid_object_return_404()
        {
            _factory.Runner.Import("okn", "objects", "Data/single_record.json", true);

            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync("/api/objects/5af2796e32");

            // Must be successful.
            Assert.Equal(System.Net.HttpStatusCode.NotFound, httpResponse.StatusCode);
        }

        [Fact]
        public async Task get_list_of_object()
        {
            _factory.Runner.Import("okn", "objects", "Data/many_records.json", true);
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
            _factory.Runner.Import("okn", "objects", "Data/many_records.json", true);
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

        [Fact]
        public async Task get_list_of_object_with_filter()
        {
            var nameToken = "здание";
            var types = new[] { (int)EObjectType.Municipal, (int)EObjectType.Regional };

            _factory.Runner.Import("okn", "objects", "Data/many_records.json", true);
            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync($"/api/objects?name={nameToken}&types={string.Join(",", types)}");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var objects = JsonConvert.DeserializeObject<PagedList<OknObject>>(stringResponse);

            Assert.Equal(1, objects.Page);
            Assert.Equal(100, objects.PerPage);
            Assert.NotEmpty(objects.Data);
            Assert.Equal(4, objects.Data.Count);
            Assert.Equal(4, objects.Total);

            Assert.All(objects.Data, result => Assert.Contains(nameToken, result.Name.ToLower()));
            Assert.All(objects.Data, result => Assert.Contains((int)result.Type, types));
        }
    }

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
            _factory.Runner.Import("okn", "objects", "Data/single_record.json", true);

            var content = new StringContent(File.ReadAllText("Data/update_request_with_photos.json"), Encoding.UTF8, "application/json");
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
            Assert.Equal("51.5391153", obj.Latitude.ToString(new CultureInfo("en-us")));
            Assert.Equal("46.0091007", obj.Longitude.ToString(new CultureInfo("en-us")));
            Assert.Equal(EObjectType.Regional, obj.Type);

            ValidateVersion(obj.Version);
        }

        private void ValidateVersion(VersionInfo version)
        {
            Assert.NotNull(version);
            Assert.Equal(1, version.VersionId);

            ValidateUser(version.Author);
        }

        private void ValidateUser(UserInfo versionAuthor)
        {
            Assert.Equal("1", versionAuthor.UserId);
            Assert.Equal("TestUser", versionAuthor.UserName);
            Assert.Equal("test@test.com", versionAuthor.Email);
        }
    }
}
