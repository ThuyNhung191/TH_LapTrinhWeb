using System.ComponentModel.DataAnnotations;


namespace WebBanHang.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }  // Đường dẫn đến hình ảnh sản phẩm
         public Product? Product { get; set; }  // Sản phẩm tương ứng
    }
}

