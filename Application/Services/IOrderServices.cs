﻿using Domain.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Request.OrderRequest;
using static Application.ViewModel.Response.OrderResponse;

namespace Application.Services
{
    public interface IOrderServices
    {
        Task<ResponseDTO> CreateOrderAsync(CreateOrderDTO request, HttpContext context);
        Task<ResponseDTO> GetAllOrderAsync(int pageIndex, int pageSize, Status? status);
        Task<ResponseDTO> GetAllOrderByCustomerIdAsync(long customerId, int pageIndex, int pageSize, Status? status);
        Task<ResponseDTO> GetOrderByIdAsync(long orderId);
        Task<ResponseDTO> UpdateOrderStatusAsync(long orderId, UpdateOrderStatusDTO request);
        Task<long> GetOrderIdByOrderIdAsync(long orderId);
        Task<ResponseDTO> GetAllOrderByCustomerNameAsync(string customerName, int pageIndex, int pageSize);
        Task<ResponseDTO> GetAllOrderByCurrentCustomerAsync(int pageIndex, int pageSize, Status? status);
        Task<ResponseDTO> SearchOrderbyEmail(string email, int pageIndex, int pageSize);
        Task<ResponseDTO> SearchOrderbyCreateDate(DateOnly date, int pageIndex, int pageSize);

    }
}
