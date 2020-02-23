using Newtonsoft.Json;
using OKN.Core.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace OKN.WebApp.Tests
{
    public static class AssertHelpers
    {
        public static void ValidateVersion(int expectedVesion, VersionInfo version)
        {
            Assert.NotNull(version);
            Assert.Equal(expectedVesion, version.VersionId);

            ValidateUser(version.Author);
        }

        public static void ValidateUser(UserInfo versionAuthor)
        {
            Assert.Equal("1", versionAuthor.UserId);
            Assert.Equal("TestUser", versionAuthor.UserName);
            Assert.Equal("test@test.com", versionAuthor.Email);
        }

        public static async Task AssertObjectVersionsListHasNewRecord(HttpClient client)
        {
            var httpResponse = await client.GetAsync("/api/objects/5af2796e32522f798f822a41/versions");
            httpResponse.EnsureSuccessStatusCode();
            var obj = JsonConvert.DeserializeObject<PagedList<VersionInfo>>(await httpResponse.Content.ReadAsStringAsync());

            Assert.Single(obj.Data);
        }
        public static async Task AssertObjectHasNewVersion(HttpClient client)
        {
            var httpResponse = await client.GetAsync("/api/objects/5af2796e32522f798f822a41");
            httpResponse.EnsureSuccessStatusCode();
            var obj = JsonConvert.DeserializeObject<OknObject>(await httpResponse.Content.ReadAsStringAsync());

            AssertHelpers.ValidateVersion(2, obj.Version);
        }
    }
}
