using Application.Commons;
using Application.Utils;
using AutoMapper;
using Domain;
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
        public OrderServices(IUnitOfWorks unitOfWork, IMapper mapper, JWTUtils jwtUtils)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtUtils = jwtUtils;
        }

        public async Task<ResponseDTO> CreateOrderAsync(CreateOrderDTO request)
        {
            // Lấy người dùng hiện tại (Nếu cần)
            // var user = await _jwtUtils.GetCurrentUserAsync();
            // if (user == null)
            // {
            //     return new ResponseDTO(Const.FAIL_READ_CODE, "No User found.");
            // }

            // Danh sách lỗi
            var errorMessages = new List<string>();

            // Kiểm tra từng sản phẩm trước khi tạo đơn hàng
            foreach (var item in request.OrderItems)
            {
                var product = await _unitOfWork.productRepository.GetProductById(item.ProductId);

                if (product == null)
                {
                    errorMessages.Add($"Product ID {item.ProductId} not found.");
                    continue;
                }

                if (product.Status == 0)
                {
                    errorMessages.Add($"Product ID {item.ProductId} is unavailable.");
                    continue;
                }

                if (product.StockQuantity < item.StockQuantity)
                {
                    errorMessages.Add($"Product ID {item.ProductId} not enough stock.");
                }
            }

            // Nếu có lỗi, hủy tạo đơn hàng
            if (errorMessages.Any())
            {
                return new ResponseDTO(Const.FAIL_CREATE_CODE, "Order create failed.", errorMessages);
            }

            // Nếu không có lỗi, tiến hành tạo đơn hàng
            var order = new Order
            {
                CustomerId = 6,
                TotalPrice = 0, // Tính sau
                Status = 1, // Đang xử lý
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
            };

            await _unitOfWork.orderRepository.AddAsync(order);

            decimal? totalPrice = 0;
            var orderItems = new List<ViewProductDTO>();

            foreach (var item in request.OrderItems)
            {
                var product = await _unitOfWork.productRepository.GetProductById(item.ProductId);

                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.StockQuantity,
                    UnitPrice = product.Price
                };

                await _unitOfWork.orderDetailRepository.AddAsync(orderDetail);

                // Cập nhật số lượng tồn kho
                product.StockQuantity -= item.StockQuantity;

                // Nếu sản phẩm hết hàng, cập nhật trạng thái thành 0 (Unavailable)
                if (product.StockQuantity == 0)
                {
                    product.Status = 0;
                }

                await _unitOfWork.productRepository.UpdateAsync(product);

                totalPrice += (product.Price ?? 0) * item.StockQuantity;

                // Thêm sản phẩm vào danh sách kết quả
                orderItems.Add(new ViewProductDTO
                {
                    ProductName = product.ProductName ?? "Unknown",
                    Price = product.Price,
                    StockQuantity = item.StockQuantity
                });
            }

            order.TotalPrice = totalPrice;

            await _unitOfWork.orderRepository.UpdateAsync(order);

            // Tạo DTO kết quả
            var orderResult = new OrderResultDTO
            {
                TotalPrice = order.TotalPrice,
                OrderItems = orderItems
            };

            return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "Create Order Success.", orderResult);
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
    }
}
