namespace OrderService.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string ProductName { get; set; }
        public decimal Amount { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
    }
}
