using EventFlow.Queries;
using Mongo2Go;
using MongoDB.Driver;
using Moq;
using OKN.Core.Aggregate;
using OKN.Core.Handlers.Commands;
using OKN.Core.Identity;
using OKN.Core.Models;
using OKN.Core.Models.Commands;
using OKN.Core.Models.Entities;
using OKN.Core.Models.Queries;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OKN.Core.Tests
{
    public class UpdateObjectCommandHandlerTests : IDisposable
    {
        private static MongoDbRunner _runner;
        private static IMongoDatabase _database;

        public UpdateObjectCommandHandlerTests()
        {
            _runner = MongoDbRunner.Start();

            var client = new MongoClient(_runner.ConnectionString);

            _database = client.GetDatabase("okn");
        }

        [Fact]
        public async Task UpdateObjectThatNotExist()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "1.json");
            _runner.Import("okn", "objects", path, true);
            
            var handler = new UpdateObjectCommandHandler(new DbContext(_database));
            
            var command = new UpdateObjectCommand(new ObjectId("5af2796e32522f798f822a41"));

            var result = await handler.ExecuteCommandAsync(new ObjectAggregate(new ObjectId("5af2796e32522f798f822a41")), command, CancellationToken.None);
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateObjectThatExist()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "1.json");
            _runner.Import("okn", "objects", path, true);
            
            var handler = new UpdateObjectCommandHandler(new DbContext(_database));

            var command = new UpdateObjectCommand(new ObjectId("5af2796e32522f798f822a41"))
            {
                ObjectId = "5af2796e32522f798f822a41",
                Name = "TEST1"
            };

            var result = await handler.ExecuteCommandAsync(new ObjectAggregate(new ObjectId("5af2796e32522f798f822a41")), command, CancellationToken.None);
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);

            var entities = _database.GetCollection<ObjectEntity>("objects").Find(Builders<ObjectEntity>.Filter
                .Where(x => x.ObjectId == command.ObjectId)).ToList();

            Assert.Single(entities);
            Assert.Equal(command.Name, entities[0].Name);

            var versions = _database.GetCollection<ObjectEntity>("objects_versions").Find(Builders<ObjectEntity>.Filter
                .Where(x => x.ObjectId == command.ObjectId)).ToList();
            Assert.Single(versions);
            Assert.Equal("TEST", versions[0].Name);
        }

        public void Dispose()
        {
            _runner?.Dispose();
        }
    }
}
