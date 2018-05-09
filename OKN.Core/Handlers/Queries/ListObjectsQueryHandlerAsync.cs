using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MongoDB.Driver;
using OKN.Core.Models;
using OKN.Core.Models.Entities;
using OKN.Core.Models.Queries;

namespace OKN.Core.Handlers.Queries
{
    public class ListObjectsQueryHandlerAsync : IRequestHandler<ListObjectsQuery, PagedList<OKNObject>>
    {
        private readonly IMapper _mapper;
        private readonly DbContext _context;

        public ListObjectsQueryHandlerAsync(IMapper mapper, DbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<PagedList<OKNObject>> Handle(ListObjectsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var cursor = _context.Objects.Find(Builders<ObjectEntity>.Filter.Empty);
                var count = cursor.Count();
                var items = await cursor
                    .Skip((query.Page - 1) * query.PerPage)
                    .Limit(query.PerPage)
                    .ToListAsync(cancellationToken: cancellationToken);

                var model = _mapper.Map<List<ObjectEntity>, List<OKNObject>>(items);

                var paged = new PagedList<OKNObject>
                {
                    Data = model,
                    Page = query.Page,
                    PerPage = query.PerPage,
                    Total = count
                };

                return paged;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}