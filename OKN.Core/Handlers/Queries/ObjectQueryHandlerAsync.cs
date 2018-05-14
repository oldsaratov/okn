using System.Threading;
using System.Threading.Tasks;
using OKN.Core.Models;
using OKN.Core.Models.Entities;
using AutoMapper;
using MediatR;
using MongoDB.Driver;
using OKN.Core.Models.Queries;

namespace OKN.Core.Handlers.Queries
{
    public class ObjectQueryHandlerAsync : IRequestHandler<ObjectQuery, OKNObject>
    {
        private readonly IMapper _mapper;
        private readonly DbContext _context;

        public ObjectQueryHandlerAsync(IMapper mapper, DbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<OKNObject> Handle(ObjectQuery query, CancellationToken cancellationToken)
        {
            var filter = Builders<ObjectEntity>.Filter
                .Where(x => x.ObjectId == query.ObjectId);

            var versionFilter = Builders<ObjectEntity>.Filter
                .Where(x => x.Version.Version == query.Version);
            
            var emptyVersionFilter = Builders<ObjectEntity>.Filter
                .Where(x => x.Version == null);

            if (query.Version.HasValue)
            {
                filter = Builders<ObjectEntity>.Filter
                    .And(filter, query.Version == 0 ?  emptyVersionFilter : versionFilter);
            }
                
            var obj = await _context.Objects
                .Find(filter)
                .SortByDescending(x => x.Version.Version)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (obj != null)
            {
                var model = _mapper.Map<ObjectEntity, OKNObject>(obj);
                return model;
            }
            else
            {
                return null;
            }
        }
    }
}