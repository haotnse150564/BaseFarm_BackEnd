using Application.Commons;
using Application.Interfaces;
using AutoMapper;
using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
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

        public ScheduleServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
        }
        public Task<ScheduleResponse> ChangeScheduleStatusById(long ScheduleId)
        {
            throw new NotImplementedException();
        }

        public Task<ScheduleResponse> CreateScheduleAsync(ScheduleRequest request)
        {
            throw new NotImplementedException();
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

        public Task<ScheduleResponse> GetScheduleByIdAsync(long ScheduleId)
        {
            throw new NotImplementedException();
        }

        public Task<ScheduleResponse> UpdateScheduleById(long ScheduleId, ScheduleRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
