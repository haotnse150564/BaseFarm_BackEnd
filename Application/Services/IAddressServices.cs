using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.AddressReponse;

namespace Application.Services
{
    public interface IAddressServices
    {
        Task<ResponseDTO> GetAddressAsync();
        Task<ResponseDTO> GetAddressReponseByIdAsync(long id);
        Task<ResponseDTO> CreateAddressAsync(AddressRequest addressRequest);
        Task<ResponseDTO> UpdateAddressAsync(long id, AddressRequest addressRequest);
        Task<ResponseDTO> DeleteAddressAsync(long id);

    }
}
