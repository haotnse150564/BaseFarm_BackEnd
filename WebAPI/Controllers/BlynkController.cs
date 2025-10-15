using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}
