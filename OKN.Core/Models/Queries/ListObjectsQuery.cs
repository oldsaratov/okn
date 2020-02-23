using EventFlow.Queries;

namespace OKN.Core.Models.Queries
{
    public class ListObjectsQuery : PagedQuery, IQuery<PagedList<OknObject>>
    {
        public ListObjectsQuery(int? page = null, int? perPage = null) : base(page, perPage)
        {
        }

        public EObjectType[] Types { get; set; }

        public string NameToken { get; set; }
    }
}
