using Application.Commons;
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
            await _unitOfWork.SaveChangesAsync(); // 🔥 Save để cập nhật OrderId

            decimal totalPrice = 0;
            var orderItems = new List<OrderDetail>();
            var orderedProducts = new List<Product>(); // 🔥 Lưu danh sách sản phẩm đã đặt

            foreach (var item in request.OrderItems)
            {
                var product = await _unitOfWork.productRepository.GetProductById(item.ProductId);
                if (product == null) continue;

                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.StockQuantity,
                    UnitPrice = product.Price
                };

                await _unitOfWork.orderDetailRepository.AddAsync(orderDetail);
                product.StockQuantity -= item.StockQuantity;
                if (product.StockQuantity == 0) product.Status = 0;
                await _unitOfWork.productRepository.UpdateAsync(product);
                totalPrice += (decimal)((product.Price ?? 0) * item.StockQuantity);

                orderItems.Add(orderDetail);
                orderedProducts.Add(product); // 🔥 Thêm sản phẩm vào danh sách
            }

            order.TotalPrice = totalPrice;
            await _unitOfWork.orderRepository.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync(); // 🔥 Lưu thay đổi sau khi cập nhật OrderDetail & Product

            // 🔥 Tạo URL thanh toán VnPay
            var paymentModel = new PaymentInformationModel
            {
                Amount = (double)totalPrice,
                OrderDescription = $"Thanh toán đơn hàng #{order.OrderId}",
                OrderType = "billpayment",
                Name = "IOT Base Farm"
            };

            var paymentUrl = _vnPayService.CreatePaymentUrl(paymentModel, context);

            // 🔥 Ánh xạ sang CreateOrderResultDTO
            var orderResultDTO = _mapper.Map<CreateOrderResultDTO>(order);
            orderResultDTO.OrderItems = _mapper.Map<List<ViewProductDTO>>(orderedProducts); // 🔥 Mapping từ Product thay vì OrderDetail
            orderResultDTO.PaymentUrl = paymentUrl;

            return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "Order created. Redirect to payment.", orderResultDTO);
        }



        //public async Task<ResponseDTO> CreateOrderAsync(CreateOrderDTO request)
        //{
        //    // Lấy người dùng hiện tại (Nếu cần)
        //    // var user = await _jwtUtils.GetCurrentUserAsync();
        //    // if (user == null)
        //    // {
        //    //     return new ResponseDTO(Const.FAIL_READ_CODE, "No User found.");
        //    // }

        //    // Danh sách lỗi
        //    var errorMessages = new List<string>();

        //    // Kiểm tra từng sản phẩm trước khi tạo đơn hàng
        //    foreach (var item in request.OrderItems)
        //    {
        //        var product = await _unitOfWork.productRepository.GetProductById(item.ProductId);

        //        if (product == null)
        //        {
        //            errorMessages.Add($"Product ID {item.ProductId} not found.");
        //            continue;
        //        }

        //        if (product.Status == 0)
        //        {
        //            errorMessages.Add($"Product ID {item.ProductId} is unavailable.");
        //            continue;
        //        }

        //        if (product.StockQuantity < item.StockQuantity)
        //        {
        //            errorMessages.Add($"Product ID {item.ProductId} not enough stock.");
        //        }
        //    }

        //    // Nếu có lỗi, hủy tạo đơn hàng
        //    if (errorMessages.Any())
        //    {
        //        return new ResponseDTO(Const.FAIL_CREATE_CODE, "Order create failed.", errorMessages);
        //    }

        //    // Nếu không có lỗi, tiến hành tạo đơn hàng
        //    var order = new Order
        //    {
        //        CustomerId = 6,
        //        TotalPrice = 0, // Tính sau
        //        Status = 1, // Đang xử lý
        //        CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
        //    };

        //    await _unitOfWork.orderRepository.AddAsync(order);

        //    decimal? totalPrice = 0;
        //    var orderItems = new List<ViewProductDTO>();

        //    foreach (var item in request.OrderItems)
        //    {
        //        var product = await _unitOfWork.productRepository.GetProductById(item.ProductId);

        //        var orderDetail = new OrderDetail
        //        {
        //            OrderId = order.OrderId,
        //            ProductId = item.ProductId,
        //            Quantity = item.StockQuantity,
        //            UnitPrice = product.Price
        //        };

        //        await _unitOfWork.orderDetailRepository.AddAsync(orderDetail);

        //        // Cập nhật số lượng tồn kho
        //        product.StockQuantity -= item.StockQuantity;

        //        // Nếu sản phẩm hết hàng, cập nhật trạng thái thành 0 (Unavailable)
        //        if (product.StockQuantity == 0)
        //        {
        //            product.Status = 0;
        //        }

        //        await _unitOfWork.productRepository.UpdateAsync(product);

        //        totalPrice += (product.Price ?? 0) * item.StockQuantity;

        //        // Thêm sản phẩm vào danh sách kết quả
        //        orderItems.Add(new ViewProductDTO
        //        {
        //            ProductName = product.ProductName ?? "Unknown",
        //            Price = product.Price,
        //            StockQuantity = item.StockQuantity
        //        });
        //    }

        //    order.TotalPrice = totalPrice;

        //    await _unitOfWork.orderRepository.UpdateAsync(order);

        //    // Tạo DTO kết quả
        //    var orderResult = new OrderResultDTO
        //    {
        //        TotalPrice = order.TotalPrice,
        //        OrderItems = orderItems
        //    };

        //    return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "Create Order Success.", orderResult);
        //}

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
