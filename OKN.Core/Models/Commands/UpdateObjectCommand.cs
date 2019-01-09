using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using EventFlow.Core;
using OKN.Core.Aggregate;
using OKN.Core.Identity;

namespace OKN.Core.Models.Commands
{
	public class UpdateObjectCommand : Command<ObjectAggregate, ObjectId, IExecutionResult>
    {
		public string ObjectId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	    public decimal Latitude { get; set; }
	    public decimal Longitude { get; set; }
	    public EObjectType Type { get; set; }
	    
	    public long UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public UpdateObjectCommand(ObjectId aggregateId) : base(aggregateId)
        {
        }

        public UpdateObjectCommand(ObjectId aggregateId, ISourceId sourceId) : base(aggregateId, sourceId)
        {
        }
    }
}
