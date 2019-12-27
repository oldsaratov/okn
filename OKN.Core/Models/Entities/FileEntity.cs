using MongoDB.Bson.Serialization.Attributes;

namespace OKN.Core.Models.Entities
{
    [BsonIgnoreExtraElements]
    public class FileEntity
    {
        [BsonElement("fileId")]
        public string FileId { get; set; }

        [BsonElement("url")]
        public string Url { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }
    }
}