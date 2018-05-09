using MediatR;

namespace OKN.Core.Models.Commands
{
	public class UpdateObjectCommand : IRequest<OKNObject>
	{
		public string ObjectId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
