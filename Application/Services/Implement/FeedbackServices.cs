using Application.Commons;
using Application.Interfaces;
using Application.Utils;
using AutoMapper;
using Domain.Enum;
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
        private readonly JWTUtils _jwtUtils;
        public FeedbackServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper, JWTUtils jwtUtils)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
            _jwtUtils = jwtUtils;
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
                var user = await _jwtUtils.GetCurrentUserAsync();
                if (user == null)
                    throw new Exception("User not found");
                var orderDetail = await _unitOfWork.orderDetailRepository.GetByIdAsync(request.OrderDetailId);
                if (orderDetail == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Order detail not found.");
                }
                // Ánh xạ từ DTO sang Entity
                var feedback = _mapper.Map<Feedback>(request);
                feedback.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
                feedback.CustomerId = user.AccountId;
                feedback.Status = Domain.Enum.Status.ACTIVE;

                await _unitOfWork.feedbackRepository.AddAsync(feedback);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "Feedback created!");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> UpdateFeedbackById(long feedbackId, CreateFeedbackDTO request)
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

        public async Task<ResponseDTO> UpdateFeedbackStatusById(long feedbackId)
        {
            try
            {
                var feedback = await _unitOfWork.feedbackRepository.GetByIdAsync(feedbackId);
                if (feedback == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Feedback not found !");
                }
                feedback.Status = feedback.Status == Domain.Enum.Status.ACTIVE
                    ? Domain.Enum.Status.DEACTIVATED
                    : Domain.Enum.Status.ACTIVE;
                // Lưu các thay đổi vào cơ sở dữ liệu
                await _unitOfWork.feedbackRepository.UpdateAsync(feedback);

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_UPDATE_MSG);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> GetFeedbackByProductIdAsync(long productId)
        {
            var getUser = await _jwtUtils.GetCurrentUserAsync();
            var feedbacks = await _unitOfWork.feedbackRepository.GetByProductIdAsync(productId);
            var result = _mapper.Map<List<ViewFeedbackDTO>>(feedbacks);
            if (feedbacks == null || !feedbacks.Any())
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, "There are do not any feedback for this product.");
            }
            else if (getUser == null || !getUser.Role.Equals(Roles.Staff))
            {
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result.Where(x => x.Status == Status.ACTIVE.ToString()).ToList());
            }
            else
            {
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }



            // Map dữ liệu sang DTO


        }
        
        public async Task<ResponseDTO> GetFeedbackByOrderIdAsync(long orderId)
        {
            var feedbacks = await _unitOfWork.feedbackRepository.GetByOrderIdAsync(orderId);
            var result = _mapper.Map<List<ViewFeedbackDTO>>(feedbacks);
            if (feedbacks == null || !feedbacks.Any())
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, "There are do not any feedback for this Order.");
            }

            return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);

        }
    }
}
