using EventFlow.Queries;

namespace OKN.Core.Models.Queries
{
    public class ListVersionsQuery : PagedQuery, IQuery<PagedList<VersionInfo>>
    {
        public ListVersionsQuery(string objectId, int? page, int? perPage) : base(page, perPage)
        {
            ObjectId = objectId;
        }

        public string ObjectId { get; }
    }
}
