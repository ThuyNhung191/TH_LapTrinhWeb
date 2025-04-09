using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.Areas.Identity.Data;

public class Order
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? NgayCan { get; set; }
    public DateTime? NgayGiao { get; set; }
    public string? HoTen { get; set; }
    public string? DienThoai { get; set; }
    public string CachThanhToan { get; set; } = null!;
    public string CachVanChuyen { get; set; } = null!;
    public double PhiVanChuyen { get; set; }
    public int MaTrangThai { get; set; }
    public decimal TotalPrice { get; set; }

    [Required(ErrorMessage = "Shipping Address is required.")]
    public string? ShippingAddress { get; set; }
    public string? Notes { get; set; }


    [ForeignKey("UserId")]
    public ApplicationUser? ApplicationUser { get; set; }
    public List<OrderDetail>? OrderDetails { get; set; }
}
