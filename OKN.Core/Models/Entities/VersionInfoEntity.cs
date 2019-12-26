using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OKN.Core.Models.Entities
{
    [BsonIgnoreExtraElements]
    public class VersionInfoEntity
    {
        [BsonElement("versionId")]
        public long VersionId { get; set; }
        
        [BsonElement("createdAt")]
        public BsonDateTime CreateDate { get; set; }
        
        [BsonElement("createdBy")]
        public UserInfoEntity Author { get; set; }
    }
}