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

            public int? ActivityType { get; set; }

            public DateOnly? StartDate { get; set; }

            public DateOnly? EndDate { get; set; }

            public Status? Status { get; set; }
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
