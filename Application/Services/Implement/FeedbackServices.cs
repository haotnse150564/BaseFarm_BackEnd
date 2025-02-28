using Application.Commons;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using static Infrastructure.ViewModel.Response.FeedbackResponse;

namespace Application.Services.Implement
{
    public class FeedbackServices : IFeedbackSevices
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICurrentTime _currentTime;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;

        public FeedbackServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
        }

        public async Task<ResponseDTO> GetAllFeedbackAsync(int pageIndex, int pageSize)
        {
            try
            {
                var totalItemCount = await _unitOfWork.feedbackRepository.CountAsync(); // Đếm tổng số sản phẩm
                var listFeedback = await _unitOfWork.feedbackRepository
                                    .GetPagedAsync(pageIndex, pageSize); // Lấy danh sách sản phẩm theo trang

                if (listFeedback == null || !listFeedback.Any())
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Feedback found.");
                }

                // Map dữ liệu sang DTO
                var result = _mapper.Map<List<ViewFeedbackDTO>>(listFeedback);

                // Tạo đối tượng phân trang
                var pagination = new Pagination<ViewFeedbackDTO>
                {
                    TotalItemCount = totalItemCount,
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
    }
}
