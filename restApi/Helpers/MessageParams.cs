namespace API.Helpers
{
    public class MessageParams: PaginationParams
    {
        public string MessageContainer { get; set; } = "Unread";
    }
}