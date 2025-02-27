using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.FeedbackResponse;

namespace Application.Services
{
    public interface IFeedbackSevices
    {
        Task<ResponseDTO> GetAllFeedbackAsync(int pageIndex, int pageSize);
    }
}
