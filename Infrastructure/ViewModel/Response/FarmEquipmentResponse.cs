using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Response
{
    public class FarmEquipmentResponse
    {
        public class FarmEquipmentView
        {
            public long FarmEquipmentId { get; set; }
            public string? DeviceName { get; set; }
            public string? FarmName { get; set; }
            public DateOnly? AssignDate { get; set; }
            public string? Note { get; set; }
            public string? Status { get; set; }
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
