using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Response.ProductResponse;

namespace Application.ViewModel.Response
{
    public class OrderResponse
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

        public class OrderResultDTO
        {
            public decimal? TotalPrice { get; set; }
            public string? Phone { get; set; }
            public DateTime? CreatedAt { get; set; }
            public List<ViewProductDTO> OrderItems { get; set; } = new();
        }

        public class CreateOrderResultDTO
        {
            public decimal? TotalPrice { get; set; }
            public string? Email { get; set; }
            public DateTime? CreatedAt { get; set; }
            public List<ViewProductDTO> OrderItems { get; set; } = new();
            public string? PaymentUrl { get; set; } // 🔥 Thêm URL thanh toán vào DTO
        }

    }
}
