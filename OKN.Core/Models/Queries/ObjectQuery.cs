using MediatR;

namespace OKN.Core.Models.Queries
{
    public class ObjectQuery : IRequest<OKNObject>
    {
        public string ObjectId { get; set; }
    }
}