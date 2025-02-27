using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Response
{
    public class FeedbackResponse
    {
        public class ResponseDTO
        {
            public int Status { get; set; }
            public string? Message { get; set; }
            public object? Data { get; set; }
            public ResponseDTO(int status, string? message, object? data = null)
            {
                Status = status;
                Message = message;
                Data = data;
            }
        }

        public class ViewFeedbackDTO
        {
            public string? Comment { get; set; }

            public int? Rating { get; set; }

            public DateOnly? CreatedAt { get; set; }
            public string? Email { get; set; }
        }
    }
}
