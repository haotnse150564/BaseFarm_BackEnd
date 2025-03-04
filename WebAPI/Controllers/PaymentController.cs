using Application;
using Application.Services;
using Application.Services.Implement;
using Application.ViewModel.Request;
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
        private readonly IUnitOfWorks _unitOfWork;

        public PaymentController(IVnPayService vnPayService, ILogger<PaymentController> logger, IOrderServices orderServices, IUnitOfWorks unitOfWork)
        {
            _vnPayService = vnPayService;
            _logger = logger;
            _orderServices = orderServices;
            _unitOfWork = unitOfWork;
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

        /// <summary>
        /// Xử lý callback từ VNPay sau khi thanh toán
        /// </summary>
        //[HttpGet("callback")]
        //public async Task<IActionResult> PaymentCallback()
        //{
        //    try
        //    {
        //        var response = await Task.Run(() => _vnPayService.PaymentExecute(Request.Query));

        //        if (response == null)
        //        {
        //            return BadRequest(new { message = "Invalid payment response." });
        //        }

        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while processing payment callback.");
        //        return StatusCode(500, new { message = "An error occurred while processing the payment response." });
        //    }
        //}

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

                // 🔥 Lấy OrderId từ phản hồi VNPay
                if (!long.TryParse(response.OrderId, out long orderId))
                {
                    return BadRequest(new { message = "Invalid OrderId in payment response." });
                }

                // 🔥 Tìm đơn hàng trong database
                var order = await _unitOfWork.orderRepository.GetOrderById(orderId);
                if (order == null)
                {
                    return NotFound(new { message = "Order not found." });
                }

                // 🔥 Cập nhật trạng thái đơn hàng dựa vào phản hồi từ VNPay
                if (response.VnPayResponseCode == "00") // ✅ 00: Thanh toán thành công
                {
                    order.Status = 3; // Đơn hàng đã thanh toán thành công
                }
                else // ❌ Các mã khác: Thanh toán thất bại
                {
                    order.Status = 4; // Thanh toán thất bại
                }

                await _unitOfWork.orderRepository.UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync();

                // 🔥 Trả về thông tin payment sau khi cập nhật đơn hàng
                return Ok(new
                {
                    message = "Payment processed successfully.",
                    orderStatus = order.Status, // ✅ Trạng thái đơn hàng đã được cập nhật
                    payment = new
                    {
                        response.TransactionId,
                        response.OrderId, // Đây là OrderId của Payment
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
