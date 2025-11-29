using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Response
{
    public class IOTLogResponse
    {
        public class IOTLogView
        {
            public long IotLogId { get; set; }
            public long DevicesId { get; set; }

            public string? VariableId { get; set; }

            public string? SensorName { get; set; }

            public double? Value { get; set; }

            public DateTime Timestamp { get; set; }
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
