using Application;
using Application.Services;
using Application.Services.Implement;
using Application.ViewModel.Request;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using static Application.ViewModel.Request.OrderRequest;

namespace WebAPI.Controllers
{
    [Route("api/vnpay")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly ILogger<PaymentController> _logger;
        private readonly IOrderServices _orderServices;

        public PaymentController(IVnPayService vnPayService, ILogger<PaymentController> logger, IOrderServices orderServices)
        {
            _vnPayService = vnPayService;
            _logger = logger;
            _orderServices = orderServices;
        }

        /// Tạo URL thanh toán VNPay
        [HttpPost("create-payment-url")]
        public async Task<IActionResult> CreatePaymentUrl([FromBody] PaymentInformationModel model)
        {
            if (model == null || model.Amount <= 0)
            {
                return BadRequest(new { message = "Invalid payment request." });
            }

            try
            {
                var paymentUrl = await Task.Run(() => _vnPayService.CreatePaymentUrl(model, HttpContext));

                if (string.IsNullOrEmpty(paymentUrl))
                {
                    return StatusCode(500, new { message = "Failed to generate payment URL." });
                }

                return Ok(new { url = paymentUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating payment URL.");
                return StatusCode(500, new { message = "An error occurred while processing payment." });
            }
        }

        //[HttpGet("callback")]
        //public async Task<IActionResult> PaymentCallback()
        //{
        //    try
        //    {
        //        var response = await Task.Run(() => _vnPayService.PaymentExecute(Request.Query));
        //        Console.WriteLine($"Response: {response}");
        //        if (response == null)
        //        {
        //            return BadRequest(new { message = "Invalid payment response." });
        //        }

        //        // ✅ Save payment to the database
        //        await _vnPayService.SavePaymentAsync(response);

        //        //// 🔥 Redirect to frontend with payment status
        //        //string frontendUrl = response.Success ? "https://iotbasedfarm.netlify.app/vnpay-callback" : "https://iotbasedfarm.netlify.app/order-failed";
        //        //string redirectUrl = $"{frontendUrl}?vnp_ResponseCode={response.VnPayResponseCode}&vnp_TransactionNo={response.TransactionId}&vnp_TxnRef={response.OrderId}&vnp_Amount={response.Amount}";

        //        return Ok(response);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while processing payment callback.");
        //        return StatusCode(500, new { message = "An error occurred while processing the payment response." });
        //    }
        //}

        [HttpGet("ipn")]
        public async Task<IActionResult> PaymentIpn()
        {
            try
            {
                var response = await Task.Run(() => _vnPayService.PaymentExecute(Request.Query));

                if (response == null)
                {
                    return BadRequest("Invalid IPN request.");
                }

                await _vnPayService.SavePaymentAsync(response);

                return Ok("IPN received and processed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing VNPAY IPN.");
                return StatusCode(500, "Internal Server Error while processing IPN.");
            }
        }


        [HttpGet("PaymentByOrderId/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrderId(long orderId)
        {
            var result = await _vnPayService.GetPaymentByOrderIdAsync(orderId);

            if (result.Status == Const.FAIL_READ_CODE)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> PaymentCallbackForApp()
        {
            try
            {
                var response = await Task.Run(() => _vnPayService.PaymentExecute(Request.Query));
                if (response == null)
                {
                    return BadRequest(new { message = "Invalid payment response." });
                }

                //  Lưu thông tin thanh toán
                await _vnPayService.SavePaymentAsync(response);

                // Luôn redirect về deeplink app mobile
                string appScheme = "ifms://payment-result"; //  thay bằng deeplink app thật

                string redirectUrl =
                    $"{appScheme}?success={(response.Success ? "true" : "false")}" +
                    $"&orderId={response.OrderId}" +
                    $"&amount={response.Amount}" +
                    $"&code={response.VnPayResponseCode}" +
                    $"&message={(response.Success ? "PaymentSuccess" : "PaymentFailed")}";

                // 👉 Redirect HTTP 302 — app sẽ nhận deeplink này
                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing payment callback.");

                // Nếu lỗi, vẫn redirect về deeplink báo lỗi cho app
                string failUrl = "myapp://payment-result?success=false&message=PaymentError";
                return Redirect(failUrl);
            }
        }

    }

}
