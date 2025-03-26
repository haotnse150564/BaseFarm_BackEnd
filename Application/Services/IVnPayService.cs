
using Application.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using Microsoft.AspNetCore.Http;
using static Application.ViewModel.Response.OrderResponse;

namespace Application.Services;
public interface IVnPayService
{
    string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
    PaymentResponseModel PaymentExecute(IQueryCollection collections);
    Task<ResponseDTO> SavePaymentAsync(PaymentResponseModel response);
    Task<ResponseDTO> GetPaymentByOrderIdAsync(long orderId);
}