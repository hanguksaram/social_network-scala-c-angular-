namespace API.Helpers
{
    public abstract class PaginationParams
    {
        public const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        public int pageSize = 10;
        public int PageSize
        {
            get { return pageSize;;}
            set { pageSize = (value > MaxPageSize) ? MaxPageSize: value;}
        }
        public int UserId { get; set; }
    }
}