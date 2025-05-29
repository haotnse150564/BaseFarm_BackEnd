
using Application.ViewModel.Request;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.ViewModel.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using WebAPI.Services;
using static Application.ViewModel.Response.OrderResponse;

namespace Application.Services.Implement;

public class VnPayService : IVnPayService
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWorks _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<VnPayService> _logger;

    public VnPayService( IConfiguration configuration, IUnitOfWorks unitOfWorks, IMapper mapper
        , IOrderRepository orderRepository, IPaymentRepository paymentRepository, ILogger<VnPayService> logger)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWorks;
        _mapper = mapper;
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
    {
        var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
        var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
        var pay = new VnPayLibrary();

        var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];
        //var ipnUrl = _configuration["PaymentCallBack:IpnUrl"]; // Lấy IPN URL từ appsettings

        pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
        pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
        pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
        pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
        pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
        pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
        pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
        pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
        pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
        pay.AddRequestData("vnp_OrderType", model.OrderType);
        pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
        //pay.AddRequestData("vnp_IpnUrl", ipnUrl);

        // 🔥 Sử dụng OrderId của hệ thống thay vì tick
        pay.AddRequestData("vnp_TxnRef", model.OrderId.ToString());

        var paymentUrl =
            pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

        return paymentUrl;
    }



    public PaymentResponseModel PaymentExecute(IQueryCollection collections)
    {
        var pay = new VnPayLibrary();
        var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

        return response;
    }

    //public async Task<ResponseDTO> SavePaymentAsync(PaymentResponseModel response)
    //{
    //    //using var transaction = await _unitOfWork.BeginTransactionAsync();
    //    try
    //    {
    //        var order = await _orderRepository.GetByIdAsync(response.OrderId);
    //        if (order == null)
    //            throw new Exception($"Order ID {response.OrderId} not found.");

    //        var payment = new Payment
    //        {
    //            OrderId = response.OrderId,
    //            TransactionId = response.TransactionId,
    //            PaymentMethod = response.PaymentMethod,
    //            Success = response.Success ? true : false,
    //            Amount = response.Amount,
    //            PaymentTime = DateTime.UtcNow,
    //            VnPayResponseCode = response.VnPayResponseCode,
    //            CreateDate = DateTime.UtcNow
    //        };
    //        _logger.LogInformation("payment"+payment);

    //        await _unitOfWork.paymentRepository.AddAsync(payment);

    //        if (response.Success)
    //        {
    //            order.Status = Status.PAID;
    //            await _orderRepository.UpdateAsync(order);
    //        }
    //        else
    //        {
    //            order.Status = Status.UNDISCHARGED; // Đặt trạng thái khác khi response không thành công
    //            await _orderRepository.UpdateAsync(order);
    //        }

    //        await _unitOfWork.SaveChangesAsync();
    //        //await transaction.CommitAsync();
    //        return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "Payment Created.");
    //    }
    //    catch(Exception ex)
    //    {
    //        //await transaction.RollbackAsync();
    //        return new ResponseDTO(Const.ERROR_EXCEPTION, "An error occurred while creating the payment.", ex.Message);
    //    }
    //}

    public async Task<ResponseDTO> SavePaymentAsync(PaymentResponseModel response)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(response.OrderId);
            if (order == null)
                throw new Exception($"Order ID {response.OrderId} not found.");

            var payment = new Payment
            {
                OrderId = response.OrderId,
                TransactionId = response.TransactionId,
                PaymentMethod = response.PaymentMethod,
                Success = response.Success,
                Amount = response.Amount,
                PaymentTime = DateTime.UtcNow,
                VnPayResponseCode = response.VnPayResponseCode,
                CreateDate = DateTime.UtcNow
            };

            await _unitOfWork.paymentRepository.AddAsync(payment);

            if (response.Success)
            {
                order.Status = PaymentStatus.PAID;
                await _orderRepository.UpdateAsync(order);
            }
            else
            {
                order.Status = PaymentStatus.UNDISCHARGED;
                await _orderRepository.UpdateAsync(order);

                // 🔥 Hoàn lại số lượng sản phẩm nếu thanh toán thất bại
                var orderDetails = await _unitOfWork.orderDetailRepository.GetOrderDetailsByOrderId(order.OrderId);
                foreach (var item in orderDetails)
                {
                    var product = await _unitOfWork.productRepository.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity ?? 0; // Đảm bảo không bị null

                        // Nếu trước đó sản phẩm hết hàng thì cập nhật lại trạng thái
                        if (product.StockQuantity > 0)
                            product.Status = ProductStatus.ACTIVE; // Đổi sang trạng thái phù hợp

                        await _unitOfWork.productRepository.UpdateAsync(product);
                    }
                }

                // 🔥 Gọi `SaveChangesAsync()` để lưu thay đổi trong `Product`
                //await _unitOfWork.SaveChangesAsync();
            }

            await _unitOfWork.SaveChangesAsync();
            return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "Payment processed successfully.");
        }
        catch (Exception ex)
        {
            return new ResponseDTO(Const.ERROR_EXCEPTION, "An error occurred while processing the payment.", ex.Message);
        }
    }



    public async Task<ResponseDTO> GetPaymentByOrderIdAsync(long orderId)
    {
        try
        {
            var getPayment = await _unitOfWork.paymentRepository.GetByOrderIdAsync(orderId);

            if (getPayment == null)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, "No Payment found.");
            }

            return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, getPayment);
        }
        catch (Exception ex)
        {
            return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
        }
    }
}
