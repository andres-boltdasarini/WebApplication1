namespace WebApplication1.Models
{
    public class WealthyCustomerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal AverageOrderAmount { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
    }
}