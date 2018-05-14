using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OKN.Core.Models.Commands;
using OKN.Core.Models.Entities;
using OKN.Core.Models.Queries;

namespace OKN.Core.Handlers.Commands
{
    public class UpdateObjectCommandHandler : IRequestHandler<UpdateObjectCommand>
    {
        private readonly IMediator _mediator;
        private readonly DbContext _context;

        public UpdateObjectCommandHandler(DbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task Handle(UpdateObjectCommand request, CancellationToken cancellationToken)
        {
            var model = await _mediator.Send(new ObjectQuery()
            {
                ObjectId = request.ObjectId
            }, cancellationToken);

            if (model == null) return;

            var entity = new ObjectEntity()
            {
                ObjectId = request.ObjectId,
                Name = request.Name,
                Description = request.Description,
                Longitude = request.Longitude,
                Latitude = request.Latitude,
                Type = request.Type,
                Version = new VersionInfoEntity()
                {
                    Version = model.Version?.Version + 1 ?? 1,
                    CreateDate = DateTime.UtcNow,
                    Author = request.Author
                }
            };
            
            await _context.Objects.InsertOneAsync(entity, cancellationToken: cancellationToken);
        }
    }
}