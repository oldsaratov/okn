using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Mongo2Go;
using MongoDB.Driver;
using OKN.Core.Handlers.Queries;
using OKN.Core.Mappings;
using OKN.Core.Models.Queries;
using Xunit;

namespace OKN.Core.Tests
{
    public class ObjectQueryHandleTests : IDisposable
    {
        private static MongoDbRunner _runner;
        private static IMongoDatabase _database;

        public ObjectQueryHandleTests()
        {
            _runner = MongoDbRunner.Start();

            var client = new MongoClient(_runner.ConnectionString);

            _database = client.GetDatabase("okn");
        }

        [Fact]
        public async Task QuerySingleObject()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "1.json");
            _runner.Import("okn", "objects", path, true);

            var config = new MapperConfiguration(cfg => cfg.AddProfile(typeof(MappingProfile)));
            var mapper = config.CreateMapper();

            var handler = new ObjectQueryHandler(mapper, new DbContext(_database));

            var query = new ObjectQuery( "5af2796e32522f798f822a41");

            var result = await handler.ExecuteQueryAsync(query, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(query.ObjectId, result.ObjectId);
        }

        [Fact]
        public async Task QuerySingleObjectLatestVersion()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "2.json");
            _runner.Import("okn", "objects", path, true);

            var config = new MapperConfiguration(cfg => cfg.AddProfile(typeof(MappingProfile)));
            var mapper = config.CreateMapper();

            var handler = new ObjectQueryHandler(mapper, new DbContext(_database));
            
            var query = new ObjectQuery("5af2796e32522f798f822a41");

            var result = await handler.ExecuteQueryAsync(query, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(query.ObjectId, result.ObjectId);
            Assert.Equal(2, result.Version.VersionId);
            Assert.Equal("VERSION2", result.Name);
        }

        [Fact]
        public async Task QuerySingleObjectPreviousVersion()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "2.json");
            _runner.Import("okn", "objects", path, true);

            var config = new MapperConfiguration(cfg => cfg.AddProfile(typeof(MappingProfile)));
            var mapper = config.CreateMapper();

            var handler = new ObjectQueryHandler(mapper, new DbContext(_database));

            var query = new ObjectQuery("5af2796e32522f798f822a41", 1);

            var result = await handler.ExecuteQueryAsync(query, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(query.ObjectId, result.ObjectId);
            Assert.Equal(1, result.Version.VersionId);
            Assert.Equal("VERSION1", result.Name);
        }

        public void Dispose()
        {
            _runner?.Dispose();
        }
    }
}
