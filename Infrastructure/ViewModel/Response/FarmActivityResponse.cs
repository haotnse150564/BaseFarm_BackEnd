using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.AccountResponse;

namespace Infrastructure.ViewModel.Response
{
    public class FarmActivityResponse
    {
        public class FarmActivityView
        {
            public string? FarmName { get; set; }

            public string? Location { get; set; }

            public DateOnly? CreatedAt { get; set; }

            public DateOnly? UpdatedAt { get; set; }

            public ViewAccount Account { get; set; }
        }
        public class ResponseDTO
        {
            public int Status { get; set; }
            public string Message { get; set; }
            public ResponseDTO(int status, string message)
            {
                Status = status;
                Message = message;
            }
        }
    }
}
