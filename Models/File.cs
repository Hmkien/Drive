using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Drive.Models;
using Microsoft.AspNetCore.Identity;

public class File
{
    [Key]
    public int FileId { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    [RegularExpression(@"^[\w\-. ]+$", ErrorMessage = "Tên tệp không hợp lệ.")]
    public string FileName { get; set; }

    [Required]
    public string FilePath { get; set; }

    [Range(0, long.MaxValue)]
    public long FileSize { get; set; }

    [RegularExpression(@"^[\w]+\/[\w\-.]+$", ErrorMessage = "Loại tệp không hợp lệ.")]
    public string FileType { get; set; }

    public DateTime UploadedAt { get; set; }
    public int? FolderId { get; set; }

    [ForeignKey("FolderId")]
    public virtual Folder? Folder { get; set; }
    public bool IsShared { get; set; } = false;
    public bool isDelete { get; set; } = false;
    public bool Star { get; set; } = false;

}


