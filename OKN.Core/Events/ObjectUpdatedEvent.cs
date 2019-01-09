using EventFlow.Aggregates;
using EventFlow.EventStores;
using OKN.Core.Aggregate;
using OKN.Core.Identity;

namespace OKN.Core.Events
{
    [EventVersion("update-object", 1)]
    public class ObjectUpdatedEvent : AggregateEvent<ObjectAggregate, ObjectId>
    {
        public ObjectUpdatedEvent(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }

        public string Description { get; }

    }
}