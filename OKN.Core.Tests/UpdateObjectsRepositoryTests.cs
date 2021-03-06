using AutoMapper;
using OKN.Core.Mappings;
using OKN.Core.Models.Commands;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Mongo2Go;
using OKN.Core.Exceptions;
using OKN.Core.Repositories;
using Xunit;

namespace OKN.Core.Tests
{
    public class UpdateObjectsRepositoryTests
    {
        [Fact]
        public async Task UpdateObjectThatNotExist()
        {
            var runner = MongoDbRunner.Start();
            var database = TestHelpers.GetDefaultDatabase(runner.ConnectionString);

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "1.json");
            runner.Import("okn", "objects", path, true);
            
            var repo = new ObjectsRepository(null, new DbContext(database));
            
            var command = new UpdateObjectCommand("5af27196e32522f798f822a41");

            await Assert.ThrowsAsync<ObjectNotExistException>(() => repo.UpdateObject(command, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateObjectThatExist()
        {
            var runner = MongoDbRunner.Start();
            var database = TestHelpers.GetDefaultDatabase(runner.ConnectionString);

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "1.json");
            runner.Import("okn", "objects", path, true);
            
            var config = new MapperConfiguration(cfg => cfg.AddProfile(typeof(MappingProfile)));
            var mapper = config.CreateMapper();
            
            var repo = new ObjectsRepository(mapper, new DbContext(database));

            var command = new UpdateObjectCommand("5af2796e32522f798f822a41")
            {
                Name = "TEST1"
            };

            await repo.UpdateObject(command, CancellationToken.None);
;
           // var entities = _database.GetCollection<ObjectEntity>("objects").Find(Builders<ObjectEntity>.Filter
           //     .Where(x => x.ObjectId == command.ObjectId)).ToList();

           // Assert.Single(entities);
           // Assert.Equal(command.Name, entities[0].Name);

           // var versions = _database.GetCollection<ObjectEntity>("objects_versions").Find(Builders<ObjectEntity>.Filter
           //     .Where(x => x.ObjectId == command.ObjectId)).ToList();
          //  Assert.Single(versions);
          //  Assert.Equal("TEST", versions[0].Name);
        }
    }
}
