using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drive.Models
{
    public class Trash
    {
        public int TrashId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [RegularExpression(@"^(File|Folder)$", ErrorMessage = "Loại đối tượng không hợp lệ.")]
        public string ItemType { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DeletedAt { get; set; }
        public DateTime ExpirationDate { get; set; }

        public int? FileId { get; set; }
        public int? FolderId { get; set; }

        [ForeignKey("FileId")]
        public virtual File? File { get; set; }

        [ForeignKey("FolderId")]
        public virtual Folder? Folder { get; set; }

    }

}
