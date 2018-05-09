using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OKN.Core.Models.Entities
{
    [BsonIgnoreExtraElements]
    public class ObjectEntity
    {
        public ObjectId Id { get; set; }
        
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }
        
        [BsonElement("latitude")]
        public decimal Latitude { get; set; }
        
        [BsonElement("longitude")]
        public decimal Longitude { get; set; }
        
        [BsonElement("type")]
        public int Type { get; set; }

        public ObjectEntity() { }
    }
}