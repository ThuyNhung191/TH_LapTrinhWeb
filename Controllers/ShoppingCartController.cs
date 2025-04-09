using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Identity.Data;
using WebBanHang.Extensions;
using WebBanHang.Models;
using WebBanHang.Repositories;
namespace WebBanHang.Areas.Identity.Data;

//namespace WebBanHang.Controllers;

public class ShoppingCartController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ProductDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
   
    public ShoppingCartController(IProductRepository productRepository, ProductDbContext context, UserManager<ApplicationUser> userManager)
    {
        _productRepository = productRepository;
        _context = context;
        _userManager = userManager;
    }

   [HttpGet]
    public IActionResult Checkout()
    {
        // Lấy giỏ hàng từ session
        var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
        if (cart == null || !cart.Items.Any())
        {
            return RedirectToAction("Index");
        }

        // Tạo đối tượng Order từ giỏ hàng
        var order = new Order
        {
            TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity),
            OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price,
                Product = new Product
                {
                    ImageUrl = i.ImageUrl, // Đảm bảo thuộc tính này tồn tại trong CartItem
                    Name = i.Name
                }
            }).ToList()
        };

        // Nếu người dùng đã đăng nhập, tự động điền thông tin
        if (User.Identity.IsAuthenticated)
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user != null)
            {
                order.HoTen = user.FullName; // Gán tên đầy đủ từ tài khoản
                order.DienThoai = user.PhoneNumber; // Gán số điện thoại từ tài khoản (nếu có)
                order.ShippingAddress = user.Address; // Gán địa chỉ từ tài khoản (nếu có)
            }
        }

        return View(order);
    }
   

    [HttpPost]
    public async Task<IActionResult> Checkout(Order order)
    {
        // Kiểm tra trạng thái đăng nhập
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account");
        }

        // Lấy giỏ hàng từ session
        var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
        if (cart == null || !cart.Items.Any())
        {
            return RedirectToAction("Index");
        }

        if (!ModelState.IsValid)
        {
            return View(order);
        }

        // Lấy thông tin người dùng
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Gán thông tin đơn hàng
        order.UserId = user.Id;
        order.OrderDate = DateTime.UtcNow;
        order.TotalPrice = cart.Items.Sum(i => i.Quantity * i.Price); // Tính tổng giá trị đơn hàng
        order.OrderDetails = cart.Items.Select(i => new OrderDetail
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            Price = i.Price, // Đảm bảo giá được lưu đúng
            ReturnUrl = string.Empty // Gán giá trị mặc định cho ReturnUrl
            
        }).ToList();

        // Lưu đơn hàng vào cơ sở dữ liệu
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Xóa giỏ hàng khỏi session
        HttpContext.Session.Remove("Cart");

        // Chuyển hướng đến trang xác nhận đơn hàng
        return View("OrderCompleted", order.Id);
    }
   
    [HttpGet]
    public IActionResult Invoice(int orderId)
    {
        // Lấy thông tin đơn hàng từ cơ sở dữ liệu
        var order = _context.Orders
            .Include(o => o.ApplicationUser) // Tải thông tin người dùng
            .Include(o => o.OrderDetails) // Tải chi tiết đơn hàng
            .ThenInclude(d => d.Product) // Tải thông tin sản phẩm
            .FirstOrDefault(o => o.Id == orderId);

        if (order == null)
        {
            return RedirectToAction("Index");
        }

        // Tạo ViewModel để truyền dữ liệu sang View
        var invoiceViewModel = new InvoiceViewModel
        {
            OrderId = order.Id,
            OrderDate = order.OrderDate,
            CustomerName = order.ApplicationUser?.FullName ?? "Unknown",
            CustomerEmail = order.ApplicationUser?.Email ?? "Unknown",
            Items = order.OrderDetails.Select(d => new InvoiceItem
            {
                ProductName = d.Product.Name,
                Quantity = d.Quantity,
                Price = d.Price // Đảm bảo giá được truyền đúng
            }).ToList(),
            TotalAmount = order.TotalPrice // Tổng giá trị đơn hàng
        };

        return View(invoiceViewModel);
    }

   
    public async Task<IActionResult> AddToCart(int productId, int quantity)
    {
        // Giả sử bạn có phương thức lấy thông tin sản phẩm từ productId
        var product = await GetProductFromDatabase(productId);

        var cartItem = new CartItem
        {
            ProductId = productId,
            Name = product.Name,
            Price = product.Price,
            Quantity = quantity
        };

        var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
        cart.AddItem(cartItem);

        HttpContext.Session.SetObjectAsJson("Cart", cart);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult GetCartItemCount()
    {
        // Lấy giỏ hàng từ session
        var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

        // Tính tổng số lượng sản phẩm trong giỏ hàng
        int itemCount = cart.Items.Sum(item => item.Quantity);

        // Trả về số lượng dưới dạng JSON
        return Json(new { count = itemCount });
    }


    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart")?? new ShoppingCart();
        if (cart.IsEmpty())
        {
            // Xử lý giỏ hàng trống...
            return View("EmptyCart");
        }
        return View(cart);
    }

    // Các actions khác...
    private async Task<Product> GetProductFromDatabase(int productId)
    {
        // Truy vấn cơ sở dữ liệu để lấy thông tin sản phẩm
        var product = await _productRepository.GetByIdAsync(productId);
        return product;
    }

    [HttpPost]
    
    public IActionResult UpdateQuantity(int productId, int quantity)
    {
        // Lấy giỏ hàng từ session
        var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
        if (cart == null)
        {
            return RedirectToAction("Index");
        }

        // Tìm sản phẩm trong giỏ hàng
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            // Cập nhật số lượng (đảm bảo số lượng >= 1)
            item.Quantity = quantity > 0 ? quantity : 1;
        }

        // Lưu giỏ hàng trở lại session
        HttpContext.Session.SetObjectAsJson("Cart", cart);

        // Quay lại trang giỏ hàng
        return RedirectToAction("Index");
    }

    public IActionResult RemoveFromCart(int productId)
    {
        var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

        if (cart != null)
        {
            cart.RemoveItem(productId);
        }

        // Lưu lại giỏ hàng vào Session sau khi đã xóa mục
        HttpContext.Session.SetObjectAsJson("Cart", cart);

        return RedirectToAction("Index");
    }

    
}
