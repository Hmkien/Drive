using System.ComponentModel.DataAnnotations;

namespace Drive.Models.ViewModels
{
    public class RegisterVM
    {
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Tên đăng nhập trống")]
        public string? Name { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email trống")]
        public string? Email { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu trống")]
        public string? Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        public string? ConfirmPassword { get; set; }

    }
    public class FolderViewModel
    {
        public Folder CurrentFolder { get; set; }
        public List<Folder> FolderList { get; set; }
        public List<File> FileList { get; set; }
    }

}