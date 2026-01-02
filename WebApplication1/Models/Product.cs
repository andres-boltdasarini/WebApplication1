using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        // Связь с категорией
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // Связь с изображениями
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    }
}