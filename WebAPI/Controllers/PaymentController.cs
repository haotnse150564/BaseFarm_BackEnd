using Application.Services;
using Application.ViewModel.Request;
using Microsoft.AspNetCore.Mvc;

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

        /// <summary>
        /// Tạo URL thanh toán VNPay
        /// </summary>
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

        [HttpGet("callback")]
        public async Task<IActionResult> PaymentCallback()
        {
            try
            {
                var response = await Task.Run(() => _vnPayService.PaymentExecute(Request.Query));

                if (response == null)
                {
                    return BadRequest(new { message = "Invalid payment response." });
                }

                // 🔥 Trả về thông tin payment như bình thường
                return Ok(new
                {
                    message = "Payment processed successfully.",
                    payment = new
                    {
                        response.TransactionId,
                        response.OrderId, // Đây là OrderId của Payment, không phải Order thực tế
                        response.PaymentMethod,
                        response.VnPayResponseCode,
                        response.Success
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing payment callback.");
                return StatusCode(500, new { message = "An error occurred while processing the payment response." });
            }
        }

    }

}
