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
            public int FarmActivitiesId { get; set; }
            public int? ActivityType { get; set; }

            public string? StartDate { get; set; }

            public string? EndDate { get; set; }

            public string? Status { get; set; }
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
