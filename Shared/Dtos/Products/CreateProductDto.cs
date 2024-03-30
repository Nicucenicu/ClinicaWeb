using System.ComponentModel.DataAnnotations;

namespace ClinicaWeb.Shared.Dtos.Products
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string ShortDescription { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
