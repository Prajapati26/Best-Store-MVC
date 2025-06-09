using BestStoreMVC.Models;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                TempData["Message"] = "Please login to continue.";
                return RedirectToAction("Login", "Auth");
            }
            var product = context.products.OrderByDescending(p => p.Id).ToList();
            return View(product);
        }

        public IActionResult Create()
        {
            return View();
        }





        [HttpPost]
        public IActionResult Create(ProductAdd productadd)
        {
            if (productadd.ImageFile == null || productadd.ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }

            if (!ModelState.IsValid)
            {
                return View(productadd);
            }

            // Generate unique filename with timestamp + extension
            string newfileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(productadd.ImageFile.FileName);

            // Define folder path in wwwroot/products
            string imageDirectory = Path.Combine(environment.WebRootPath, "products");

            // Ensure directory exists
            if (!Directory.Exists(imageDirectory))
                Directory.CreateDirectory(imageDirectory);

            // Full path where image will be saved
            string imageFullPath = Path.Combine(imageDirectory, newfileName);

            // Save the image to server
            using (var stream = new FileStream(imageFullPath, FileMode.Create))
            {
                productadd.ImageFile.CopyTo(stream);
            }

            // Create product entity
            Product product = new Product()
            {
                Name = productadd.Name,
                Brand = productadd.Brand,
                Category = productadd.Category,
                Price = productadd.Price,
                Description = productadd.Description,
                ImageFileName = newfileName,
                CreateAt = DateTime.Now,
            };

            // Add and save product in database
            context.products.Add(product);
            context.SaveChanges();

            // Redirect to product listing page after success
            return RedirectToAction("Index", "Products");
        }
      



        public IActionResult Edit(int Id)
        {
            var product = context.products.Find(Id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            var productadd = new ProductAdd()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,

            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreateAt"] = product.CreateAt.ToString("yyyy-MM-dd");


            return View(productadd);

        }




        [HttpPost]
        public IActionResult Edit(int Id, ProductAdd productes)  // assuming you're using a view model with ImageFile
        {
            var product = context.products.Find(Id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreateAt"] = product.CreateAt.ToString("yyyy-MM-dd");
                return View(productes);
            }

            // ✅ If a new image is uploaded, save it and update file name
            if (productes.ImageFile != null)
            {
                string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                                     Path.GetExtension(productes.ImageFile.FileName);
                string imagePath = Path.Combine(environment.WebRootPath, "products", newFileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    productes.ImageFile.CopyTo(stream);
                }

                product.ImageFileName = newFileName;
            }

            // ✅ Update other properties
            product.Name = productes.Name;
            product.Brand = productes.Brand;
            product.Category = productes.Category;
            product.Price = productes.Price;
            product.Description = productes.Description;

            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var product = context.products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            // Delete the image file from wwwroot/products folder
            string imageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
            System.IO.File.Delete(imageFullPath);

            // Remove the product from the database
            context.products.Remove(product);
            context.SaveChanges(true);

            return RedirectToAction("Index", "Products");
        }

    }

}