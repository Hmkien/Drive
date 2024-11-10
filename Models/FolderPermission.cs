using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Drive.Models
{
    public class FolderPermission
    {
        [Key]
        public int PermissionId { get; set; }

        [Required]
        public int FolderId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [RegularExpression(@"^(View|Edit)$", ErrorMessage = "Cấp độ truy cập không hợp lệ.")]
        public string AccessLevel { get; set; }

        public DateTime GrantedAt { get; set; }
    }

}