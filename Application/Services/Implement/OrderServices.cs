using Application.Commons;
using Application.Utils;
using Application.ViewModel.Request;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly CheckDate _checkDate;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<OrderNotificationHub> _hubContext;
        public OrderServices(IUnitOfWorks unitOfWork, IMapper mapper, JWTUtils jwtUtils, IVnPayService vnPayService, CheckDate checkDate, IConfiguration configuration, IHubContext<OrderNotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtUtils = jwtUtils;
            _vnPayService = vnPayService;
            _checkDate = checkDate;
            _configuration = configuration;
            _hubContext = hubContext;
        }

        public async Task<ResponseDTO> CreateOrderAsync(CreateOrderDTO request, HttpContext context)
        {
            var strategy = _unitOfWork.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _unitOfWork.Database.BeginTransactionAsync();
                try
                {
                    var user = await _jwtUtils.GetCurrentUserAsync();

                    var errorMessages = new List<string>();
                    var productList = new Dictionary<long, Product>();

                    // Validate products
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
                            productList[item.ProductId] = product;
                    }

                    if (errorMessages.Any())
                    {
                        return new ResponseDTO(Const.FAIL_CREATE_CODE, "Order creation failed.", errorMessages);
                    }

                    // Tạo Order
                    var order = new Order
                    {
                        CustomerId = user.AccountId,
                        TotalPrice = 0,
                        Status = PaymentStatus.PENDING,
                        CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                        ShippingAddress = request.ShippingAddress,
                    };

                    await _unitOfWork.orderRepository.AddAsync(order);
                    await _unitOfWork.SaveChangesAsync(); // Lưu để lấy OrderId

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
                        orderItems.Add(orderDetail);

                        // Cập nhật tồn kho (bạn đang comment, nên mình bỏ comment lại)
                        //product.StockQuantity -= item.StockQuantity;
                        //if (product.StockQuantity == 0) product.Status = 0;

                        await _unitOfWork.productRepository.UpdateAsync(product);

                        totalPrice += (decimal)((product.Price ?? 0) * item.StockQuantity);
                    }

                    // Cập nhật tổng tiền
                    order.TotalPrice = totalPrice;
                    await _unitOfWork.orderRepository.UpdateAsync(order);
                    await _unitOfWork.SaveChangesAsync();

                    // Commit transaction
                    await transaction.CommitAsync();

                    // Tạo payment URL và mapping DTO
                    var baseUrl = _configuration["BaseUrl"];
                    var paymentUrl = $"{baseUrl}/api/vnpay/redirect?orderId={order.OrderId}";

                    var orderDetailDTOs = orderItems.Select(od => new OrderDetailDTO
                    {
                        ProductId = od.ProductId,
                        Images = productList[od.ProductId].Images,
                        ProductName = productList[od.ProductId].ProductName,
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
                    await transaction.RollbackAsync();
                    // Log lỗi nếu cần
                    return new ResponseDTO(Const.ERROR_EXCEPTION, "An error occurred while creating the order.", ex.Message);
                }
            });
        }

        public async Task<ResponseDTO> GetAllOrderAsync(int pageIndex, int pageSize, PaymentStatus? status)
        {
            try
            {
                var listOrder = await _unitOfWork.orderRepository.GetAllOrdersAsync(pageIndex, pageSize, status);

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


        public async Task<ResponseDTO> GetAllOrderByCustomerIdAsync(long customerId, int pageIndex, int pageSize, PaymentStatus? status)
        {
            try
            {
                var listOrder = await _unitOfWork.orderRepository.GetOrdersByCustomerIdAsync(customerId, pageIndex, pageSize, status);

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

        public async Task<ResponseDTO> GetAllOrderByCurrentCustomerAsync(int pageIndex, int pageSize, PaymentStatus? status)
        {
            try
            {
                var user = await _jwtUtils.GetCurrentUserAsync();
                if (user == null)
                {
                    return new ResponseDTO(Const.ERROR_EXCEPTION, "No Login Session Found!");
                }

                var listOrder = await _unitOfWork.orderRepository
                    .GetOrdersByCustomerIdAsync(user.AccountId, pageIndex, pageSize, status);

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


        public async Task<ResponseDTO> UpdateOrderDeliveryStatusAsync(long orderId)
        {
            try
            {
                var order = await _unitOfWork.orderRepository.GetOrderById(orderId);
                if (order == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Order not found !");
                }
                
                order.Status = PaymentStatus.DELIVERED;

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _unitOfWork.orderRepository.UpdateAsync(order);
                bool shouldNotify = order.Status == PaymentStatus.DELIVERED;
                // Gửi thông báo real-time nếu cần
                if (shouldNotify)
                {
                    var notification = new OrderStatusNotification
                    {
                        OrderId = order.OrderId,
                        Message = $"Your order #{order.OrderId} is on the way",
                        Status = "DELIVERED"
                    };

                    // Gửi riêng cho khách hàng sở hữu đơn hàng này
                    await _hubContext.Clients
                        .Group($"User_{order.CustomerId}")  
                        .SendAsync("ReceiveOrderStatusUpdate", notification);
                }

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

        public async Task<ResponseDTO> SearchOrderbyEmail(string email, int pageIndex, int pageSize, PaymentStatus? status)
        {
            try
            {
                var listOrder = await _unitOfWork.orderRepository.SearchOrdersByEmailAsync(email, status);

                if (listOrder == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Order found.");
                }
                var result = _mapper.Map<List<OrderResultDTO>>(listOrder);
                var pagination = new Pagination<OrderResultDTO>
                {
                    TotalItemCount = result.Count(),
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    Items = result.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
                };
                result.OrderBy(o => o.CreatedAt);
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, pagination);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }

        }

        public async Task<ResponseDTO> SearchOrderbyCreateDate(DateOnly date, int pageIndex, int pageSize)
        {
            try
            {
                if (!_checkDate.IsValidDate(date))
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Date is not valid.");
                }
                var listOrder = await _unitOfWork.orderRepository.SearchOrdersByDateAsync(date);

                if (listOrder == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Order found.");
                }
                var result = _mapper.Map<List<OrderResultDTO>>(listOrder);
                result.OrderBy(o => o.CreatedAt);
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> UpdateOrderCompletedStatusAsync(long orderId)
        {
            try
            {
                var order = await _unitOfWork.orderRepository.GetOrderById(orderId);
                if (order == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Order not found !");
                }

                order.Status = PaymentStatus.COMPLETED;

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _unitOfWork.orderRepository.UpdateAsync(order);

                bool shouldNotify = order.Status == PaymentStatus.COMPLETED;
                // Gửi thông báo real-time nếu cần
                if (shouldNotify)
                {
                    var notification = new OrderStatusNotification
                    {
                        OrderId = order.OrderId,
                        Message = $"Your order #{order.OrderId} has been completed successfully",
                        Status = "COMPLETED"
                    };

                    // Gửi riêng cho khách hàng sở hữu đơn hàng này
                    await _hubContext.Clients
                        .Group($"User_{order.CustomerId}")
                        .SendAsync("ReceiveOrderStatusUpdate", notification);
                }
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, "Change Status Succeed");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> UpdateOrderCancelStatusAsync(long orderId)
        {
            try
            {
                var order = await _unitOfWork.orderRepository.GetOrderById(orderId);
                if (order == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Order not found!");
                }

                if (order.Status == PaymentStatus.CANCELLED)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Order is already cancelled.");
                }

                // Đổi trạng thái sang CANCELLED
                order.Status = PaymentStatus.CANCELLED;
                await _unitOfWork.orderRepository.UpdateAsync(order);

                //// 🔁 Hoàn lại số lượng sản phẩm nếu đơn bị hủy
                //var orderDetails = await _unitOfWork.orderDetailRepository.GetOrderDetailsByOrderId(order.OrderId);
                //foreach (var item in orderDetails)
                //{
                //    var product = await _unitOfWork.productRepository.GetByIdAsync(item.ProductId);
                //    if (product != null)
                //    {
                //        product.StockQuantity += item.Quantity ?? 0;

                //        // Nếu sản phẩm trước đó hết hàng thì cập nhật lại trạng thái
                //        if (product.StockQuantity > 0)
                //        {
                //            product.Status = ProductStatus.ACTIVE;
                //        }

                //        await _unitOfWork.productRepository.UpdateAsync(product);
                //    }
                //}

                await _unitOfWork.SaveChangesAsync();
                bool shouldNotify = order.Status == PaymentStatus.CANCELLED;
                // Gửi thông báo real-time nếu cần
                if (shouldNotify)
                {
                    var notification = new OrderStatusNotification
                    {
                        OrderId = order.OrderId,
                        Message = $"Your order #{order.OrderId} has been cancelled.",
                        Status = "CANCELLED"
                    };

                    // Gửi riêng cho khách hàng sở hữu đơn hàng này
                    await _hubContext.Clients
                        .Group($"User_{order.CustomerId}")
                        .SendAsync("ReceiveOrderStatusUpdate", notification);
                }
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, "Order cancelled and stock quantity restored.");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }


        public async Task<ResponseDTO> CreateOrderPaymentAsync(long orderId, HttpContext context)
        {
            try
            {
                var order = await _unitOfWork.orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    return new ResponseDTO(Const.ERROR_EXCEPTION, $"Order ID {orderId} not found.");
                }

                if (order.Status != PaymentStatus.PENDING && order.Status != PaymentStatus.UNDISCHARGED)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, $"Order ID {orderId} is not in a valid state for payment.");
                }

                // ❗️Chỉ trừ stock nếu order đang ở trạng thái UNDISHCHARGED
                if (order.Status == PaymentStatus.UNDISCHARGED)
                {
                    using var transaction = await _unitOfWork.BeginTransactionAsync();

                    var orderDetails = await _unitOfWork.orderDetailRepository.GetOrderDetailsByOrderId(orderId);

                    // 🔥 Cập nhật số lượng tồn kho sau khi thanh toán thành công
                    foreach (var detail in order.OrderDetails)
                    {
                        // Trừ tồn kho từng sản phẩm theo số lượng đặt mua
                        await _unitOfWork.productRepository.UpdateStockByOrderAsync(detail.ProductId, detail.Quantity ?? 0);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();
                }

                // ✅ Tạo URL thanh toán cho cả hai trạng thái
                var paymentModel = new PaymentInformationModel
                {
                    OrderId = order.OrderId,
                    Amount = (double)(order.TotalPrice ?? 0),
                    OrderDescription = $"Thanh toán đơn hàng #{order.OrderId}",
                    OrderType = "billpayment",
                    Name = "IOT Base Farm"
                };

                var paymentUrl = _vnPayService.CreatePaymentUrl(paymentModel, context);

                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "Payment URL generated successfully.", new
                {
                    OrderId = order.OrderId,
                    PaymentUrl = paymentUrl
                });
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, "An error occurred while creating payment.", ex.Message);
            }
        }

        public async Task UpdateStockAfterOrderAsync(Order order)
        {
            foreach (var detail in order.OrderDetails)
            {
                // Trừ tồn kho từng sản phẩm theo số lượng đặt mua
                await _unitOfWork.productRepository.UpdateStockByOrderAsync(detail.ProductId, detail.Quantity ?? 0);
            }
        }
    }
}
