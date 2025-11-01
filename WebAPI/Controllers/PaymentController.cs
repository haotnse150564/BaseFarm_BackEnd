using Application;
using Application.Services;
using Application.Services.Implement;
using Application.ViewModel.Request;
using Domain.Enum;
using Domain.Model;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Web;
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
        private readonly IUnitOfWorks _unitOfWork;

        public PaymentController(IVnPayService vnPayService, ILogger<PaymentController> logger, IOrderServices orderServices, IUnitOfWorks unitOfWork)
        {
            _vnPayService = vnPayService;
            _logger = logger;
            _orderServices = orderServices;
            _unitOfWork = unitOfWork;
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

        //[HttpGet("callback")]
        //public async Task<IActionResult> PaymentCallbackForApp()
        //{
        //    try
        //    {
        //        var response = await Task.Run(() => _vnPayService.PaymentExecute(Request.Query));
        //        if (response == null)
        //        {
        //            return BadRequest(new { message = "Invalid payment response." });
        //        }

        //        //  Lưu thông tin thanh toán
        //        await _vnPayService.SavePaymentAsync(response);

        //        // Luôn redirect về deeplink app mobile
        //        string appScheme = "ifms://payment-result"; //  thay bằng deeplink app thật

        //        string redirectUrl =
        //            $"{appScheme}?success={(response.Success ? "true" : "false")}" +
        //            $"&orderId={response.OrderId}" +
        //            $"&amount={response.Amount}" +
        //            $"&code={response.VnPayResponseCode}" +
        //            $"&message={(response.Success ? "PaymentSuccess" : "PaymentFailed")}";

        //        // 👉 Redirect HTTP 302 — app sẽ nhận deeplink này
        //        return Redirect(redirectUrl);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while processing payment callback.");

        //        // Nếu lỗi, vẫn redirect về deeplink báo lỗi cho app
        //        string failUrl = "myapp://payment-result?success=false&message=PaymentError";
        //        return Redirect(failUrl);
        //    }
        //}
        [HttpGet("CallBackForApp")]
        public async Task<IActionResult> PaymentCallbackForApp([FromQuery] string source = "mobile")
        {
            try
            {
                var response = _vnPayService.PaymentExecute(Request.Query);
                if (response == null)
                {
                    return BadRequest(new { message = "Invalid payment response." });
                }

                await _vnPayService.SavePaymentAsync(response);

                // ✅ Xử lý dữ liệu redirect
                string redirectUrl = $"ifms://payment-result" +
                                     $"?success={(response.Success ? "true" : "false")}" +
                                     $"&orderId={response.OrderId}" +
                                     $"&amount={response.Amount}" +
                                     $"&code={WebUtility.UrlEncode(response.VnPayResponseCode)}" +
                                     $"&message={WebUtility.UrlEncode(response.Success ? "PaymentSuccess" : "PaymentFailed")}";

                _logger.LogInformation($"CallBackForApp - Redirecting to deeplink: {redirectUrl}");

                // ✅ Không cache callback
                Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
                Response.Headers["Pragma"] = "no-cache";

                // ✅ Nếu test trên trình duyệt (không có app) thì fallback sang trang kết quả web
                string userAgent = Request.Headers["User-Agent"].ToString().ToLower();
                bool isMobile = userAgent.Contains("mobile") || userAgent.Contains("android") || userAgent.Contains("iphone");

                string fallbackUrl = $"https://iotfarm.onrender.com/payment-result?success={(response.Success ? "true" : "false")}&orderId={response.OrderId}";

                string finalRedirect = isMobile ? redirectUrl : fallbackUrl;

                // ✅ Trang HTML chuẩn UTF-8, có script và meta refresh
                string html = $@"
<!DOCTYPE html>
<html lang='vi'>
    <head>
        <meta charset='utf-8' />
        <meta name='viewport' content='width=device-width, initial-scale=1'>
        <title>Đang chuyển hướng...</title>
        <meta http-equiv='refresh' content='1;url={finalRedirect}' />
        <style>
            body {{
                font-family: Arial, sans-serif;
                text-align: center;
                margin-top: 80px;
                color: #333;
            }}
            a {{
                color: #007bff;
                text-decoration: none;
            }}
        </style>
    </head>
    <body>
        <h3>Đang chuyển hướng về ứng dụng...</h3>
        <p>Nếu không tự chuyển, hãy <a href='{finalRedirect}'>bấm vào đây</a>.</p>
        <script>
            setTimeout(function() {{
                window.location.href = '{finalRedirect}';
            }}, 800);
        </script>
    </body>
</html>";

                return Content(html, "text/html; charset=utf-8");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing payment callback for app.");

                string failUrl = "ifms://payment-result?success=false&message=PaymentError";
                string html = $@"
<!DOCTYPE html>
<html lang='vi'>
    <head>
        <meta charset='utf-8' />
        <meta name='viewport' content='width=device-width, initial-scale=1'>
        <title>Thanh toán thất bại</title>
        <meta http-equiv='refresh' content='2;url={failUrl}' />
    </head>
    <body style='font-family: Arial, sans-serif; text-align: center; margin-top: 80px;'>
        <h3>Thanh toán thất bại. Đang chuyển hướng...</h3>
        <a href='{failUrl}'>Nhấn vào đây nếu không tự chuyển.</a>
        <script>
            setTimeout(function() {{
                window.location.href = '{failUrl}';
            }}, 800);
        </script>
    </body>
</html>";

                return Content(html, "text/html; charset=utf-8");
            }
        }




        [HttpGet("redirect")]
        public async Task<IActionResult> RedirectToVnpay(long orderId)
        {
            var order = await _unitOfWork.orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return NotFound("Order not found");

            // ✅ Chuẩn bị model thanh toán
            var paymentModel = new PaymentInformationModel
            {
                OrderId = order.OrderId,
                Amount = (double)order.TotalPrice,
                OrderDescription = $"Thanh toán đơn hàng #{order.OrderId}",
                OrderType = "billpayment",
                Name = "IOT Base Farm"
            };

            // ✅ Tạo URL thật sang VNPAY gateway
            var vnpUrl = _vnPayService.CreatePaymentUrl(paymentModel, HttpContext);

            // ✅ Cập nhật trạng thái đơn hàng
            order.Status = PaymentStatus.PENDING;
            await _unitOfWork.orderRepository.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // ✅ Trang HTML trung gian (Proxy)
            var html = $@"
        <html>
        <head>
<meta charset=""utf-8"" />
            <meta name='viewport' content='width=device-width, initial-scale=1'>
            <title>Redirecting to VNPAY...</title>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    text-align: center;
                    padding-top: 40px;
                }}
                a {{
                    color: #007bff;
                    text-decoration: none;
                }}
            </style>
        </head>
        <body>
            <p>Redirecting to VNPAY...</p>
            <a href='{vnpUrl}'>Click here if not redirected automatically</a>
            <script>
                setTimeout(function() {{
                    window.location.href = '{vnpUrl}';
                }}, 1000);
            </script>
        </body>
        </html>
    ";

            return Content(html, "text/html");
        }


    }

}
