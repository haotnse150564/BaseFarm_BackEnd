﻿using Domain.Enum;
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
            public long OrderId { get; set; }
            public decimal? TotalPrice { get; set; }
            public string? Email { get; set; }
            public List<long> OrderDetailIds { get; set; } = new();
            public DateTime? CreatedAt { get; set; }
            public Status? Status { get; set; }
            public string? ShippingAddress { get; set; }
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

        public class OrderDetailDTO
        {
            public long ProductId { get; set; } // ID của Product
            public string ProductName { get; set; } = string.Empty; 
            public decimal? UnitPrice { get; set; }
            public int? Quantity { get; set; }
        }

    }
}
