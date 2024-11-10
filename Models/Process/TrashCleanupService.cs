using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Drive.Data;

namespace Drive.Models.Process
{
    public class TrashCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public TrashCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CleanupExpiredTrashAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);  // Điều chỉnh thời gian nếu cần
            }
        }

        private async Task CleanupExpiredTrashAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var expiredItems = await context.Trashes
                    .Where(t => t.ExpirationDate >= DateTime.UtcNow)
                    .Include(t => t.File)
                    .Include(t => t.Folder)
                    .ToListAsync(stoppingToken);

                foreach (var item in expiredItems)
                {
                    if (stoppingToken.IsCancellationRequested)
                        return;
                    context.Trashes.Remove(item);


                    if (item.ItemType == "Folder" && item.Folder != null)
                    {
                        context.Folders.Remove(item.Folder);
                    }
                    else if (item.ItemType == "File" && item.File != null)
                    {
                        context.Files.Remove(item.File);
                    }

                }

                await context.SaveChangesAsync(stoppingToken);
            }
        }
    }
}
