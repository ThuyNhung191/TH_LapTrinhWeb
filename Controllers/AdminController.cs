using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using WebBanHang.Repositories;

namespace WebBanHang.Controllers
{
   
    public class AdminController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ProductDbContext _context;

        public AdminController(IProductRepository productRepository, ICategoryRepository categoryRepository, ProductDbContext context)
   
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _context = context;
        }

        [Authorize(Roles = "Admin")] // Chỉ cho phép Admin truy cập
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if(product == null) return NotFound();

            return View(product);

        }

        [Authorize(Roles = "Admin")] // Chỉ cho phép Admin truy cập
        public async Task<IActionResult> Index(string search="")
        {
                string z = search;
                if(search == null || search == "")
            {
                var applicationDbContext = _context.Products.Include(p => p.Category);
                ViewBag.search = search;
                return View(await applicationDbContext.ToListAsync());
            
            }
            else
            {

                IEnumerable<Product> dssearch = _context.Products.Include(p => p.Category);
                List<Product> ds = new List<Product>();
                foreach(var i in dssearch)
                {
                    string a1 =(i.Description.ToUpper());
                    if (a1.ToUpper().Contains(z.ToUpper()))
                    {
                        ds.Add(i);
                        continue;
                    }
                    string a2 = (i.Name.ToUpper());
                    if (a2.ToUpper().Contains(z.ToUpper()))
                    {
                        ds.Add(i);
                        continue;
                    }
                    string a3 = (i.Category.Name.ToUpper());
                    if (a3.ToUpper().Contains(search.ToUpper()))
                    {
                        ds.Add(i);
                        continue;
                    }
                }
                ViewBag.search = search;
                return View(ds);
            }
             
            
        }

       [Authorize(Roles = "Admin")] // Chỉ cho phép Admin truy cập
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Admin")] // Chỉ cho phép Admin truy cập
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                return View(product);  // Nếu model không hợp lệ, trả về form kèm theo lỗi
            }

            if (imageUrl != null && imageUrl.Length > 0)
            {
                product.ImageUrl = await SaveImage(imageUrl);
            }
            else
            {
                product.ImageUrl = "~/images/default.jpg";  // Nếu không có ảnh, dùng ảnh mặc định
            }

            await _productRepository.AddAsync(product);

            // Thêm ảnh vào bảng ProductImages
            var productImage = new ProductImage
            {
                Url = product.ImageUrl,
                ProductId = product.Id  // Gán ProductId từ sản phẩm vừa được thêm
            };

            _context.ProductImages.Add(productImage);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));  // Sau khi lưu thành công, chuyển hướng về trang danh sách sản phẩm
        }


        // Các actions khác như Display, Update, Delete 

        // Display a list of products 
        // Display a single product 
        

        // Show the product update form 

       [Authorize(Roles = "Admin")] // Chỉ cho phép Admin truy cập
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
        [Authorize(Roles = "Admin")] // Chỉ cho phép Admin truy cập
        public async Task<IActionResult> Update(int id, Product product, IFormFile imageUrl)
        {
            ModelState.Remove("ImageUrl");

            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingProduct = await _productRepository.GetByIdAsync(id);

                // Giữ nguyên thông tin hình ảnh nếu không có hình mới
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

                // Cập nhật bảng ProductImages nếu cần
                var productImage = new ProductImage
                {
                    Url = product.ImageUrl,
                    ProductId = product.Id  // Cập nhật ProductId
                };

                _context.ProductImages.Update(productImage); // Cập nhật thay vì thêm mới
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }


        [Authorize(Roles = "Admin")] // Chỉ cho phép Admin truy cập
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
        
       [Authorize(Roles = "Admin")] // Chỉ cho phép Admin truy cập
        // Process the product deletion 
        [HttpPost, ActionName("DeleteConfirmed")]
        
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }


       [Authorize(Roles = "Admin")] // Chỉ cho phép Admin truy cập
        public async Task<string> SaveImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return "~/images/default.jpg";  // Trả về ảnh mặc định nếu không có ảnh

            // Đảm bảo rằng thư mục tồn tại
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);  // Tạo thư mục nếu nó không tồn tại
            }

            // Lấy tên tệp ảnh
            string fileName = Path.GetFileName(imageFile.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            // Lưu ảnh vào thư mục
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return $"~/images/{fileName}";  // Trả về đường dẫn ảnh đã lưu
        }

    }
}
