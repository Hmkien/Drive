using System.Security.Claims;
using Drive.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Drive.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        private readonly ApplicationDbContext _context;
        public FileController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> UploadFile(int? FolderId, IFormFile file)
        {
            if (file != null)
            {
                var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userid == null)
                {
                    return Unauthorized();
                }
                var fileType = file.ContentType;
                var allowedFileTypes = new List<string>
{
    "application/msword",
    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    "application/vnd.ms-excel",
    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    "application/vnd.ms-powerpoint",
    "application/vnd.openxmlformats-officedocument.presentationml.presentation",
    "image/jpeg",
    "image/png",
    "image/gif",
    "application/zip",
    "application/x-zip-compressed",
    "application/x-rar-compressed",
    "audio/mpeg",
    "audio/wav",
    "video/mp4",
    "application/pdf"
};

                if (!allowedFileTypes.Contains(fileType))
                {
                    return BadRequest("Loại tệp không được hỗ trợ.");
                }

                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var model = new File
                {
                    UserId = userid,
                    FileName = fileName,
                    FilePath = "~/Upload/" + fileName,
                    FileSize = file.Length,
                    FileType = fileType,
                    UploadedAt = DateTime.Now,
                };

                if (FolderId != null)
                {
                    model.FolderId = FolderId;
                }
                _context.Files.Add(model);
                await _context.SaveChangesAsync();
                if (FolderId != null)
                {
                    return RedirectToAction("FolderDetail", "Folder", new { id = FolderId });

                }
                return RedirectToAction("Index", "Home");
            }
            return BadRequest("Đã xảy ra lỗi, vui lòng thử lại!");
        }

        public async Task<IActionResult> Download(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var file = await _context.Files.FirstOrDefaultAsync(f => f.FileId == id && f.UserId == userId && f.isDelete == false);
            if (file == null)
            {
                return NotFound("Không tìm thấy tệp hoặc bạn không có quyền truy cập.");
            }
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FilePath.TrimStart('~', '/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Tệp không tồn tại.");
            }
            string contentType;
            switch (file.FileType.ToLower())
            {
                case "application/msword":
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case "application/vnd.ms-excel":
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case "application/vnd.ms-powerpoint":
                case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                    contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                    break;
                case "application/zip":
                case "application/x-rar-compressed":
                    contentType = file.FileType.ToLower();
                    break;
                case "audio/mpeg":
                    contentType = "audio/mpeg";
                    break;
                case "video/mp4":
                    contentType = "video/mp4";
                    break;
                case "image/jpeg":
                case "image/png":
                case "image/gif":
                    contentType = file.FileType.ToLower();
                    break;
                case "application/pdf":
                    contentType = "application/pdf";
                    break;
                default:
                    contentType = "application/octet-stream";
                    break;
            }
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, contentType, file.FileName);
        }
        public IActionResult GetFile(int fileId)
        {
            var file = _context.Files.FirstOrDefault(f => f.FileId == fileId);
            if (file == null) return NotFound();

            var filePath = file.FilePath;
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, file.FileType);
        }

    }
}