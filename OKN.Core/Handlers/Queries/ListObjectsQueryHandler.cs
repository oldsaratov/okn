using System.Collections.Generic;
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
    public class ListObjectsQueryHandler : IQueryHandler<ListObjectsQuery, PagedList<OknObject>>
    {
        private readonly IMapper _mapper;
        private readonly DbContext _context;

        public ListObjectsQueryHandler(IMapper mapper, DbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<PagedList<OknObject>> ExecuteQueryAsync(ListObjectsQuery query, CancellationToken cancellationToken)
        {
            var filter = Builders<ObjectEntity>.Filter.Empty;
            if (query.Types != null)
            {
                filter = Builders<ObjectEntity>.Filter.In(x => x.Type, query.Types);
            }

            var cursor = _context.Objects.Find(filter);
            var count = cursor.CountDocuments(cancellationToken);
            var items = await cursor
                .SortByDescending(x => x.Version.VersionId)
                .Limit(query.PerPage)
                .Skip((query.Page - 1) * query.PerPage)
                .ToListAsync(cancellationToken);

            var model = _mapper.Map<List<ObjectEntity>, List<OknObject>>(items);

            var paged = new PagedList<OknObject>
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