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
using OKN.Core.Models;
using OKN.Core.Models.Queries;
using Xunit;

namespace OKN.Core.Tests
{
    public class ListObjectsQueryHandlerTests : IDisposable
    {
        private static MongoDbRunner _runner;
        private static IMongoDatabase _database;

        public ListObjectsQueryHandlerTests()
        {
            _runner = MongoDbRunner.Start();

            var client = new MongoClient(_runner.ConnectionString);

            _database = client.GetDatabase("okn");
        }

        [Fact]
        public async Task QueryObjectListWithPaging()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3.json");
            _runner.Import("okn", "objects", path, true);

            var config = new MapperConfiguration(cfg => cfg.AddProfile(typeof(MappingProfile)));
            var mapper = config.CreateMapper();

            var handler = new ListObjectsQueryHandler(mapper, new DbContext(_database));

            var query = new ListObjectsQuery
            {
                Page = 2,
                PerPage = 5
            };

            var result = await handler.ExecuteQueryAsync(query, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(4, result.Data?.Count);
        }

        [Fact]
        public async Task QueryObjectListWithoutPaging()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3.json");
            _runner.Import("okn", "objects", path, true);

            var config = new MapperConfiguration(cfg => cfg.AddProfile(typeof(MappingProfile)));
            var mapper = config.CreateMapper();

            var handler = new ListObjectsQueryHandler(mapper, new DbContext(_database));

            var query = new ListObjectsQuery();

            var result = await handler.ExecuteQueryAsync(query, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(9, result.Data?.Count);
            Assert.Equal(1, result.Page);
        }

        [Fact]
        public async Task QueryObjectListWithTypesFilter()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3.json");
            _runner.Import("okn", "objects", path, true);

            var config = new MapperConfiguration(cfg => cfg.AddProfile(typeof(MappingProfile)));
            var mapper = config.CreateMapper();

            var handler = new ListObjectsQueryHandler(mapper, new DbContext(_database));

            var query = new ListObjectsQuery
            {
                Types = new[] { EObjectType.Federal }
            };

            var result = await handler.ExecuteQueryAsync(query, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(3, result.Data?.Count);
            Assert.All(result.Data, x => Assert.Equal(EObjectType.Federal, x.Type));
        }

        public void Dispose()
        {
            _runner?.Dispose();
        }
    }
}
