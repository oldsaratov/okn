using OKN.Core.Models.Entities;
using MongoDB.Driver;

namespace OKN.Core
{
    public class DbContext
	{
		public DbContext(string connectionString)
		{
            var url = new MongoUrl(connectionString);
            var settings = new MongoClientSettings();
          
			var client = new MongoClient(url);
			Database = client.GetDatabase(url.DatabaseName);
		}

		private IMongoDatabase Database { get; }

		public IMongoCollection<ObjectEntity> Objects => 
            Database.GetCollection<ObjectEntity>("objects");
    }
}