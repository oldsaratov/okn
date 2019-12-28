using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Mongo2Go;
using OKN.Core.Models;
using OKN.Core.Models.Queries;
using OKN.Core.Repositories;
using Xunit;

namespace OKN.Core.Tests
{
    public class ObjectsListsRepositoryTests
    {
        [Fact]
        public async Task QueryObjectListWithPaging()
        {
            var runner = MongoDbRunner.Start();
            var database = TestHelpers.GetDefaultDatabase(runner.ConnectionString);

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3.json");
            runner.Import("okn", "objects", path, true);

            var handler = new ObjectsRepository(TestHelpers.GetDefaultMapper(), new DbContext(database));

            var query = new ListObjectsQuery(page: 2, perPage: 5);

            var result = await handler.GetObjects(query, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(4, result.Data?.Count);

            runner.Dispose();
        }

        [Fact]
        public async Task QueryObjectListWithoutPaging()
        {
            var runner = MongoDbRunner.Start();
            var database = TestHelpers.GetDefaultDatabase(runner.ConnectionString);

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3.json");
            runner.Import("okn", "objects", path, true);

            var handler = new ObjectsRepository(TestHelpers.GetDefaultMapper(), new DbContext(database));

            var query = new ListObjectsQuery();

            var result = await handler.GetObjects(query, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(9, result.Data?.Count);
            Assert.Equal(1, result.Page);

            runner.Dispose();
        }

        [Fact]
        public async Task QueryObjectListWithTypesFilter()
        {
            var runner = MongoDbRunner.Start();
            var database = TestHelpers.GetDefaultDatabase(runner.ConnectionString);

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3.json");
            runner.Import("okn", "objects", path, true);

            var handler = new ObjectsRepository(TestHelpers.GetDefaultMapper(), new DbContext(database));

            var query = new ListObjectsQuery
            {
                Types = new[] { EObjectType.Federal }
            };

            var result = await handler.GetObjects(query, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(3, result.Data?.Count);
            Assert.All(result.Data, x => Assert.Equal(EObjectType.Federal, x.Type));

            runner.Dispose();
        }
    }
}
