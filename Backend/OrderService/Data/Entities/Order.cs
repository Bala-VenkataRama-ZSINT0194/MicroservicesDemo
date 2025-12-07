namespace OrderService.Data.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }  // Denormalized from User
        public string UserEmail { get; set; } // Denormalized from User
        public string ProductName { get; set; }
        public decimal Amount { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
    }
}
