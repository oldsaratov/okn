using MediatR;

namespace OKN.Core.Models.Commands
{
	public class UpdateObjectCommand : IRequest
	{
		public string ObjectId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	    public decimal Latitude { get; set; }
	    public decimal Longitude { get; set; }
	    public EObjectType Type { get; set; }
	    
	    public string Author { get; set; }
	}
}
