using EventFlow.Queries;

namespace OKN.Core.Models.Queries
{
    public class ListObjectsQuery : PagedQuery, IQuery<PagedList<OknObject>>
    {
        public EObjectType[] Types { get; set; }
    }
}
