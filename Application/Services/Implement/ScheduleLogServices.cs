using Application.Commons;
using Application.Interfaces;
using Application.Utils;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Infrastructure.Repositories;
using Infrastructure.ViewModel.Request;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.ScheduleResponse;

namespace Application.Services.Implement
{
    public class ScheduleLogServices : IScheduleLogServices
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICurrentTime _currentTime;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;
        private readonly JWTUtils _jwtUtils;
        private readonly IScheduleLogRepository _scheduleLogRepo;

        public ScheduleLogServices(
            IUnitOfWorks unitOfWork,
            ICurrentTime currentTime,
            IConfiguration configuration,
            IMapper mapper,
            JWTUtils jWTUtils,
            IScheduleLogRepository scheduleLogRepo
            )
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
            _jwtUtils = jWTUtils;
            _scheduleLogRepo = scheduleLogRepo;
        }
        public async Task<ResponseDTO> GetLogsByScheduleIdAsync(long scheduleId, int pageIndex = 1, int pageSize = 10)
        {
            // Optional: Check schedule tồn tại trước (tốt cho UX)
            var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null)
            {
                return new ResponseDTO(Const.WARNING_NO_DATA_CODE, "Không tìm thấy lịch với ID này");
                // hoặc Const.NOT_FOUND_CODE nếu bạn có
            }

            var resultView = await _scheduleLogRepo.GetAllWithName(scheduleId);

            //var resultView = _mapper.Map<List<ScheduleLogDto>>(list ?? new List<ScheduleLog>());

            var pagination = new Pagination<ScheduleLogResponse>
            {
                TotalItemCount = resultView.Count,
                PageSize = pageSize,
                PageIndex = pageIndex,
                Items = resultView.Skip(pageIndex * pageSize).Take(pageSize).ToList()
            };

            // ✅ LUÔN TRẢ SUCCESS nếu không có lỗi, dù Items rỗng
            // Đây là chuẩn REST: 200 OK + empty array
            return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, pagination);
        }

        public async Task<ResponseDTO> CreateManualLogAsync(CreateScheduleLogRequest request)
        {
            var user = await _jwtUtils.GetCurrentUserAsync();
            if (user == null)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, "Người dùng không hợp lệ");
            }

            var userName = user.AccountProfile.Fullname;
            // Validate
            if (request.ScheduleId <= 0)
                return new ResponseDTO(Const.FAIL_CREATE_CODE, "ScheduleId không hợp lệ");

            if (string.IsNullOrWhiteSpace(request.Notes))
                return new ResponseDTO(Const.FAIL_CREATE_CODE, "Ghi chú không được để trống");

            // Optional: Check schedule tồn tại (nên có)
            var scheduleExists = await _unitOfWork.scheduleRepository.GetByIdAsync(request.ScheduleId);
            if (scheduleExists == null)
                return new ResponseDTO(Const.WARNING_NO_DATA_CODE, "Không tìm thấy lịch với ID này");

            // Tạo log
            var log = new ScheduleLog
            {
                ScheduleId = request.ScheduleId,
                Notes = $"[Ghi chú thủ công] {request.Notes.Trim()}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = user.AccountId,
            };

            await _scheduleLogRepo.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();

            var logDto = _mapper.Map<ScheduleLogDto>(log);

            return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "Ghi log thành công", logDto);
        }

        public async Task<ResponseDTO> UpdateManualLogAsync(UpdateScheduleLogRequest request)
        {
            var user = await _jwtUtils.GetCurrentUserAsync();
            if (user == null)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, "Người dùng không hợp lệ");
            }
            // Validate
            if (request.LogId <= 0)
                return new ResponseDTO(Const.FAIL_UPDATE_CODE, "LogId không hợp lệ");

            if (string.IsNullOrWhiteSpace(request.Notes))
                return new ResponseDTO(Const.FAIL_UPDATE_CODE, "Ghi chú không được để trống");

            // Tìm log cần update
            var existingLog = await _scheduleLogRepo.GetByIdAsync(request.LogId);
            if (existingLog == null)
                return new ResponseDTO(Const.WARNING_NO_DATA_CODE, "Không tìm thấy bản ghi log này");

            // Chỉ cho phép update log do chính mình tạo hoặc Manager
            if (existingLog.CreatedBy != user.AccountId && user.Role != Roles.Manager)
                return new ResponseDTO(Const.FAIL_UPDATE_CODE, "Bạn không có quyền sửa log này");

            // Backup old notes để ghi lịch sử thay đổi (tùy chọn nâng cao)
            var oldNotes = existingLog.Notes;

            // Update
            existingLog.Notes = $"[Ghi chú thủ công (Đã sửa lúc {DateTime.UtcNow:HH:mm dd/MM/yyyy})] {request.Notes.Trim()}";

            existingLog.UpdatedAt = DateTime.UtcNow;
            existingLog.UpdatedBy = user.AccountId;

            await _scheduleLogRepo.UpdateAsync(existingLog);
            await _unitOfWork.SaveChangesAsync();
            var account1 = await _unitOfWork.accountRepository.GetByIdAsync(existingLog.CreatedBy);
            var account2 = await _unitOfWork.accountRepository.GetByIdAsync(existingLog.UpdatedBy);
            var logDto = _mapper.Map<ScheduleLogResponse>(existingLog);
            logDto.StaffNameCreate = account1 != null && account1.AccountProfile != null
                ? account1.AccountProfile.Fullname
                : "Không tìm thấy tên người dùng";

            logDto.StaffNameUpdate = account2 != null && account2.AccountProfile != null
                ? account2.AccountProfile.Fullname
                : "Không tìm thấy tên người dùng";

            return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, "Cập nhật log thành công", logDto);
        }
    }
}
