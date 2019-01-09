using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using MongoDB.Driver;
using OKN.Core.Aggregate;
using OKN.Core.Identity;
using OKN.Core.Models.Commands;
using OKN.Core.Models.Entities;

namespace OKN.Core.Handlers.Commands
{
    public class CreateObjectEventCommandHandler : CommandHandler<ObjectAggregate, ObjectId, IExecutionResult, CreateObjectEventCommand>
    {
        private readonly DbContext _context;

        public CreateObjectEventCommandHandler(DbContext context)
        {
            _context = context;
        }

        public override async Task<IExecutionResult> ExecuteCommandAsync(ObjectAggregate aggregate, CreateObjectEventCommand command, CancellationToken cancellationToken)
        {
            var filter = Builders<ObjectEntity>.Filter.Where(x => x.ObjectId == command.ObjectId);
            var originalEntity = await _context.Objects
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);

            if (originalEntity == null) return new FailedExecutionResult(new[] { "Object with this id doesn't exist" });

            var links = command.Links?.Select(x => new LinkEntity(x.Url, x.Description)).ToList();
            var images = command.Images?.Select(x => new ImageLinkEntity(x.Url, x.Description)).ToList();

            var entity = new ObjectEventEntity
            {
                EventId = command.EventId,
                Name = command.Name,
                Description = command.Description,
                Links = links,
                Images = images,
                OccuredAt = command.OccuredAt,
                Author = new UserInfoEntity
                {
                    Email = command.Email,
                    UserName = command.Name,
                    Id = command.UserId
                }
            };

            if (originalEntity.Events == null)
                originalEntity.Events = new List<ObjectEventEntity>();

            originalEntity.Events.Add(entity);

            var result = await _context.Objects.ReplaceOneAsync(filter, originalEntity, cancellationToken: cancellationToken);
            
            return new SuccessExecutionResult();
        }
    }
}