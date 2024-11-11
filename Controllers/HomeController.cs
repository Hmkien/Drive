using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Drive.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Drive.Data;
using Microsoft.EntityFrameworkCore;
using Drive.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Drive.Controllers;
[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        var folders = await _context.Folders
            .Where(e => e.UserId == userId && e.isDelete == false && e.ParentFolderId == null).OrderBy(e => e.CreatedAt)
            .Take(5)
            .ToListAsync();

        var files = await _context.Files
            .Where(e => e.UserId == userId && e.isDelete == false && e.Folder == null && e.FolderId == null)
            .Take(10)
            .ToListAsync();
        var folderList = _context.Folders.ToList();
        folderList.Insert(0, new Folder { FolderId = -1, FolderName = "Không có thư mục" });

        ViewData["FolderList"] = new SelectList(folderList, "FolderId", "FolderName");
        var model = new FolderViewModel
        {
            FileList = files,
            FolderList = folders,
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFolder(string folderName, int? parentFolder)
    {
        try
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var folder = new Folder();
            folder.UserId = userId;
            folder.FolderName = folderName;
            folder.ParentFolderId = parentFolder;
            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();
            if (parentFolder != null)
            {
                return RedirectToAction("FolderDetail", "Folder", new { id = parentFolder });
            }
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            return BadRequest("Đã xảy ra lỗi" + ex.Message);
        }

    }
    public async Task<IActionResult> DeleteFolder(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }
        var trash = new Trash
        {
            UserId = userId,
            DeletedAt = DateTime.Now,
            ExpirationDate = DateTime.Now.AddDays(30)
        };

        var folder = await _context.Folders.FindAsync(id);
        if (folder != null)
        {
            folder.isDelete = true;
            trash.ItemType = "Folder";
            trash.Folder = folder;
            _context.Trashes.Add(trash);
            _context.Folders.Update(folder);

            await _context.SaveChangesAsync();
            if (folder.ParentFolderId != null)
            {
                return RedirectToAction("FolderDetail", "Folder", new { id = folder.ParentFolderId });
            }
            return RedirectToAction("Index");
        }
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }
        var trash = new Trash
        {
            UserId = userId,
            DeletedAt = DateTime.Now,
            ExpirationDate = DateTime.Now.AddDays(30)
        };
        var file = await _context.Files.FindAsync(id);
        if (file != null)
        {
            file.isDelete = true;
            trash.File = file;
            trash.ItemType = "File";
            _context.Trashes.Add(trash);
            _context.Files.Update(file);

            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> Search(string keySeach)
    {
        FolderViewModel model = new FolderViewModel
        {
            FolderList = new List<Folder>(),
            FileList = new List<File>()
        };

        if (keySeach != null)
        {
            var folder = await _context.Folders
                .Where(e => e.FolderName.Contains(keySeach))
                .ToListAsync();
            var file = await _context.Files
                .Where(e => e.FileName.Contains(keySeach))
                .ToListAsync();

            model.FolderList = folder;
            model.FileList = file;
        }

        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> RenameFile(int FileId, int? FolderId, string newFileName)
    {
        if (string.IsNullOrWhiteSpace(newFileName))
        {
            return BadRequest("Tên file mới không được để trống.");
        }

        var file = await _context.Files.FindAsync(FileId);
        if (file == null)
        {
            return NotFound("Không tìm thấy file.");
        }

        file.FileName = newFileName;
        await _context.SaveChangesAsync();
        if (FolderId != null)
        {
            return RedirectToAction("FolderDetail", "Folder", new { id = FolderId });
        }
        return RedirectToAction("Index", "Home");
    }
    [HttpPost]
    public async Task<IActionResult> RenameFolder(int FolderId, int? parentFolderId, string newFolderName)
    {
        if (string.IsNullOrWhiteSpace(newFolderName))
        {
            return BadRequest("Tên file mới không được để trống.");
        }

        var folder = await _context.Folders.FindAsync(FolderId);
        if (folder == null)
        {
            return NotFound("Không tìm thấy file.");
        }

        folder.FolderName = newFolderName;
        await _context.SaveChangesAsync();
        if (parentFolderId != null)
        {
            return RedirectToAction("FolderDetail", "Folder", new { id = parentFolderId });
        }
        return RedirectToAction("Index", "Home");
    }
    [HttpPost]
    public async Task<IActionResult> MovingFile(int FileId, int? FolderId, int DesFolderId)
    {
        if (DesFolderId < 0)
        {
            var fileInitial = await _context.Files.FindAsync(FileId);
            if (fileInitial == null)
            {
                return NotFound("Không tìm thấy file.");
            }
            fileInitial.Folder = null;
            fileInitial.FolderId = null;
            await _context.SaveChangesAsync();
            if (FolderId != null)
            {
                return RedirectToAction("FolderDetail", "Folder", new { id = FolderId });
            }
            return RedirectToAction("Index", "Home");
        }
        var folder = await _context.Folders.FindAsync(DesFolderId);
        if (folder == null)
        {
            return NotFound("Không tìm thấy thư mục đích.");
        }

        var file = await _context.Files.FindAsync(FileId);
        if (file == null)
        {
            return NotFound("Không tìm thấy file.");
        }

        file.Folder = folder;
        file.FolderId = folder.FolderId;
        await _context.SaveChangesAsync();

        if (FolderId != null)
        {
            return RedirectToAction("FolderDetail", "Folder", new { id = FolderId });
        }
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> MovingFolder(int FolderId, int? parentFolderId, int DesFolderId)
    {
        var folderInitial = await _context.Folders.FindAsync(FolderId);

        if (DesFolderId < 0)
        {
            if (folderInitial == null)
            {
                return NotFound("Không tìm thấy thư mục.");
            }

            folderInitial.ParentFolderId = null;
            await _context.SaveChangesAsync();
            return parentFolderId != null
                ? RedirectToAction("FolderDetail", "Folder", new { id = parentFolderId })
                : RedirectToAction("Index", "Home");
        }
        var destinationFolder = await _context.Folders.FindAsync(DesFolderId);
        if (destinationFolder == null)
        {
            return NotFound("Không tìm thấy thư mục đích.");
        }
        if (destinationFolder.FolderId == FolderId)
        {
            return BadRequest("Không thể di chuyển thư mục vào chính nó.");
        }
        if (folderInitial == null)
        {
            return NotFound("Không tìm thấy thư mục cần di chuyển.");
        }

        folderInitial.ParentFolderId = DesFolderId;
        await _context.SaveChangesAsync();
        return parentFolderId != null
            ? RedirectToAction("FolderDetail", "Folder", new { id = parentFolderId })
            : RedirectToAction("Index", "Home");
    }

}


