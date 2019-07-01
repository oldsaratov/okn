using System;
using Mongo2Go;
using MongoDB.Driver;

namespace OKN.Core.Tests
{
    public class BaseRepositoryTests : IDisposable
    {
        protected static MongoDbRunner _runner;
        protected static IMongoDatabase _database;

        public BaseRepositoryTests()
        {
            _runner = MongoDbRunner.Start();
            _database = TestHelpers.GetDefaultDatabase(_runner.ConnectionString);
        }

        public void Dispose()
        {
            _runner?.Dispose();
        }
    }
}
