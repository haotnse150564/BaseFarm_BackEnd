using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implement
{
    public class AddressServices : IAddressServices
    {
        public Task<AddressReponse> CreateAddressAsync(AddressRequest addressRequest)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAddressAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<AddressReponse> GetAddressReponseAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AddressReponse> GetAddressReponseByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<AddressReponse> UpdateAddressAsync(long id, AddressRequest addressRequest)
        {
            throw new NotImplementedException();
        }
    }
}
