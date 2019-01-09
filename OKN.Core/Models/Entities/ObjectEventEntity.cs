using System.Collections.Generic;
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
        
        [BsonElement("links")]
        public List<LinkEntity> Links { get; set; }

        [BsonElement("images")]
        public List<ImageLinkEntity> Images { get; set; }
        
        [BsonElement("occuredAt")]
        public BsonDateTime OccuredAt { get; set; }
        
        [BsonElement("createdBy")]
        public UserInfoEntity Author { get; set; }

        public ObjectEventEntity() { }
    }

    public class LinkEntity
    {
        public LinkEntity(string url, string description)
        {
            Url = url;
            Description = description;
        }

        [BsonElement("description")]
        public string Description { get; set; }
        
        [BsonElement("url")]
        public string Url { get; set; }
    }

    public class ImageLinkEntity : LinkEntity
    {
        public ImageLinkEntity(string url, string description) : base(url, description)
        {
        }
    }
}