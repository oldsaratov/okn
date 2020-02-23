using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Mongo2Go;
using OKN.Core.Models.Queries;
using OKN.Core.Repositories;
using Xunit;

namespace OKN.Core.Tests
{
    public class ObjectsRepositoryTests : IDisposable
    {
        private readonly ObjectsRepository repo;
        private readonly MongoDbRunner runner;

        public ObjectsRepositoryTests()
        {
            runner = MongoDbRunner.Start();
            var database = TestHelpers.GetDefaultDatabase(runner.ConnectionString);

            repo = new ObjectsRepository(TestHelpers.GetDefaultMapper(), new DbContext(database));
        }

        [Fact]
        public async Task GetSingleObject()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "1.json");
            runner.Import("okn", "objects", path, true);

            var query = new ObjectQuery("5af2796e32522f798f822a41");

            var result = await repo.GetObject(query, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(query.ObjectId, result.ObjectId);
        }

        [Fact]
        public async Task GetObjectThatNotExist()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "1.json");
            runner.Import("okn", "objects", path, true);

            var query = new ObjectQuery("5af2796e32522f798f825642a41");

            var obj = await repo.GetObject(query, CancellationToken.None);

            Assert.Null(obj);
        }

        [Fact]
        public async Task GetObjectLatestVersion()
        {
            var path1 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "single_record.json");
            runner.Import("okn", "objects", path1, true);
            var path2 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "record_versions.json");
            runner.Import("okn", "objects_versions", path2, true);

            var query = new ObjectQuery("5af2796e32522f798f822a41");

            var result = await repo.GetObject(query, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(query.ObjectId, result.ObjectId);
            Assert.Equal(3, result.Version.VersionId);
            Assert.Equal("VESION3", result.Name);
        }

        [Fact]
        public async Task GetObjectPreviousVersion()
        {
            var path1 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "single_record.json");
            runner.Import("okn", "objects", path1, true);
            var path2 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "record_versions.json");
            runner.Import("okn", "objects_versions", path2, true);

            var query = new ObjectQuery("5af2796e32522f798f822a41", version: 1);

            var result = await repo.GetObject(query, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(query.ObjectId, result.ObjectId);
            Assert.Equal(1, result.Version.VersionId);
            Assert.Equal("VERSION1", result.Name);
        }

        public void Dispose()
        {
            runner.Dispose();
        }
    }
}
