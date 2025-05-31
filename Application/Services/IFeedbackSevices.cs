using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Request.FeedbackRequest;
using static Infrastructure.ViewModel.Response.FeedbackResponse;

namespace Application.Services
{
    public interface IFeedbackSevices
    {
        Task<ResponseDTO> GetAllFeedbackAsync(int pageIndex, int pageSize);
        Task<ResponseDTO> CreateFeedbackAsync(CreateFeedbackDTO request);
        Task<ResponseDTO> UpdateFeedbackById(long feedbackId, CreateFeedbackDTO request);
        Task<ResponseDTO> UpdateFeedbackStatusById(long feedbackId);
        Task<ResponseDTO> GetFeedbackByProductIdAsync(long productId);
        Task<ResponseDTO> GetFeedbackByOrderIdAsync(long orderId);
        Task<ResponseDTO> GetFeedbackByOrderDetailIdAsync(long orderDetailId);
    }
}
