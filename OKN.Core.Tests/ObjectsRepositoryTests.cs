using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OKN.Core.Models.Queries;
using Xunit;

namespace OKN.Core.Tests
{
    public class ObjectsRepositoryTests : BaseRepositoryTests
    {
        [Fact]
        public async Task GetSingleObject()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "1.json");
            _runner.Import("okn", "objects", path, true);

            var repo = new ObjectsRepository(TestHelpers.GetDefaultMapper(), new DbContext(_database));

            var query = new ObjectQuery("5af2796e32522f798f822a41");

            var result = await repo.GetObject(query, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(query.ObjectId, result.ObjectId);
        }
        
        [Fact]
        public async Task GetObjectThatNotExist()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "1.json");
            _runner.Import("okn", "objects", path, true);
            
            var repo = new ObjectsRepository(null, new DbContext(_database));

            var query = new ObjectQuery("5af2796e32522f798f825642a41");

            await Assert.ThrowsAsync<ObjectNotExistException>(() => repo.GetObject(query, CancellationToken.None));
        }

        [Fact]
        public async Task GetObjectLatestVersion()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "2.json");
            _runner.Import("okn", "objects", path, true);

            var repo = new ObjectsRepository(TestHelpers.GetDefaultMapper(), new DbContext(_database));

            var query = new ObjectQuery("5af2796e32522f798f822a41");

            var result = await repo.GetObject(query, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(query.ObjectId, result.ObjectId);
            Assert.Equal(2, result.Version.VersionId);
            Assert.Equal("VERSION2", result.Name);
        }

        [Fact]
        public async Task GetObjectPreviousVersion()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "2.json");
            _runner.Import("okn", "objects", path, true);

            var repo = new ObjectsRepository(TestHelpers.GetDefaultMapper(), new DbContext(_database));

            var query = new ObjectQuery("5af2796e32522f798f822a41", 1);

            var result = await repo.GetObject(query, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(query.ObjectId, result.ObjectId);
            Assert.Equal(1, result.Version.VersionId);
            Assert.Equal("VERSION1", result.Name);
        }
    }
}
