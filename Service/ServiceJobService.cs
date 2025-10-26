using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Threading.Tasks;

namespace Service
{
    public class ServiceJobService : IServiceJobService
    {
        private readonly IServiceRequestRepository _requestRepo;
        private readonly IServiceJobRepository _jobRepo;

        public ServiceJobService(IServiceRequestRepository requestRepo, IServiceJobRepository jobRepo)
        {
            _requestRepo = requestRepo;
            _jobRepo = jobRepo;
        }

        public async Task CreateAfterFullPaymentAsync(Guid expenseId)
        {

            var request = await _requestRepo.GetByExpenseIdAsync(expenseId);
            if (request == null)
                throw new KeyNotFoundException("Không tìm thấy yêu cầu dịch vụ tương ứng với expense.");


            var existingJob = await _jobRepo.GetByRequestIdAsync(request.Id);
            if (existingJob != null) return;

            var job = new ServiceJob
            {
                Id = Guid.NewGuid(),
                RequestId = request.Id,
                TechnicianId = request.TechnicianId ?? throw new InvalidOperationException("Chưa gán kỹ thuật viên."),
                ScheduledAt = DateTime.UtcNow,
                Status = "SCHEDULED",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _jobRepo.AddAsync(job);

            request.Status = "IN_PROGRESS";
            request.UpdatedAt = DateTime.UtcNow;



            await _jobRepo.SaveChangesAsync();
        }
        public async Task<IEnumerable<ServiceJobDto>> GetAllAsync(Guid? technicianId = null)
        {
            var list = await _jobRepo.GetAllAsync();
            if (technicianId.HasValue)
                list = list.Where(j => j.TechnicianId == technicianId.Value);

            return list.Select(j => new ServiceJobDto
            {
                Id = j.Id,
                Title = j.Request.Title,
                TechnicianName = j.Technician.FullName,
                ScheduledAt = j.ScheduledAt,
                CompletedAt = j.CompletedAt,
                ReportUrl = j.ReportUrl,
                Status = j.Status,
                CreatedAt = j.CreatedAt
            });
        }


        public async Task<ServiceJobDto> GetByIdAsync(Guid id)
        {
            var job = await _jobRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Không tìm thấy công việc dịch vụ");

            return new ServiceJobDto
            {
                Id = job.Id,
                Title = job.Request.Title,
                TechnicianName = job.Technician.FullName,
                ScheduledAt = job.ScheduledAt,
                CompletedAt = job.CompletedAt,
                ReportUrl = job.ReportUrl,
                Status = job.Status,
                CreatedAt = job.CreatedAt
            };
        }


        public async Task UpdateStatusAsync(Guid jobId, UpdateServiceJobStatusRequest req, Guid technicianId)
        {
            var job = await _jobRepo.GetByIdAsync(jobId)
                ?? throw new KeyNotFoundException("Không tìm thấy công việc.");

            if (job.TechnicianId != technicianId)
                throw new UnauthorizedAccessException("Bạn không có quyền cập nhật công việc này.");

            var newStatus = req.Status.ToUpper();
            job.Status = newStatus;
            job.UpdatedAt = DateTime.UtcNow;


            if (newStatus == "DONE")
            {
                job.CompletedAt = DateTime.UtcNow;


                var request = await _requestRepo.GetByIdAsync(job.RequestId);
                if (request != null)
                {
                    request.Status = "COMPLETED";
                    request.CompletedAt = DateTime.UtcNow;
                    request.UpdatedAt = DateTime.UtcNow;


                }
            }
            else if (newStatus == "CANCELED")
            {

                var request = await _requestRepo.GetByIdAsync(job.RequestId);
                if (request != null)
                {
                    request.Status = "CANCELED";
                    request.UpdatedAt = DateTime.UtcNow;

                }
            }

            await _jobRepo.UpdateAsync(job);
            await _jobRepo.SaveChangesAsync();
        }



        public async Task UpdateReportAsync(Guid jobId, UpdateServiceJobReportRequest req, Guid technicianId)
        {
            var job = await _jobRepo.GetByIdAsync(jobId)
                ?? throw new KeyNotFoundException("Không tìm thấy công việc.");

            if (job.TechnicianId != technicianId)
                throw new UnauthorizedAccessException("Bạn không có quyền cập nhật công việc này.");

            job.ReportUrl = req.ReportUrl;
            job.UpdatedAt = DateTime.UtcNow;
            await _jobRepo.UpdateAsync(job);
            await _jobRepo.SaveChangesAsync();
        }
    }
}
