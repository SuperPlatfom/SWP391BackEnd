using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;

        public NotificationService(INotificationRepository repo)
        {
            _repo = repo;
        }

        public async Task CreateAsync(Guid userId, string title, string message, string type, Guid? refId = null)
        {
            var noti = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                RefId = refId
            };

            await _repo.AddAsync(noti);
            await _repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<NotificationDto>> GetMyAsync(Guid userId)
        {
            var list = await _repo.GetByUserIdAsync(userId);
            return list.Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                RefId = n.RefId,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            });
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            await _repo.MarkAsReadAsync(id);
            await _repo.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            await _repo.MarkAllAsReadAsync(userId);
            await _repo.SaveChangesAsync();
        }
    }
}
