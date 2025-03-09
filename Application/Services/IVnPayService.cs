
using Application.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using Microsoft.AspNetCore.Http;

namespace Application.Services;
public interface IVnPayService
{
    string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
    PaymentResponseModel PaymentExecute(IQueryCollection collections);
    Task SavePaymentAsync(PaymentResponseModel response);
}