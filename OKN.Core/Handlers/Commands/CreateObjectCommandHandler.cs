using System;
using System.Threading;
using System.Threading.Tasks;
using OKN.Core.Models.Commands;
using MediatR;
using OKN.Core.Models;

namespace OKN.Core.Handlers.Commands
{
    public class CreateObjectCommandHandler : IRequestHandler<CreateObjectCommand, OKNObject>
    {
		DbContext _context;

		public CreateObjectCommandHandler(DbContext context)
        {
            _context = context;
        }

        public async Task<OKNObject> Handle(CreateObjectCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}