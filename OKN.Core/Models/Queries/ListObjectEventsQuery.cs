using EventFlow.Queries;

namespace OKN.Core.Models.Queries
{
    public class ListObjectEventsQuery : PagedQuery, IQuery<PagedList<OKNObjectEvent>>
    {
        public string ObjectId { get; set; }
    }
}
