using OKN.Core.Models.Entities;
using MongoDB.Driver;

namespace OKN.Core
{
    public class DbContext
    {
        private readonly IMongoDatabase _database;

        public DbContext(IMongoDatabase database)
        {
            _database = database;
		}

		public virtual IMongoCollection<ObjectEntity> Objects =>
		    _database.GetCollection<ObjectEntity>("objects");

        public virtual IMongoCollection<ObjectEntity> ObjectVersions =>
            _database.GetCollection<ObjectEntity>("objects_versions");
    }
}