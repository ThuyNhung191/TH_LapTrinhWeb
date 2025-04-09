using WebBanHang.Models;

public class ShoppingCart
{
    public List<CartItem> Items { get; set; } = new List<CartItem>();

    // Thêm sản phẩm vào giỏ hàng
    public void AddItem(CartItem item)
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existingItem != null)
        {
            // Nếu sản phẩm đã có trong giỏ, tăng số lượng
            existingItem.Quantity += item.Quantity;
        }
        else
        {
            // Nếu sản phẩm chưa có trong giỏ, thêm mới vào giỏ hàng
            Items.Add(item);
        }
    }

    // Xóa sản phẩm khỏi giỏ hàng
    public void RemoveItem(int productId)
    {
        Items.RemoveAll(i => i.ProductId == productId);
    }

    //Tính tổng giá trị giỏ hàng
    public decimal CalculateTotal()
    {
        return Items.Sum(item => item.Quantity * item.Product.Price);
    }

    // Kiểm tra giỏ hàng có rỗng không
    public bool IsEmpty()
    {
        return Items.Count == 0;
    }
}
