using System;
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
    public class UpdateObjectCommandHandler : CommandHandler<ObjectAggregate, ObjectId, IExecutionResult, UpdateObjectCommand>
    {
        private readonly DbContext _context;

        public UpdateObjectCommandHandler(DbContext context)
        {
            _context = context;
        }

        public override async Task<IExecutionResult> ExecuteCommandAsync(ObjectAggregate aggregate, UpdateObjectCommand command, CancellationToken cancellationToken)
        {
            var filter = Builders<ObjectEntity>.Filter.Where(x => x.ObjectId == command.ObjectId);
            var originalEntity = await _context.Objects
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);

            if (originalEntity == null) return new FailedExecutionResult(new[] { "Object with this id doesn't exist" });
            
            //Put original entity to history table
            await _context.ObjectVersions.InsertOneAsync(originalEntity, cancellationToken: cancellationToken);

            var entity = new ObjectEntity
            {
                Name = command.Name,
                Description = command.Description,
                Longitude = command.Longitude,
                Latitude = command.Latitude,
                Type = command.Type,
                Version = new VersionInfoEntity
                {
                    VersionId = originalEntity.Version?.VersionId + 1 ?? 1,
                    CreateDate = DateTime.UtcNow,
                    Author = new UserInfoEntity
                    {
                        Email = command.Email,
                        UserName = command.Name,
                        Id = command.UserId
                    }
                },
                Events = originalEntity.Events,
                ObjectId = originalEntity.ObjectId,
            };

            await _context.Objects.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);

            return new SuccessExecutionResult();
        }
    }
}