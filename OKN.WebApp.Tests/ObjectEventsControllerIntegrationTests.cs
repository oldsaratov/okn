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
    public class ObjectEventsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public ObjectEventsControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();

            _factory = factory;
        }

        [Fact]
        public async Task get_object_event()
        {
            _factory.Runner.Import("okn", "objects", "Data/single_record.json", true);

            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync("/api/objects/5af2796e32522f798f822a41/events/c1099b06-99e9-423d-acd3-66ec56ac2c2d");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<OknObjectEvent>(stringResponse);

            Assert.Equal("c1099b06-99e9-423d-acd3-66ec56ac2c2d", obj.EventId);
            Assert.Equal("event 1", obj.Name);
        }

        [Fact]
        public async Task get_invalid_object_event_return_404()
        {
            _factory.Runner.Import("okn", "objects", "Data/single_record.json", true);

            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync("/api/objects/5af2796e32522f798f822a41/events/c1099b06-99e9");

            // Must be successful.
            Assert.Equal(System.Net.HttpStatusCode.NotFound, httpResponse.StatusCode);
        }

        [Fact]
        public async Task get_list_of_object_events()
        {
            _factory.Runner.Import("okn", "objects", "Data/single_record.json", true);
            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync("/api/objects/5af2796e32522f798f822a41/events");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var objects = JsonConvert.DeserializeObject<PagedList<OknObjectEvent>>(stringResponse);

            Assert.Equal(1, objects.Page);
            Assert.Equal(100, objects.PerPage);
            Assert.NotEmpty(objects.Data);
            Assert.Equal(2, objects.Data.Count);
        }

        [Fact]
        public async Task get_list_of_object_events_with_paging()
        {
            _factory.Runner.Import("okn", "objects", "Data/single_record.json", true);
            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync("/api/objects/5af2796e32522f798f822a41/events?page=1&perPage=1");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var objects = JsonConvert.DeserializeObject<PagedList<OknObjectEvent>>(stringResponse);

            Assert.Equal(1, objects.Page);
            Assert.Equal(1, objects.PerPage);
            Assert.NotEmpty(objects.Data);
            Assert.Single(objects.Data);
            Assert.Equal(2, objects.Total);
        }
    }

    public class ObjectEventsControllerWithAuthIntegrationTests : IClassFixture<CustomWebApplicationFactoryWithAuth<Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactoryWithAuth<Startup> _factory;

        public ObjectEventsControllerWithAuthIntegrationTests(CustomWebApplicationFactoryWithAuth<Startup> factory)
        {
            _client = factory.CreateClient();

            _factory = factory;
        }

        [Fact]
        public async Task create_object_event()
        {
            _factory.Runner.Import("okn", "objects", "Data/single_record.json", true);
            _factory.Runner.Import("okn", "objects_versions", "Data/empty.json", true);

            var content = new StringContent(File.ReadAllText("Data/create_event_request.json"), Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

            var httpResponse = await _client.PostAsync("/api/objects/5af2796e32522f798f822a41/events", content);
            httpResponse.EnsureSuccessStatusCode();

            //Assert that event has been updated
            var httpResponse1 = await _client.GetAsync("/api/objects/5af2796e32522f798f822a41/events");
            httpResponse1.EnsureSuccessStatusCode();
            var stringResponse = await httpResponse1.Content.ReadAsStringAsync();
            var objects = JsonConvert.DeserializeObject<PagedList<OknObjectEvent>>(stringResponse);

            Assert.Equal(1, objects.Page);
            Assert.Equal(100, objects.PerPage);
            Assert.NotEmpty(objects.Data);
            Assert.Equal(3, objects.Data.Count);

            Assert.Contains(objects.Data, result => result.Name == "new event");

            //Assert that object versions list contains new version
            await AssertHelpers.AssertObjectVersionsListHasNewRecord(_client);

            //Assert that object version has been updated
            await AssertHelpers.AssertObjectHasNewVersion(_client);
        }

        [Fact]
        public async Task update_object_event()
        {
            _factory.Runner.Import("okn", "objects", "Data/single_record.json", true);
            _factory.Runner.Import("okn", "objects_versions", "Data/empty.json", true);

            var content = new StringContent(File.ReadAllText("Data/update_event_request.json"), Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync("/api/objects/5af2796e32522f798f822a41/events/c1099b06-99e9-423d-acd3-66ec56ac2c2d", content);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // The endpoint or route of the controller action.
            var httpResponse1 = await _client.GetAsync("/api/objects/5af2796e32522f798f822a41/events/c1099b06-99e9-423d-acd3-66ec56ac2c2d");
            httpResponse1.EnsureSuccessStatusCode();
            var stringResponse = await httpResponse1.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<OknObjectEvent>(stringResponse);

            Assert.Equal("c1099b06-99e9-423d-acd3-66ec56ac2c2d", obj.EventId);
            Assert.Equal("event updated", obj.Name);

            //Assert that object versions list contains new version
            await AssertHelpers.AssertObjectVersionsListHasNewRecord(_client);

            //Assert that object version has been updated
            await AssertHelpers.AssertObjectHasNewVersion(_client);
        }


        [Fact]
        public async Task delete_object_event()
        {
            _factory.Runner.Import("okn", "objects", "Data/single_record.json", true);
            _factory.Runner.Import("okn", "objects_versions", "Data/empty.json", true);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

            // The endpoint or route of the controller action.
            var httpResponse = await _client.DeleteAsync("/api/objects/5af2796e32522f798f822a41/events/c1099b06-99e9-423d-acd3-66ec56ac2c2d");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // The endpoint or route of the controller action.
            var httpResponse1 = await _client.GetAsync("/api/objects/5af2796e32522f798f822a41/events/c1099b06-99e9-423d-acd3-66ec56ac2c2d");

            Assert.Equal(System.Net.HttpStatusCode.NotFound, httpResponse1.StatusCode);

            //Assert that object versions list contains new version
            await AssertHelpers.AssertObjectVersionsListHasNewRecord(_client);

            //Assert that object version has been updated
            await AssertHelpers.AssertObjectHasNewVersion(_client);
        }
    }
}
