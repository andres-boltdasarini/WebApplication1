namespace WebApplication1.Models
{
    public class CustomerLastOrderDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public decimal LastOrderAmount { get; set; }
    }
}