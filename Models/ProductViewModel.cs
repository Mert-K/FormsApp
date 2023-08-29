using Microsoft.AspNetCore.Mvc.Rendering;

namespace FormsApp.Models
{
    public class ProductViewModel
    {
        public List<Product> Products { get; set; } = null!; //null değil demek, uyarıyı kaldırmak için.

        public List<Category> Categories { get; set; } = null!;

        public string? SelectedCategory { get; set; }
    }
}
