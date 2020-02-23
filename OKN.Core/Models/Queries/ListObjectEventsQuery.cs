using EventFlow.Queries;

namespace OKN.Core.Models.Queries
{
    public class ListObjectEventsQuery : PagedQuery, IQuery<PagedList<OknObjectEvent>>
    {
        public ListObjectEventsQuery(string objectId, int? page, int? perPage) : base(page, perPage)
        {
            ObjectId = objectId;
        }

        public string ObjectId { get; }
    }
}
