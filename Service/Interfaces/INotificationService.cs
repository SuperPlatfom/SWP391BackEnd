using BusinessObject.DTOs.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface INotificationService
    {
        Task CreateAsync(Guid userId, string title, string message, string type, Guid? refId = null);
        Task<IEnumerable<NotificationDto>> GetMyAsync(Guid userId);
        Task MarkAsReadAsync(Guid id);
        Task MarkAllAsReadAsync(Guid userId);
    }
}
