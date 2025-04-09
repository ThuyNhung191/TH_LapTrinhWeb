document.addEventListener("DOMContentLoaded", function () {
    const cartItemCount = document.getElementById("cart-item-count");

    // Hàm cập nhật số lượng sản phẩm trong giỏ hàng
    function updateCartItemCount() {
        fetch('/ShoppingCart/GetCartItemCount')
            .then(response => response.json())
            .then(data => {
                if (cartItemCount) {
                    cartItemCount.textContent = data.count;
                    cartItemCount.style.display = data.count > 0 ? 'inline' : 'none';
                }
            })
            .catch(error => console.error('Error fetching cart item count:', error));
    }

    // Gọi hàm cập nhật số lượng khi trang được tải
    updateCartItemCount();
});