using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.Response
{
    public class ProductResponse
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

        public class ViewProductDTO
        {
            public long ProductId { get; set; }
            public string? ProductName { get; set; }
            public decimal? Price { get; set; }
            public int? StockQuantity { get; set; }
            public string? Images { get; set; }
            public string? Description { get; set; }
            public Status? Status { get; set; }
            public DateOnly? CreatedAt { get; set; }
            public DateOnly? UpdatedAt { get; set; }
            public long CategoryId { get; set; }
        }

        public class ProductDetailDTO
        {
            public string? Images { get; set; }
            public string? ProductName { get; set; }
            public decimal? Price { get; set; }
            public int? StockQuantity { get; set; }
            public string? Description { get; set; }
            public long CategoryId { get; set; }
            public string? CategoryName { get; set; }
        }


    }
}
