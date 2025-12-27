using Application;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/blynk")]
    public class BlynkController : ControllerBase
    {
        private readonly IBlynkService _blynkService;
        public BlynkController(IBlynkService blynkService)
        {
            _blynkService = blynkService;
        }

        [HttpGet("get-blynk-data")]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetBlynkData()
        {
            var data = await _blynkService.GetAllDatastreamValuesAsync();
            return Ok(data);
        }

        /// <summary>
        /// Turn the pump ON or OFF via Blynk Cloud
        /// </summary>
        /// <param name="state">true = ON, false = OFF</param>
        [HttpPost("pump")]
        public async Task<IActionResult> ControlPump([FromQuery] bool state)
        {
            bool result = await _blynkService.ControlPumpAsync(state);
            if (result)
            {
                return Ok(new
                {
                    success = true,
                    message = $"Pump has been {(state ? "turned ON" : "turned OFF")} successfully."
                });
            }
            else
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Failed to send pump command to Blynk Cloud."
                });
            }
        }

        /// <summary>
        /// Set servo angle (0–180 degrees) via Blynk Cloud
        /// </summary>
        [HttpPost("servo")]
        public async Task<IActionResult> ControlServo([FromQuery] int angle)
        {
            if (angle < 0 || angle > 180)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Servo angle must be between 0 and 180 degrees."
                });
            }

            bool result = await _blynkService.ControlServoAsync(angle);
            if (result)
            {
                return Ok(new
                {
                    success = true,
                    message = $"Servo angle has been set to {angle} degrees successfully."
                });
            }
            else
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Failed to send servo command to Blynk Cloud."
                });
            }
        }
        /// <summary>
        /// Enable or disable manual control mode (V7)
        /// </summary>
        /// <param name="state">true = Manual, false = Auto</param>
        [HttpPost("manual-mode")]
        public async Task<IActionResult> SetManualMode([FromQuery] bool state)
        {
            bool result = await _blynkService.SetManualModeAsync(state);
            if (result)
            {
                return Ok(new
                {
                    success = true,
                    message = $"Manual mode has been {(state ? "ENABLED" : "DISABLED")} successfully."
                });
            }
            else
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Failed to send manual mode command to Blynk Cloud."
                });
            }
        }

        [HttpPost("threshold/soil-low")]   // V8
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> SetSoilLowThreshold([FromQuery] int value)
        {
            if (value < 0 || value > 100)
                return BadRequest(new { success = false, message = "Ngưỡng phải từ 0-100%" });

            var result = await _blynkService.SetSoilLowThresholdAsync(value);
            return result
                ? Ok(new { success = true, message = $"Ngưỡng BẬT bơm đã đặt ≤ {value}%" })
                : StatusCode(500, new { success = false, message = "Lỗi gửi lệnh đến thiết bị" });
        }

        [HttpPost("threshold/soil-high")]  // V9
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> SetSoilHighThreshold([FromQuery] int value)
        {
            if (value < 0 || value > 100)
                return BadRequest(new { success = false, message = "Ngưỡng phải từ 0-100%" });

            var result = await _blynkService.SetSoilHighThresholdAsync(value);
            return result
                ? Ok(new { success = true, message = $"Ngưỡng TẮT bơm đã đặt ≥ {value}%" })
                : StatusCode(500, new { success = false, message = "Lỗi gửi lệnh đến thiết bị" });
        }

        [HttpPost("threshold/ldr-low")]    // V10
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> SetLdrLowThreshold([FromQuery] int value)
        {
            if (value < 0 || value > 1023)
                return BadRequest(new { success = false, message = "Ngưỡng LDR phải từ 0-1023" });

            var result = await _blynkService.SetLdrLowThresholdAsync(value);
            return result
                ? Ok(new { success = true, message = $"Ngưỡng ánh sáng THẤP đã đặt: {value}" })
                : StatusCode(500, new { success = false, message = "Lỗi gửi lệnh đến thiết bị" });
        }

        [HttpPost("threshold/ldr-high")]   // V11
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> SetLdrHighThreshold([FromQuery] int value)
        {
            if (value < 0 || value > 1023)
                return BadRequest(new { success = false, message = "Ngưỡng LDR phải từ 0-1023" });

            var result = await _blynkService.SetLdrHighThresholdAsync(value);
            return result
                ? Ok(new { success = true, message = $"Ngưỡng ánh sáng CAO đã đặt: {value}" })
                : StatusCode(500, new { success = false, message = "Lỗi gửi lệnh đến thiết bị" });
        }
        [HttpGet("logs")]
        public async Task<IActionResult> GetIOTLogs()
        {
            var response = await _blynkService.GetList();
            return Ok(response);
        }
        //[HttpGet("logs/update")]
        //public async Task<IActionResult> UpdateIOTLogs()
        //{
        //    var result = await _blynkService.UpdateLogAsync();
        //   return Ok(result);
        //}
        [HttpGet("export")]
        public async Task<IActionResult> ExportCsv()
        {
            var fileBytes = await _blynkService.ExportLogsToCsvAsync();
            return File(fileBytes, "text/csv", "IFMS_iot_logs.csv");
        }



        /// <summary>
        /// Bật/tắt đèn LED bổ sung (Grow Light) - Chỉ hoạt động khi ở chế độ Manual
        /// </summary>
        [HttpPost("light")]
        public async Task<IActionResult> ControlLight([FromQuery] bool state)
        {
            bool result = await _blynkService.ControlLightAsync(state);
            return result
                ? Ok(new { success = true, message = $"Đèn LED đã được {(state ? "BẬT" : "TẮT")} thành công." })
                : StatusCode(500, new { success = false, message = "Lỗi gửi lệnh đèn đến Blynk. (Có thể đang ở chế độ Auto)" });
        }

        /// <summary>
        /// Cấu hình ngưỡng BẬT đèn khi trời tối (V13)
        /// </summary>
        [HttpPost("threshold/light-on")]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> SetLightOnThreshold([FromQuery] int value)
        {
            if (value < 0 || value > 1023)
                return BadRequest(new { success = false, message = "Ngưỡng ánh sáng phải từ 0-1023" });

            var result = await _blynkService.SetLightOnThresholdAsync(value);
            return result
                ? Ok(new { success = true, message = $"Ngưỡng BẬT đèn (trời tối) đã đặt: LDR ≤ {value}" })
                : StatusCode(500, new { success = false, message = "Lỗi gửi ngưỡng bật đèn" });
        }

        /// <summary>
        /// Cấu hình ngưỡng TẮT đèn khi trời sáng (V14)
        /// </summary>
        [HttpPost("threshold/light-off")]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> SetLightOffThreshold([FromQuery] int value)
        {
            if (value < 0 || value > 1023)
                return BadRequest(new { success = false, message = "Ngưỡng ánh sáng phải từ 0-1023" });

            var result = await _blynkService.SetLightOffThresholdAsync(value);
            return result
                ? Ok(new { success = true, message = $"Ngưỡng TẮT đèn (trời sáng) đã đặt: LDR ≥ {value}" })
                : StatusCode(500, new { success = false, message = "Lỗi gửi ngưỡng tắt đèn" });
        }
    }
}
