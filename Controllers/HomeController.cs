using FormsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace FormsApp.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {

        }

        [HttpGet]
        public IActionResult Index(string searchString, string category)
        {
            var products = Repository.Products;

            if (!string.IsNullOrEmpty(searchString))
            {
                ViewBag.SearchString = searchString;
                products = products.Where(p => p.Name.ToLower().Contains(searchString.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(category) && category != "0")
            {
                products = products.Where(p => p.CategoryId == Convert.ToInt32(category)).ToList();
            }

            //ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name", category);

            var model = new ProductViewModel()
            {
                Products = products,
                Categories = Repository.Categories,
                SelectedCategory = category
            };

            return View(model);
        }


        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            if (imageFile != null)
            {

                string UploadedFileExtension = Path.GetExtension(imageFile.FileName);
                string GeneratedRandomFileName = Guid.NewGuid().ToString();
                string GeneratedFullFileName = $"{GeneratedRandomFileName}{UploadedFileExtension}";
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", GeneratedFullFileName);

                string[] AllowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

                if (!AllowedExtensions.Contains(UploadedFileExtension)) //yüklediğimiz dosya uzantısı bizim istediğimiz dosya uzantılarıyla eşleşmiyorsa çalışır 
                {
                    ModelState.AddModelError(string.Empty, "Geçerli bir dosya uzantılı resim seçiniz"); //<div asp-validation-summary="All"> </div> sayesinde view sayfasında hatayı yazacak. Ve aşağıdaki ModelState.IsValid false olacak diğer tüm bilgiler doğru girilmiş olmasına rağmen.
                }



                if (ModelState.IsValid)
                {
                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream); //bizim yüklediğimiz image dosyasının yukarıda create edilen FileStream'e  kopyalanması işlemi.
                    }

                    product.Image = GeneratedFullFileName; //Home/Index'e döndüğünde dosya'nın src'sini bulsun diye yazdık.
                    product.ProductId = Repository.Products.Count + 1;

                    Repository.CreateProduct(product);
                    return RedirectToAction("Index");
                }
            }


            ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
            return View();
        }


        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = Repository.Products.FirstOrDefault(p => p.ProductId == id);

            if (entity == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
            return View(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    string[] AllowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

                    string UploadedFileExtension = Path.GetExtension(imageFile.FileName);
                    string GeneratedRandomFileName = Guid.NewGuid().ToString();
                    string GeneratedFullFileName = $"{GeneratedRandomFileName}{UploadedFileExtension}";

                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", GeneratedFullFileName);

                    if (!AllowedExtensions.Contains(UploadedFileExtension)) //yüklediğimiz dosya uzantısı bizim istediğimiz dosya uzantılarıyla eşleşmiyorsa çalışır 
                    {
                        ModelState.AddModelError(string.Empty, "Geçerli bir dosya uzantılı resim seçiniz"); //<div asp-validation-summary="All"> </div> sayesinde view sayfasında hatayı yazacak. Ve aşağıdaki ModelState.IsValid false olacak diğer tüm bilgiler doğru girilmiş olmasına rağmen.
                    }

                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    product.Image = GeneratedFullFileName;
                }
                Repository.EditProduct(product);
                return RedirectToAction("Index");

            }

            ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
            return View();

        }


        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product? entity = Repository.Products.FirstOrDefault(p => p.ProductId == id);

            if (entity == null)
            {
                return NotFound();
            }

            return View("DeleteConfirm", entity);
        }

        [HttpPost]
        public IActionResult Delete(int id, int ProductId)
        {
            if (id != ProductId)
            {
                return NotFound();
            }

            Product? entity = Repository.Products.FirstOrDefault(p => p.ProductId == ProductId);

            if (entity == null)
            {
                return NotFound();
            }

            Repository.DeleteProduct(entity);
            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public IActionResult EditProducts(List<Product> Products)
        {
            foreach (var product in Products)
            {
                Repository.EditIsActive(product);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}