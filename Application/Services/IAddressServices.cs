using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IAddressServices
    {
        Task<AddressReponse> GetAddressReponseAsync();
        Task<AddressReponse> GetAddressReponseByIdAsync(long id);
        Task<AddressReponse> CreateAddressAsync(AddressRequest addressRequest);
        Task<AddressReponse> UpdateAddressAsync(long id, AddressRequest addressRequest);
        Task<bool> DeleteAddressAsync(long id);

    }
}
