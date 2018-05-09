using System;
using System.Threading;
using System.Threading.Tasks;
using OKN.Core.Models;
using AutoMapper;
using MediatR;
using OKN.Core.Models.Commands;

namespace OKN.Core.Handlers.Commands
{
    public class UpdateObjectCommandHandler : IRequestHandler<UpdateObjectCommand, OKNObject>
    {
        IMapper _mapper;
        IMediator _mediator;
        DbContext _context;

        public UpdateObjectCommandHandler(IMapper mapper, DbContext context, IMediator mediator)
        {
            _mapper = mapper;
            _context = context;
            _mediator = mediator;
        }

        public async Task<OKNObject> Handle(UpdateObjectCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}