namespace OrderService.Dtos
{
    public class CreateOrderDto
    {
        public int UserId { get; set; }
        public string ProductName { get; set; }
        public decimal Amount { get; set; }
    }
}
