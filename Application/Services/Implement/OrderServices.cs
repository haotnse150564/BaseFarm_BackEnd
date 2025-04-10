using Application.Utils;
using Application.ViewModel.Request;
using AutoMapper;
using Domain.Enum;
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
            try
            {
                var user = await _jwtUtils.GetCurrentUserAsync();
                using var transaction = await _unitOfWork.BeginTransactionAsync();
                var errorMessages = new List<string>();

                var productList = new Dictionary<long, Product>();

                foreach (var item in request.OrderItems)
                {
                    var product = await _unitOfWork.productRepository.GetProductById(item.ProductId);
                    if (product == null)
                        errorMessages.Add($"Product ID {item.ProductId} not found.");
                    else if (product.Status == 0)
                        errorMessages.Add($"Product ID {item.ProductId} is unavailable.");
                    else if (product.StockQuantity < item.StockQuantity)
                        errorMessages.Add($"Product ID {item.ProductId} not enough stock.");
                    else
                        productList[item.ProductId] = product; // Lưu lại để tránh truy vấn lại
                }

                if (errorMessages.Any())
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "Order creation failed.", errorMessages);
                }

                var order = new Order
                {
                    CustomerId = user.AccountId,
                    TotalPrice = 0,
                    Status = Status.PENDING, // Đang xử lý
                    CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                    ShippingAddress = request.ShippingAddress,
                };

                await _unitOfWork.orderRepository.AddAsync(order);
                await _unitOfWork.SaveChangesAsync(); // 🔥 Đảm bảo OrderId đã cập nhật

                decimal totalPrice = 0;
                var orderItems = new List<OrderDetail>();

                foreach (var item in request.OrderItems)
                {
                    var product = productList[item.ProductId];

                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.StockQuantity,
                        UnitPrice = product.Price
                    };

                    await _unitOfWork.orderDetailRepository.AddAsync(orderDetail);
                    orderItems.Add(orderDetail); // 🔥 Lưu ngay OrderDetail

                    product.StockQuantity -= item.StockQuantity;
                    if (product.StockQuantity == 0) product.Status = 0;
                    await _unitOfWork.productRepository.UpdateAsync(product);
                    totalPrice += (decimal)((product.Price ?? 0) * item.StockQuantity);
                }

                order.TotalPrice = totalPrice;
                await _unitOfWork.orderRepository.UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync(); // 🔥 Lưu thay đổi Order

                // Commit transaction sau khi tất cả dữ liệu được thêm thành công
                await transaction.CommitAsync();

                var paymentModel = new PaymentInformationModel
                {
                    OrderId = order.OrderId, // Sử dụng OrderId của hệ thống bạn
                    Amount = (double)totalPrice,
                    OrderDescription = $"Thanh toán đơn hàng #{order.OrderId}",
                    OrderType = "billpayment",
                    Name = "IOT Base Farm"
                };

                var paymentUrl = _vnPayService.CreatePaymentUrl(paymentModel, context);


                // 🔥 Mapping lại OrderDetail sang OrderDetailDTO có ProductName
                var orderDetailDTOs = orderItems.Select(od => new OrderDetailDTO
                {
                    ProductId = od.ProductId,
                    ProductName = productList[od.ProductId].ProductName, // ✅ Lấy ProductName từ danh sách đã lưu
                    UnitPrice = od.UnitPrice,
                    Quantity = od.Quantity
                }).ToList();

                var orderResultDTO = _mapper.Map<CreateOrderResultDTO>(order);
                orderResultDTO.OrderItems = _mapper.Map<List<ViewProductDTO>>(orderDetailDTOs);
                orderResultDTO.PaymentUrl = paymentUrl;

                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "Order created. Redirect to payment.", orderResultDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, "An error occurred while creating the order.", ex.Message);
            }
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

        public async Task<ResponseDTO> GetAllOrderByCurrentCustomerAsync(int pageIndex, int pageSize)
        {
            try
            {
                var user = await _jwtUtils.GetCurrentUserAsync();
                if (user == null)
                {
                    return new ResponseDTO(Const.ERROR_EXCEPTION, "No Login Session Found!");
                }

                var listOrder = await _unitOfWork.orderRepository.GetOrdersByCustomerIdAsync(user.AccountId, pageIndex, pageSize);

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

        public async Task<ResponseDTO> GetAllOrderByCustomerNameAsync(string customerName, int pageIndex, int pageSize)
        {
            try
            {
                var listOrder = await _unitOfWork.orderRepository.GetOrdersByCustomerNameAsync(customerName, pageIndex, pageSize);

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

    }
}
