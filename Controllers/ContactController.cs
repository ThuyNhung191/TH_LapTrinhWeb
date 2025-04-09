using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class ContactController : Controller
    {
        private readonly ProductDbContext _context;

        public ContactController(ProductDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Contact contact)
        {
            if (ModelState.IsValid)
            {
                // Lưu thông tin liên hệ vào cơ sở dữ liệu
                _context.Contacts.Add(contact);
                _context.SaveChanges();

                // Hiển thị thông báo thành công
                TempData["SuccessMessage"] = "Cảm ơn bạn đã liên hệ với chúng tôi. Chúng tôi sẽ phản hồi sớm nhất!";
                return RedirectToAction("Index");
            }

            // Nếu dữ liệu không hợp lệ, hiển thị lại form với thông báo lỗi
            return View(contact);
        }
    }
}