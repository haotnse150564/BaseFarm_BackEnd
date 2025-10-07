using Application.Services;
using Infrastructure.ViewModel.Request;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/Address")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressServices _addressServices;
        public AddressController(IAddressServices addressServices)
        {
            _addressServices = addressServices;
        }
        [HttpGet]
        public async Task<IActionResult> GetAddressReponseAsync()
        {
            var address = await _addressServices.GetAddressReponseAsync();
            return Ok(address);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressReponseByIdAsync(long id)
        {
            var address = await _addressServices.GetAddressReponseByIdAsync(id);
            return Ok(address);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAddressAsync([FromBody] AddressRequest addressReponse)
        {
            var address = await _addressServices.CreateAddressAsync(addressReponse);
            return Ok(address);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddressAsync(long id, [FromBody] AddressRequest addressRequest)
        {
            var address = await _addressServices.UpdateAddressAsync(id, addressRequest);
            return Ok(address);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddressAsync(long id)
        {
            var result = await _addressServices.DeleteAddressAsync(id);
            return Ok(result);
        }
    }
}
