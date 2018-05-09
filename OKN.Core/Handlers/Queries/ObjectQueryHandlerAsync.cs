using System.Threading;
using System.Threading.Tasks;
using OKN.Core.Models;
using OKN.Core.Models.Entities;
using AutoMapper;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using OKN.Core.Models.Queries;

namespace OKN.Core.Handlers.Queries
{
    public class ObjectQueryHandlerAsync : IRequestHandler<ObjectQuery, OKNObject>
    {
        IMapper _mapper;
        DbContext _context;

        public ObjectQueryHandlerAsync(IMapper mapper, DbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<OKNObject> Handle(ObjectQuery query, CancellationToken cancellationToken)
        {
            var file = await _context.Objects.Find(x => x.Id == new ObjectId(query.ObjectId)).FirstAsync();
                
                var model = _mapper.Map<ObjectEntity, OKNObject>(file);
                return model;
        }
    }
}