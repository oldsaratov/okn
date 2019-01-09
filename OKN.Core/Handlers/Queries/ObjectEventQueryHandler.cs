using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OKN.Core.Models;
using OKN.Core.Models.Entities;
using AutoMapper;
using EventFlow.Queries;
using MongoDB.Driver;
using OKN.Core.Models.Queries;

namespace OKN.Core.Handlers.Queries
{
    public class ObjectEventQueryHandler : IQueryHandler<ObjectEventQuery, OKNObjectEvent>
    {
        private readonly IMapper _mapper;
        private readonly DbContext _context;

        public ObjectEventQueryHandler(IMapper mapper, DbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<OKNObjectEvent> ExecuteQueryAsync(ObjectEventQuery query, CancellationToken cancellationToken)
        {
            var filter = Builders<ObjectEntity>.Filter.Where(x => x.ObjectId == query.ObjectId);

            var entity = await _context.Objects
                    .Find(filter)
                    .FirstOrDefaultAsync(cancellationToken);
            
            var evnt = entity?.Events.FirstOrDefault(x => x.EventId == query.EventId);

            return evnt != null ? _mapper.Map<ObjectEventEntity, OKNObjectEvent>(evnt) : null;
        }
    }
}