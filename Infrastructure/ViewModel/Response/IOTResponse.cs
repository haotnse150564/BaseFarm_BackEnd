using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Response
{
    public class IOTResponse
    {
        public class IOTView
        {
            public string? DeviceName { get; set; }

            public string? DeviceType { get; set; }

            public Status? Status { get; set; }

            public string? SensorValue { get; set; }

            public string? Unit { get; set; }

            public DateOnly? LastUpdate { get; set; }

            public DateOnly? ExpiryDate { get; set; }

            public long FarmDetailsId { get; set; }
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
