using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Drive.Data;
using Drive.Models;
using Drive.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Drive.Controllers
{
    [Authorize]

    public class FolderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FolderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> FolderDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var folder = await _context.Folders.FindAsync(id);
            var viewModel = new FolderViewModel
            {
                CurrentFolder = folder,
                FolderList = await _context.Folders.Where(e => e.ParentFolderId == id && e.isDelete == false).ToListAsync(),
                FileList = await _context.Files.Where(e => e.FolderId == id && e.isDelete == false).ToListAsync(),

            };
            ViewData["FolderId"] = id;
            return View(viewModel);
        }
        [HttpPost]
        [Route("Folder/ToggleFavorite")]
        public JsonResult ToggleFavorite(int? folderId, int? fileId)
        {
            try
            {
                if (folderId.HasValue)
                {
                    var folder = _context.Folders.Find(folderId.Value);
                    if (folder != null)
                    {
                        folder.Star = !folder.Star;
                        _context.SaveChanges();
                        return Json(new { success = true, isFavorite = folder.Star });
                    }
                }
                else if (fileId.HasValue)
                {
                    var file = _context.Files.Find(fileId.Value);
                    if (file != null)
                    {
                        file.Star = !file.Star;
                        _context.SaveChanges();
                        return Json(new { success = true, isFavorite = file.Star });
                    }
                }

                return Json(new { success = false, message = "Không tìm thấy thư mục hoặc tệp." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Đã xảy ra lỗi trên server." });
            }
        }
        [HttpPost]
        public IActionResult UploadFolder(IFormFile[] files, int? parentFolder)
        {
            //     var user = User.FindFirstValue(ClaimsIdentity.DefaultIssuer);
            //     if (user == null)
            //     {
            //         return Unauthorized();
            //     }

            //     if (files == null || files.Length == 0)
            //     {
            //         TempData["Error"] = "Không có file nào được chọn!";
            //         return RedirectToAction("UploadFolder");
            //     }

            //     var folder = new Folder
            //     {
            //         FolderName = "My Folder",
            //         UserId = user,
            //         ParentFolderId = parentFolder
            //     };

            //     _context.Folders.Add(folder);
            //     _context.SaveChanges();

            //     string uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload");

            //     if (!Directory.Exists(uploadDirectory))
            //     {
            //         Directory.CreateDirectory(uploadDirectory);
            //     }

            //     foreach (var file in files)
            //     {
            //         string filePath = Path.Combine(uploadDirectory, file.FileName);

            //         using (var stream = new FileStream(filePath, FileMode.Create))
            //         {
            //             file.CopyTo(stream);
            //         }

            //         var uploadedFile = new File
            //         {
            //             FileName = file.FileName,
            //             FilePath = "~/Upload/" + file.FileName,
            //             FileSize = file.Length,
            //             FileType = file.ContentType,
            //             FolderId = folder.FolderId,
            //             UserId = user
            //         };

            //         _context.Files.Add(uploadedFile);
            //     }

            //     _context.SaveChanges();

            //     if (parentFolder != null)
            //     {
            //         return RedirectToAction("FolderDetail", "Folder", new { id = parentFolder });
            //     }

            //     return RedirectToAction("Index", "Home");
            // }
            return BadRequest("Chức năng đang phát triển");
        }
    }
}