using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FormsApp.Models
{
    public class Product
    {
        [Display(Name ="Ürün Id")]
        public int ProductId { get; set; }

        [DisplayName("Ürün Adı")]
        [Required(ErrorMessage = "Ürün adı Gerekli bir alan")]
        [StringLength(maximumLength: 100, MinimumLength = 3)]
        public string Name { get; set; } = null!;

        [DisplayName("Fiyat")]
        [Range(0,100000)]
        [Required]
        public decimal? Price { get; set; }

        [DisplayName("Resim")]
        public string? Image { get; set; }

        public bool IsActive { get; set; }

        [Display(Name = "Category")]
        [Required]
        public int? CategoryId { get; set; }
        
    }
}
