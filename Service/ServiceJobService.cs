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
        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly INotificationService _notificationService;
        private readonly IGroupMemberRepository _groupMemberRepo;
        public ServiceJobService(IServiceRequestRepository requestRepo, IServiceJobRepository jobRepo, IFirebaseStorageService firebaseStorageService, INotificationService notificationService, IGroupMemberRepository groupMemberRepo)
        {
            _requestRepo = requestRepo;
            _jobRepo = jobRepo;
            _firebaseStorageService = firebaseStorageService;
            _notificationService = notificationService;
            _groupMemberRepo = groupMemberRepo;
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
        public async Task<IEnumerable<ServiceJobListDto>> GetAllAsync(Guid? technicianId = null)
        {
            var list = await _jobRepo.GetAllAsync();
            if (technicianId.HasValue)
                list = list.Where(j => j.TechnicianId == technicianId.Value);

            return list.Select(j => new ServiceJobListDto
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
                VehicleName = $"{job.Request.Vehicle.Make} {job.Request.Vehicle.Model}",
                PlateNumber = job.Request.Vehicle.PlateNumber ?? "",
                GroupName = job.Request.Group.Name,
                RequestCreatedBy = job.Request.CreatedByAccount.FullName,
                IssueDescription = job.Request.Description ?? "",
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

            var request = await _requestRepo.GetByIdAsync(job.RequestId)
                ?? throw new KeyNotFoundException("Không tìm thấy yêu cầu dịch vụ.");

            var vehicleName = $"{request.Vehicle.Make} {request.Vehicle.Model}";
            var plate = string.IsNullOrWhiteSpace(request.Vehicle.PlateNumber) ? "" : $" - {request.Vehicle.PlateNumber}";
            var title = request.Title;
            var members = await _groupMemberRepo.GetByGroupIdAsync(request.GroupId);

            if (newStatus == "DONE")
            {
                job.CompletedAt = DateTime.UtcNow;
                request.Status = "COMPLETED";
                request.CompletedAt = job.CompletedAt;
                request.UpdatedAt = DateTime.UtcNow;

                foreach (var m in members)
                {
                    await _notificationService.CreateAsync(
                        m.UserId,
                        "Dịch vụ hoàn thành",
                        $"Dịch vụ \"{title}\" cho xe {vehicleName}{plate} đã hoàn thành.",
                        "SERVICE_JOB_DONE",
                        request.Id
                    );
                }
            }
            else if (newStatus == "CANCELED")
            {
                request.Status = "CANCELED";
                request.UpdatedAt = DateTime.UtcNow;

                foreach (var m in members)
                {
                    await _notificationService.CreateAsync(
                        m.UserId,
                        "Dịch vụ bị huỷ",
                        $"Dịch vụ \"{title}\" cho xe {vehicleName}{plate} đã bị huỷ.",
                        "SERVICE_JOB_CANCELED",
                        request.Id
                    );
                }
            }

            await _requestRepo.UpdateAsync(request);
            await _jobRepo.UpdateAsync(job);
            await _jobRepo.SaveChangesAsync();
        }




        public async Task UpdateReportAsync(Guid jobId, UpdateServiceJobReportRequest req, Guid technicianId)
        {
            var job = await _jobRepo.GetByIdAsync(jobId)
                ?? throw new KeyNotFoundException("Không tìm thấy công việc.");

            if (job.TechnicianId != technicianId)
                throw new UnauthorizedAccessException("Bạn không có quyền cập nhật công việc này.");

            var fileUrl = await _firebaseStorageService.UploadFileAsync(req.ReportFile, "service_reports");
            job.ReportUrl = fileUrl;
            job.UpdatedAt = DateTime.UtcNow;
            await _jobRepo.UpdateAsync(job);
            await _jobRepo.SaveChangesAsync();
        }
    }
}
