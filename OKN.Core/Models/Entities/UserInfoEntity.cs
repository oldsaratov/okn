using MongoDB.Bson.Serialization.Attributes;

namespace OKN.Core.Models.Entities
{
    [BsonIgnoreExtraElements]
    public class UserInfoEntity
    {
        public UserInfoEntity()
        {
        }

        public UserInfoEntity(string userId, string userName, string email)
        {
            UserId = userId;
            UserName = userName;
            Email = email;
        }

        [BsonElement("userId")]
        public string UserId { get; private set; }

        [BsonElement("name")]
        public string UserName { get; private set; }

        [BsonElement("email")]
        public string Email { get; private set; }
    }
}