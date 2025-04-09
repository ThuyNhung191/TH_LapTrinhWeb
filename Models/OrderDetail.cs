using WebBanHang.Models;

public class OrderDetail
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    // Thêm ReturnUrl vào OrderDetail
    public string ReturnUrl { get; set; } = string.Empty; // Đảm bảo không null

    public Order Order { get; set; }
    public Product Product { get; set; }
}
