using Application.Commons;
using Application.Interfaces;
using AutoMapper;
using Domain.Model;
using Microsoft.Extensions.Configuration;
using static Infrastructure.ViewModel.Request.FeedbackRequest;
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

        public async Task<ResponseDTO> CreateFeedbackAsync(CreateFeedbackDTO request)
        {
            try
            {

                // Ánh xạ từ DTO sang Entity
                var feedback = _mapper.Map<Feedback>(request);
                feedback.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

                // Gọi AddAsync nhưng không gán vào biến vì nó không có giá trị trả về
                var create = _unitOfWork.feedbackRepository.AddAsync(feedback);

                // Kiểm tra xem sản phẩm có được thêm không bằng cách kiểm tra product.Id (hoặc khóa chính)
                if (create == null) // Nếu Id chưa được gán, có thể việc thêm đã thất bại
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "Failed to Cteate Feedback to repository.");
                }

                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "Feedback created!");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> UpdateFeedbackById(int feedbackId, CreateFeedbackDTO request)
        {
            try
            {
                var feedback = await _unitOfWork.feedbackRepository.GetByIdAsync(feedbackId);
                if (feedback == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Feedback not found !");
                }

                // Sử dụng AutoMapper để ánh xạ thông tin từ DTO vào
                var updatedFeedback = _mapper.Map(request, feedback);

                var result = _mapper.Map<ViewFeedbackDTO>(updatedFeedback);

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _unitOfWork.feedbackRepository.UpdateAsync(feedback);

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_UPDATE_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
