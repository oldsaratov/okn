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
    public class ObjectQueryHandler : IQueryHandler<ObjectQuery, OknObject>
    {
        private readonly IMapper _mapper;
        private readonly DbContext _context;

        public ObjectQueryHandler(IMapper mapper, DbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<OknObject> ExecuteQueryAsync(ObjectQuery query, CancellationToken cancellationToken)
        {
            var filter = Builders<ObjectEntity>.Filter
                .Where(x => x.ObjectId == query.ObjectId);

            ObjectEntity entity = null;

            if (query.Version.HasValue)
            {
                var versionFilter = Builders<ObjectEntity>.Filter
                    .Where(x => x.Version.VersionId == query.Version);

                var emptyVersionFilter = Builders<ObjectEntity>.Filter
                    .Where(x => x.Version == null);

                filter = Builders<ObjectEntity>.Filter
                    .And(filter, query.Version == 0 ? emptyVersionFilter : versionFilter);

                entity = await _context.ObjectVersions
                    .Find(filter)
                    .SortByDescending(x => x.Version.VersionId)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            //if not found in versions table - try to get last version
            if (entity == null)
            {
                entity = await _context.Objects
                    .Find(filter)
                    .FirstOrDefaultAsync(cancellationToken);

                //check if requested version is last version
                if (entity != null && query.Version.HasValue && query.Version.Value != entity.Version?.VersionId)
                {
                    //current version has different version, so requested version not found
                    return null;
                }
            }

            if (entity != null)
            {
                var model = _mapper.Map<ObjectEntity, OknObject>(entity);
                return model;
            }

            return null;
        }
    }
}