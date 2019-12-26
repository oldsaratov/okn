using MongoDB.Bson.Serialization.Attributes;

namespace OKN.Core.Models.Entities
{
    [BsonIgnoreExtraElements]
    public class UserInfoEntity
    {
        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("name")]
        public string UserName { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }
    }
}