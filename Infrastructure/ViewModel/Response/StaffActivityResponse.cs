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
            public string? Id { get; set; }
            public string? ActivityType { get; set; }
            public string? StartDate { get; set; }
            public string? EndDate { get; set; }
            public string? Status { get; set; }
            public string? CropName { get; set; }

            public string? AccountId { get; set; }
            public string? StaffName { get; set; }
            public string? StaffPhone { get; set; }
            public string? StaffEmail { get; set; }
            public string? IndividualStatus { get; set; }

            public string? CreatedAt { get; set; }
            public string? CreatedBy { get; set; }
            public string? CreatedOn { get; set; }
            public string? ModifiedAt { get; set; }
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
