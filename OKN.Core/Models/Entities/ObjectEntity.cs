using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OKN.Core.Models.Entities
{
    [BsonIgnoreExtraElements]
    public class ObjectEntity
    {   
        [BsonElement("objectId")]
        public string ObjectId { get; set; }
        
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }
        
        [BsonElement("latitude")]
        public decimal Latitude { get; set; }
        
        [BsonElement("longitude")]
        public decimal Longitude { get; set; }
        
        [BsonElement("type")]
        public EObjectType Type { get; set; }
        
        [BsonElement("versionInfo")]
        public VersionInfoEntity Version { get; set; }

        public ObjectEntity() { }
    }

    public class VersionInfoEntity
    {
        [BsonElement("version")]
        public long Version { get; set; }
        
        [BsonElement("createdAt")]
        public BsonDateTime CreateDate { get; set; }
        
        [BsonElement("createdBy")]
        public string Author { get; set; }
    }
}