using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Drive.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Drive.Data;
using Microsoft.EntityFrameworkCore;
using Drive.Models.ViewModels;

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
            .Where(e => e.UserId == userId && e.isDelete == false)
            .Take(5)
            .ToListAsync();

        var files = await _context.Files
            .Where(e => e.UserId == userId && e.isDelete == false)
            .Take(10)
            .ToListAsync();

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


}
