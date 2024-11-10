using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Drive.Models
{

    public class Folder
    {
        [Key]
        public int FolderId { get; set; }

        [Required]
        public string UserId { get; set; }

        public int? ParentFolderId { get; set; }

        [Required]
        [RegularExpression(@"^[\w\-. ]+$", ErrorMessage = "Tên thư mục không hợp lệ.")]
        public string FolderName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool isDelete { get; set; } = false;
        public bool Star { get; set; } = false;

    }
}