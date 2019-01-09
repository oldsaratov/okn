using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;
using OKN.Core.Events;
using OKN.Core.Identity;

namespace OKN.Core.Aggregate
{
    public class ObjectAggregate : AggregateRoot<ObjectAggregate, ObjectId>,
        IEmit<ObjectUpdatedEvent>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ObjectAggregate(ObjectId id)
            : base(id)
        {
        }

        public IExecutionResult Update(string name, string description)
        {
            Emit(new ObjectUpdatedEvent(name, description));

            return ExecutionResult.Success();
        }

        public void Apply(ObjectUpdatedEvent aggregateEvent)
        {
            Name = aggregateEvent.Name;
            Description = aggregateEvent.Description;
        }
    }
}
