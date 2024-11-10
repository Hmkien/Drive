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
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Drive.Controllers
{
    [Authorize]

    public class NavigationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NavigationController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> MyFile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var folders = await _context.Folders
                .Where(e => e.UserId == userId && e.isDelete == false && e.ParentFolderId == null).OrderBy(e => e.CreatedAt)
                .ToListAsync();

            var files = await _context.Files
                .Where(e => e.UserId == userId && e.isDelete == false && e.FolderId == null).OrderBy(e => e.UploadedAt)
                .ToListAsync();

            var model = new FolderViewModel
            {
                FileList = files,
                FolderList = folders,
            };

            return View(model);
        }
        public async Task<IActionResult> NearFile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var folders = await _context.Folders
                .Where(e => e.UserId == userId && e.isDelete == false).OrderBy(e => e.CreatedAt).Take(10)
                .ToListAsync();

            var files = await _context.Files
                .Where(e => e.UserId == userId && e.isDelete == false && e.FolderId == null).OrderBy(e => e.UploadedAt).Take(10)
                .ToListAsync();

            var model = new FolderViewModel
            {
                FileList = files,
                FolderList = folders,
            };

            return View(model);
        }
        public async Task<IActionResult> StarFile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var folders = await _context.Folders
                .Where(e => e.UserId == userId && e.isDelete == false && e.Star == true).OrderBy(e => e.CreatedAt)
                .ToListAsync();

            var files = await _context.Files
                .Where(e => e.UserId == userId && e.isDelete == false && e.FolderId == null && e.Star == true).OrderBy(e => e.UploadedAt)
                .ToListAsync();

            var model = new FolderViewModel
            {
                FileList = files,
                FolderList = folders,
            };

            return View(model);
        }
        public async Task<IActionResult> MyStorage()
        {
            return View();
        }
        public async Task<IActionResult> ShareFile()
        {
            return View();
        }
    }
}
