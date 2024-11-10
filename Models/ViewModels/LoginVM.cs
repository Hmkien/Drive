using System.ComponentModel.DataAnnotations;

namespace Drive.Models.ViewModels
{
    public class LoginVM

    {
        [Required(ErrorMessage = "Thiếu tên đăng nhập")]
        [MaxLength(50)]
        [Display(Name = "Tên đăng nhập")]
        public String? Username { get; set; }
        [Required(ErrorMessage = "Thiếu mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu ")]
        public string? Password { get; set; }
        [Display(Name = "Nhớ mật khẩu")]
        public bool RememberMe { get; set; }
    }
}