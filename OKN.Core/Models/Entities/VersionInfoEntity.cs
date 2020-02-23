using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace OKN.Core.Models.Entities
{
    [BsonIgnoreExtraElements]
    public class VersionInfoEntity
    {
        public VersionInfoEntity()
        {
        }

        public VersionInfoEntity(long versionId, UserInfoEntity author)
        {
            VersionId = versionId;
            Author = author;

            CreateDate = DateTime.UtcNow;
        }

        [BsonElement("versionId")]
        public long VersionId { get; set; }
        
        [BsonElement("createdAt")]
        public BsonDateTime CreateDate { get; set; }
        
        [BsonElement("createdBy")]
        public UserInfoEntity Author { get; set; }
    }
}