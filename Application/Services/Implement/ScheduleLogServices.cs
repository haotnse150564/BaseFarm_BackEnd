using Application.Commons;
using Application.Interfaces;
using Application.Utils;
using AutoMapper;
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
using static Application.ViewModel.Response.OrderResponse;

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

            var list = await _scheduleLogRepo.GetAllByScheduleIdAsync(scheduleId);

            var resultView = _mapper.Map<List<ScheduleLogDto>>(list ?? new List<ScheduleLog>());

            var pagination = new Pagination<ScheduleLogDto>
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
    }
}
