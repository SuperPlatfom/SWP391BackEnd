using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification noti);
        Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId);
        Task MarkAsReadAsync(Guid id);
        Task MarkAllAsReadAsync(Guid userId);
        Task SaveChangesAsync();
    }
}
