namespace OKN.Core.Models.Queries
{
    public class PagedQuery
    {
        internal const int DefaultPerPage = 100;

        public PagedQuery(int? page, int? perPage)
        {
            Page = page ?? 1;
            PerPage = perPage ?? DefaultPerPage;
        }

        public int Page { get; }

        public int PerPage { get; }
    }
}
