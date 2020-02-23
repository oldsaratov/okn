using EventFlow.Queries;

namespace OKN.Core.Models.Queries
{
    public class ObjectQuery : IQuery<OknObject>
    {
        public ObjectQuery(string objectId, long? version = null)
        {
            ObjectId = objectId;
            Version = version;
        }

        public string ObjectId { get; }

        public long? Version { get; }
    }
}