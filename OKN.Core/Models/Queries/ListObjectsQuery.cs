using MediatR;

namespace OKN.Core.Models.Queries
{
    public class ListObjectsQuery : PagedQuery, IRequest<PagedList<OKNObject>>
    {
    }
}
