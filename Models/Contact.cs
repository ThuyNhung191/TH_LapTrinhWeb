using System.ComponentModel.DataAnnotations;


namespace WebBanHang.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên của bạn.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại của bạn.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email của bạn.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung tin nhắn của bạn.")]
        public string? Message { get; set; }
        
    }
}

