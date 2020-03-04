using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OKN.Core.Models.Entities
{
    [BsonIgnoreExtraElements]
    public class LinkEntity
    {
        [BsonElement("type")]
        public ELinkTypes Type { get; set; }

        [BsonElement("url")]
        public string Url { get; set; }
    }
}