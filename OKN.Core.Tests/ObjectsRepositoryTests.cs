using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Mongo2Go;
using OKN.Core.Exceptions;
using OKN.Core.Models.Queries;
using OKN.Core.Repositories;
using Xunit;

namespace OKN.Core.Tests
{
    public class ObjectsRepositoryTests
    {
        [Fact]
        public async Task GetSingleObject()
        {
            var runner = MongoDbRunner.Start();
            var database = TestHelpers.GetDefaultDatabase(runner.ConnectionString);

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "1.json");
            runner.Import("okn", "objects", path, true);

            var repo = new ObjectsRepository(TestHelpers.GetDefaultMapper(), new DbContext(database));

            var query = new ObjectQuery("5af2796e32522f798f822a41");

            var result = await repo.GetObject(query, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(query.ObjectId, result.ObjectId);

            runner.Dispose();
        }
        
        [Fact]
        public async Task GetObjectThatNotExist()
        {
            var runner = MongoDbRunner.Start();
            var database = TestHelpers.GetDefaultDatabase(runner.ConnectionString);

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "1.json");
            runner.Import("okn", "objects", path, true);
            
            var repo = new ObjectsRepository(null, new DbContext(database));

            var query = new ObjectQuery("5af2796e32522f798f825642a41");

            await Assert.ThrowsAsync<ObjectNotExistException>(() => repo.GetObject(query, CancellationToken.None));

            runner.Dispose();
        }

        [Fact]
        public async Task GetObjectLatestVersion()
        {
            var runner = MongoDbRunner.Start();
            var database = TestHelpers.GetDefaultDatabase(runner.ConnectionString);

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "2.json");
            runner.Import("okn", "objects", path, true);

            var repo = new ObjectsRepository(TestHelpers.GetDefaultMapper(), new DbContext(database));

            var query = new ObjectQuery("5af2796e32522f798f822a41");

            var result = await repo.GetObject(query, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(query.ObjectId, result.ObjectId);
            Assert.Equal(2, result.Version.VersionId);
            Assert.Equal("VERSION2", result.Name);

            runner.Dispose();
        }

        [Fact]
        public async Task GetObjectPreviousVersion()
        {
            var runner = MongoDbRunner.Start();
            var database = TestHelpers.GetDefaultDatabase(runner.ConnectionString);

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "2.json");
            runner.Import("okn", "objects", path, true);

            var repo = new ObjectsRepository(TestHelpers.GetDefaultMapper(), new DbContext(database));

            var query = new ObjectQuery("5af2796e32522f798f822a41", 1);

            var result = await repo.GetObject(query, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(query.ObjectId, result.ObjectId);
            Assert.Equal(1, result.Version.VersionId);
            Assert.Equal("VERSION1", result.Name);

            runner.Dispose();
        }
    }
}
