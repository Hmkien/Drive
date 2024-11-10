using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Drive.Models
{
    public class ActivityLog
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [RegularExpression(@"^(Uploaded|Deleted|Viewed|Edited)$", ErrorMessage = "Hành động không hợp lệ.")]
        public string Action { get; set; }

        [Required]
        [RegularExpression(@"^(File|Folder)$", ErrorMessage = "Loại đối tượng không hợp lệ.")]
        public string TargetType { get; set; }

        public int TargetId { get; set; }
        public DateTime Timestamp { get; set; }

    }

}