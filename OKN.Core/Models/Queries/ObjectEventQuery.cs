using EventFlow.Queries;

namespace OKN.Core.Models.Queries
{
    public class ObjectEventQuery : IQuery<OknObjectEvent>
    {
        public ObjectEventQuery(string objectId, string eventId)
        {
            ObjectId = objectId;
            EventId = eventId;
        }

        public string ObjectId { get; }

        public string EventId { get; }
    }
}