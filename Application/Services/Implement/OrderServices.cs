using Application.Commons;
using Application.Utils;
using Application.ViewModel.Request;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Microsoft.AspNetCore.Http;
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
        public OrderServices(IUnitOfWorks unitOfWork, IMapper mapper, JWTUtils jwtUtils, IVnPayService vnPayService, CheckDate checkDate)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtUtils = jwtUtils;
            _vnPayService = vnPayService;
            _checkDate = checkDate;
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
                    Status = PaymentStatus.PENDING, // Đang xử lý
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
                if(!_checkDate.IsValidDate(date))
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Date is not valid.");
                }
                var listOrder = await _unitOfWork.orderRepository.SearchOrdersByDateAsync(date);

                if (listOrder == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Order found.");
                }
                var result = _mapper.Map<List<OrderResultDTO>>(listOrder);
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
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Order not found !");
                }

                order.Status = PaymentStatus.CANCELLED;

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _unitOfWork.orderRepository.UpdateAsync(order);

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, "Change Status Succeed");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        //public async Task<ResponseDTO> CreateOrderPaymentAsync(long orderId, HttpContext context)
        //{
        //    try
        //    {
        //        var order = await _unitOfWork.orderRepository.GetByIdAsync(orderId);
        //        if (order == null)
        //        {
        //            return new ResponseDTO(Const.ERROR_EXCEPTION, $"Order ID {orderId} not found.");
        //        }

        //        if (order.Status != Status.PENDING && order.Status != Status.UNDISCHARGED)
        //        {
        //            return new ResponseDTO(Const.FAIL_READ_CODE, $"Order ID {orderId} is not in a valid state for payment.");
        //        }

        //        var paymentModel = new PaymentInformationModel
        //        {
        //            OrderId = order.OrderId,
        //            Amount = (double)(order.TotalPrice ?? 0),
        //            OrderDescription = $"Thanh toán đơn hàng #{order.OrderId}",
        //            OrderType = "billpayment",
        //            Name = "IOT Base Farm"
        //        };

        //        var paymentUrl = _vnPayService.CreatePaymentUrl(paymentModel, context);

        //        return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "Payment URL generated successfully.", new
        //        {
        //            OrderId = order.OrderId,
        //            PaymentUrl = paymentUrl
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseDTO(Const.ERROR_EXCEPTION, "An error occurred while creating payment.", ex.Message);
        //    }
        //}

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

                // Bắt đầu transaction
                using var transaction = await _unitOfWork.BeginTransactionAsync();

                // Lấy danh sách OrderDetail
                var orderDetails = await _unitOfWork.orderDetailRepository.GetOrderDetailsByOrderId(orderId);

                var productList = new Dictionary<long, Product>();
                var errorMessages = new List<string>();

                foreach (var item in orderDetails)
                {
                    var product = await _unitOfWork.productRepository.GetProductById(item.ProductId);
                    if (product == null)
                    {
                        errorMessages.Add($"Product ID {item.ProductId} not found.");
                    }
                    else if (product.Status == 0)
                    {
                        errorMessages.Add($"Product ID {item.ProductId} is unavailable.");
                    }
                    else if (product.StockQuantity < item.Quantity)
                    {
                        errorMessages.Add($"Product ID {item.ProductId} does not have enough stock.");
                    }
                    else
                    {
                        productList[item.ProductId] = product;
                    }
                }

                if (errorMessages.Any())
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "Cannot proceed with payment due to stock issues.", errorMessages);
                }

                // Cập nhật số lượng tồn kho và trạng thái sản phẩm
                foreach (var item in orderDetails)
                {
                    var product = productList[item.ProductId];
                    product.StockQuantity -= item.Quantity;
                    if (product.StockQuantity <= 0)
                    {
                        product.Status = 0; // hết hàng
                    }

                    await _unitOfWork.productRepository.UpdateAsync(product);
                }

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                // Tạo URL thanh toán
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



    }
}
