using Domain.Enum;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.CategoryResponse;

namespace Infrastructure.ViewModel.Response
{
    public class CropResponse
    {
        public class CropView
        {
            public long CropId { get; set; }
            public string? CropName { get; set; }

            public string? Description { get; set; }

            public string Status { get; set; }
            public string? Origin { get; set; }


        }
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
    }
}
