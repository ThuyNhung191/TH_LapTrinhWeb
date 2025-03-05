using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebBanHang.Models;
using WebBanHang.Repositories;

namespace WebBanHang.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
   
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if(product == null) return NotFound();

            return View(product);

        }
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl)
        {
            // if (ModelState.IsValid)
            // {
            //     if (imageUrl != null)
            //     {
            //         // Lưu hình ảnh đại diện tham khảo bài 02 hàm SaveImage 
            //         product.ImageUrl = await SaveImage(imageUrl);
            //     }

            //     await _productRepository.AddAsync(product);
            //     return RedirectToAction(nameof(Index));
            // }
            // // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập 
            // var categories = await _categoryRepository.GetAllAsync();
            // ViewBag.Categories = new SelectList(categories, "Id", "Name");
            // return View(product);
            if (!ModelState.IsValid)
            {
                var categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                return View(product);
            }

            if (imageUrl != null && imageUrl.Length > 0)
            {
                product.ImageUrl = await SaveImage(imageUrl);
            }
            else
            {
                product.ImageUrl = "~/images/default.jpg"; // ✅ Nếu không có ảnh, dùng ảnh mặc định
            }

            await _productRepository.AddAsync(product);
            return RedirectToAction(nameof(Index));
        }

        // Các actions khác như Display, Update, Delete 

        // Display a list of products 
        // Display a single product 
        

        // Show the product update form 
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name",product.CategoryId);

            return View(product);
        }

        // Process the product update 
        [HttpPost]
        public async Task<IActionResult> Update(int id, Product product,IFormFile imageUrl)
        {
            
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho 
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                var existingProduct = await _productRepository.GetByIdAsync(id); // Giả định có phương thức GetByIdAsync 

                // Giữ nguyên thông tin hình ảnh nếu không có hình mới được 

                if (imageUrl == null)
                {
                    product.ImageUrl = existingProduct.ImageUrl;
                }
                else
                {
                    // Lưu hình ảnh mới 
                    product.ImageUrl = await SaveImage(imageUrl);
                }
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;
                await _productRepository.UpdateAsync(existingProduct);
                return RedirectToAction(nameof(Index));
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }


        // Show the product delete confirmation 
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        
        // Process the product deletion 
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<string> SaveImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return "~/images/default.jpg"; // ✅ Trả về ảnh mặc định nếu không có ảnh

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            string fileName = Path.GetFileName(imageFile.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return $"~/images/{fileName}"; // ✅ Trả về đường dẫn ảnh
        }
    }
}
