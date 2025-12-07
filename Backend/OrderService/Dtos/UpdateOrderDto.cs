namespace OrderService.Dtos
{
    public class UpdateOrderDto
    {
        public string ProductName { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }
}
