namespace UserService.Events
{
    public class UserDeletedEvent
    {
        public int UserId { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
