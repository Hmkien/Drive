using Drive.Data;
using Drive.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Drive.Controllers
{
    [Authorize]

    public class TrashController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TrashController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var deletedItems = await _context.Trashes
                .Include(t => t.File)
                .Include(t => t.Folder)
                .ToListAsync();

            return View(deletedItems);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Trashes
                .Include(t => t.Folder)
                .Include(t => t.File)
                .FirstOrDefaultAsync(t => t.TrashId == id);

            if (item == null)
            {
                return NotFound();
            }

            _context.Trashes.Remove(item);

            switch (item.ItemType)
            {
                case "Folder":
                    if (item.Folder != null)
                    {
                        var count = await _context.Files.Where(e => e.FolderId == item.Folder.FolderId).ToListAsync();
                        if (count.Count > 0)
                        {
                            _context.Files.RemoveRange(count);
                        }
                        _context.Folders.Remove(item.Folder);
                    }
                    break;
                case "File":
                    if (item.File != null)
                    {
                        _context.Files.Remove(item.File);
                    }
                    break;
                default:
                    return BadRequest("Invalid ItemType.");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Restore(int id)
        {
            var trash = await _context.Trashes
                .Include(t => t.Folder)
                .Include(t => t.File)
                .FirstOrDefaultAsync(t => t.TrashId == id);

            if (trash == null)
            {
                return NotFound();
            }
            try
            {
                switch (trash.ItemType)
                {
                    case "Folder":
                        if (trash.Folder != null)
                        {
                            trash.Folder.isDelete = false;
                            _context.Folders.Update(trash.Folder);
                        }
                        break;

                    case "File":
                        if (trash.File != null)
                        {
                            trash.File.isDelete = false;
                            _context.Files.Update(trash.File);
                        }
                        break;

                    default:
                        return BadRequest("Invalid ItemType.");
                }
                _context.Trashes.Remove(trash);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi" + ex.Message);
            }


        }

    }
}