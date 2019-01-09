using EventFlow.Queries;

namespace OKN.Core.Models.Queries
{
    public class ListVersionsQuery : PagedQuery, IQuery<PagedList<VersionInfo>>
    {
        public string ObjectId { get; set; }
    }
}
