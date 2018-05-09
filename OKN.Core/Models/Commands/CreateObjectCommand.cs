using MediatR;

namespace OKN.Core.Models.Commands
{
    public class CreateObjectCommand  : IRequest<OKNObject>
	{
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
