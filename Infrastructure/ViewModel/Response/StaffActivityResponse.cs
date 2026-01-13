using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Response
{
   public class StaffActivityResponse
    {
        public class StaffFarmActivityResponse
        {
            public string? Id;
            public string? ActivityType;
            public string? StartDate;
            public string? EndDate;
            public string? Status;
            public string? CropName;

            public string? AccountId;
            public string? StaffName;
            public string? StaffPhone;
            public string? StaffEmail;

            public string? CreatedAt;
            public string? CreatedBy;
            public string? CreatedOn;
            public string? ModifiedAt;

        }
        public class Response_DTO
        {
            public int Status { get; set; }
            public string? Message { get; set; }
            public object? Data { get; set; }
            public Response_DTO(int status, string? message, object? data = null)
            {
                Status = status;
                Message = message;
                Data = data;
            }
        }
    }
}
