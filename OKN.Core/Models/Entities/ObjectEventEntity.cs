using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OKN.Core.Models.Entities
{
    [BsonIgnoreExtraElements]
    public class ObjectEventEntity
    {   
        [BsonElement("eventId")]
        public string EventId { get; set; }
        
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }
        
        [BsonElement("occuredAt")]
        public BsonDateTime OccuredAt { get; set; }
        
        [BsonElement("createdBy")]
        public UserInfoEntity Author { get; set; }

        public ObjectEventEntity() { }
    }
}