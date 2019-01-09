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
    public class ListObjectVersionsQueryHandler : IQueryHandler<ListVersionsQuery, PagedList<VersionInfo>>
    {
        private readonly IMapper _mapper;
        private readonly DbContext _context;

        public ListObjectVersionsQueryHandler(IMapper mapper, DbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<PagedList<VersionInfo>> ExecuteQueryAsync(ListVersionsQuery query, CancellationToken cancellationToken)
        {
            var filter = Builders<ObjectEntity>.Filter.Where(x => x.Version != null);

            var cursor = _context.ObjectVersions.Find(filter);
            var count = cursor.CountDocuments(cancellationToken);
            var items = await cursor
                .Skip((query.Page - 1) * query.PerPage)
                .Limit(query.PerPage)
                .Project(x => x.Version)
                .ToListAsync(cancellationToken);

            var model = _mapper.Map<List<VersionInfoEntity>, List<VersionInfo>>(items);

            var paged = new PagedList<VersionInfo>
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