﻿using Application.Commons;
using Application.Utils;
using Application.ViewModel.Request;
using AutoMapper;
using Domain;
using Domain.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Request.OrderRequest;
using static Application.ViewModel.Response.OrderResponse;
using static Application.ViewModel.Response.ProductResponse;
using ResponseDTO = Application.ViewModel.Response.OrderResponse.ResponseDTO;

namespace Application.Services.Implement
{
    public class OrderServices : IOrderServices
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly IMapper _mapper;
        private readonly JWTUtils _jwtUtils;
        private readonly IVnPayService _vnPayService;
        public OrderServices(IUnitOfWorks unitOfWork, IMapper mapper, JWTUtils jwtUtils, IVnPayService vnPayService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtUtils = jwtUtils;
            _vnPayService = vnPayService;
        }

        public async Task<ResponseDTO> CreateOrderAsync(CreateOrderDTO request, HttpContext context)
        {
            var user = await _jwtUtils.GetCurrentUserAsync();
            var errorMessages = new List<string>();

            foreach (var item in request.OrderItems)
            {
                var product = await _unitOfWork.productRepository.GetProductById(item.ProductId);
                if (product == null) errorMessages.Add($"Product ID {item.ProductId} not found.");
                else if (product.Status == 0) errorMessages.Add($"Product ID {item.ProductId} is unavailable.");
                else if (product.StockQuantity < item.StockQuantity) errorMessages.Add($"Product ID {item.ProductId} not enough stock.");
            }

            if (errorMessages.Any())
            {
                return new ResponseDTO(Const.FAIL_CREATE_CODE, "Order creation failed.", errorMessages);
            }

            var order = new Order
            {
                CustomerId = user.AccountId,
                TotalPrice = 0,
                Status = 1, // Đang xử lý
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
            };

            await _unitOfWork.orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync(); // 🔥 Đảm bảo OrderId đã cập nhật

            decimal totalPrice = 0;
            var orderItems = new List<OrderDetail>();

            foreach (var item in request.OrderItems)
            {
                var product = await _unitOfWork.productRepository.GetProductById(item.ProductId);
                if (product == null) continue;

                if (product.StockQuantity >= item.StockQuantity)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.StockQuantity,
                        UnitPrice = product.Price
                    };

                    await _unitOfWork.orderDetailRepository.AddAsync(orderDetail);
                    await _unitOfWork.SaveChangesAsync(); // 🔥 Lưu ngay OrderDetail

                    product.StockQuantity -= item.StockQuantity;
                    if (product.StockQuantity == 0) product.Status = 0;
                    await _unitOfWork.productRepository.UpdateAsync(product);
                    await _unitOfWork.SaveChangesAsync(); // 🔥 Lưu ngay sản phẩm cập nhật

                    totalPrice += (decimal)((product.Price ?? 0) * item.StockQuantity);
                    orderItems.Add(orderDetail);
                }
                else
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, $"Not enough stock for product {item.ProductId}.", null);
                }
            }

            order.TotalPrice = totalPrice;
            await _unitOfWork.orderRepository.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync(); // 🔥 Lưu thay đổi Order

            var paymentModel = new PaymentInformationModel
            {
                Amount = (double)totalPrice,
                OrderDescription = $"Thanh toán đơn hàng #{order.OrderId}",
                OrderType = "billpayment",
                Name = "IOT Base Farm"
            };

            var paymentUrl = _vnPayService.CreatePaymentUrl(paymentModel, context);

            var orderResultDTO = _mapper.Map<CreateOrderResultDTO>(order);
            orderResultDTO.OrderItems = _mapper.Map<List<ViewProductDTO>>(orderItems);
            orderResultDTO.PaymentUrl = paymentUrl;

            return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "Order created. Redirect to payment.", orderResultDTO);
        }


        public async Task<ResponseDTO> GetAllOrderAsync(int pageIndex, int pageSize)
        {
            try
            {
                var listOrder = await _unitOfWork.orderRepository.GetAllOrdersAsync(pageIndex, pageSize);

                if (listOrder == null || !listOrder.Items.Any())
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Order found.");
                }

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, listOrder);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> GetAllOrderByCustomerIdAsync(long customerId, int pageIndex, int pageSize)
        {
            try
            {
                var listOrder = await _unitOfWork.orderRepository.GetOrdersByCustomerIdAsync(customerId, pageIndex, pageSize);

                if (listOrder == null || !listOrder.Items.Any())
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Order found.");
                }

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, listOrder);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> GetOrderByIdAsync(long orderId)
        {
            try
            {
                var getOrder = await _unitOfWork.orderRepository.GetOrderById(orderId);

                if (getOrder == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Order found.");
                }

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, getOrder);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<long> GetOrderIdByOrderIdAsync(long orderId)
        {
            try
            {
                var getOrder = await _unitOfWork.orderRepository.GetOrderById(orderId);

                if (getOrder == null)
                {
                    return -1;
                }

                return getOrder.OrderId;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }


        public async Task<ResponseDTO> UpdateOrderStatusAsync(long orderId, UpdateOrderStatusDTO request)
        {
            try
            {
                var order = await _unitOfWork.orderRepository.GetOrderById(orderId);
                if (order == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Order not found !");
                }

                // Sử dụng AutoMapper để ánh xạ thông tin từ DTO
                var updatedOrderStatus = _mapper.Map(request, order);

                var result = _mapper.Map<UpdateOrderStatusDTO>(updatedOrderStatus);

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _unitOfWork.orderRepository.UpdateAsync(updatedOrderStatus);

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, "Change Status Succeed");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
