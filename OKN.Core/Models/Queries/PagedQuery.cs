namespace OKN.Core.Models.Queries
{
    public class PagedQuery
    {
        public PagedQuery()
        {
            Page = 1;
            PerPage = 10;
        }

        public int Page { get; set; }
		public int PerPage { get; set; }
    }
}
