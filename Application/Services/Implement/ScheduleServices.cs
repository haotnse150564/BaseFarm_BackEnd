using Application.Commons;
using Application.Interfaces;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Infrastructure.Repositories;
using Infrastructure.ViewModel.Request;
using Microsoft.Extensions.Configuration;
using static Infrastructure.ViewModel.Response.ScheduleResponse;
using ResponseDTO = Infrastructure.ViewModel.Response.ScheduleResponse.ResponseDTO;

namespace Application.Services.Implement
{
    public class ScheduleServices : IScheduleServices
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICurrentTime _currentTime;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;
        private readonly IAccountRepository _account;
        public ScheduleServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper, IAccountRepository account)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
            _account = account;
        }


        public Task<ResponseDTO> CreateScheduleAsync(ScheduleRequest request)
        {
            throw new NotImplementedException();
        }
        public async Task<ResponseDTO> ChangeScheduleStatusById(long ScheduleId, string status)
        {
            try
            {
                var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(ScheduleId);

                if (schedule == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Schedule found.");
                }

                // Map dữ liệu sang DTO
                schedule.Status = (Status)Enum.Parse(typeof(Status), status); // Chuyển chuỗi sang Enum
                await _unitOfWork.scheduleRepository.UpdateAsync(schedule);
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.Map<ViewSchedule>(schedule);
                result.FullNameStaff = (await _account.GetAccountProfileByAccountIdAsync(schedule.AssignedTo)).AccountProfile?.Fullname;
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }




        public async Task<ResponseDTO> GetAllScheduleAsync(int pageIndex, int pageSize)
        {
            try
            {
                var list = await _unitOfWork.scheduleRepository.GetAllAsync();

                if (list == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Schedule found.");
                }

                // Map dữ liệu sang DTO
                var result = _mapper.Map<List<ViewSchedule>>(list);
                var totalItem = result.Count();
                foreach (var item in result)
                {

                    item.FullNameStaff = (await _account.GetAccountProfileByAccountIdAsync(item.AssignedTo)).AccountProfile?.Fullname;

                }
                // Tạo đối tượng phân trang
                var pagination = new Pagination<ViewSchedule>
                {
                    TotalItemCount = totalItem,
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    Items = result
                };

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, pagination);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
        public async Task<ResponseDTO> UpdateScheduleById(long ScheduleId, ScheduleRequest request)
        {
            try
            {
                var schedule = await _unitOfWork.scheduleRepository.GetByIdAsync(ScheduleId);

                if (schedule == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Schedule found.");
                }
                var update = _mapper.Map(request, schedule);
                update.UpdatedAt = _currentTime.GetCurrentTime();
                // Map dữ liệu sang DTO
                _unitOfWork.scheduleRepository.Update(update);
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.Map<ViewSchedule>(update);
                result.FullNameStaff = (await _account.GetAccountProfileByAccountIdAsync(result.AssignedTo)).AccountProfile?.Fullname;
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
        public Task<ResponseDTO> GetScheduleByIdAsync(long ScheduleId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> AsignStaff(long scheduleID)
        {
            throw new NotImplementedException();
        }


    }
}
