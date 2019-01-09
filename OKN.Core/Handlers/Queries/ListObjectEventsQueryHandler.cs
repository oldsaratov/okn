using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventFlow.Queries;
using MongoDB.Driver;
using OKN.Core.Models;
using OKN.Core.Models.Entities;
using OKN.Core.Models.Queries;

namespace OKN.Core.Handlers.Queries
{
    public class ListObjectEventsQueryHandler : IQueryHandler<ListObjectEventsQuery, PagedList<OKNObjectEvent>>
    {
        private readonly IMapper _mapper;
        private readonly DbContext _context;

        public ListObjectEventsQueryHandler(IMapper mapper, DbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<PagedList<OKNObjectEvent>> ExecuteQueryAsync(ListObjectEventsQuery query, CancellationToken cancellationToken)
        {
            var filter = Builders<ObjectEntity>.Filter.Where(x => x.ObjectId == query.ObjectId);

            var objectEntity = await _context.Objects.Find(filter).SingleOrDefaultAsync(cancellationToken);
            if (objectEntity?.Events == null) return null;
            
            var count = objectEntity.Events.Count;
            var items = objectEntity.Events.AsQueryable()
                .Skip((query.Page - 1) * query.PerPage)
                .Take(query.PerPage).ToList();

            var model = _mapper.Map<List<ObjectEventEntity>, List<OKNObjectEvent>>(items);

            var paged = new PagedList<OKNObjectEvent>
            {
                Data = model,
                Page = query.Page,
                PerPage = query.PerPage,
                Total = count
            };

            return paged;
        }
    }
}