using Application.Utils;
using AutoMapper;
using Domain.Model;
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
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly IMapper _mapper;
        private readonly JWTUtils _jWTUtils;
        public AddressServices(IUnitOfWorks unitOfWorks, IMapper mapper, JWTUtils jWTUtils)
        {
            _unitOfWorks = unitOfWorks;
            _mapper = mapper;
            _jWTUtils = jWTUtils;
        }

        public async Task<AddressReponse> CreateAddressAsync(AddressRequest addressRequest)
        {
            var user = await _jWTUtils.GetCurrentUserAsync();
            if (user == null)
            {
                return null;
            }
            else
            {
                var address = _mapper.Map<Address>(addressRequest);
                address.CreatedAt = DateTime.UtcNow;
                address.CustomerID = user.AccountId;
                await _unitOfWorks.addressRepository.AddAsync(address);
                if (await _unitOfWorks.SaveChangesAsync() < 1)
                {
                    return null;
                }
                else
                {
                    var result = _mapper.Map<AddressReponse>(address);
                    var account = await _unitOfWorks.accountRepository.GetByIdAsync(user.AccountId);

                    return result;
                }
            }
        }

        public async Task<bool> DeleteAddressAsync(long id)
        {
            var user = await _jWTUtils.GetCurrentUserAsync();
            if (user == null)
            {
                return false;
            }
            else
            {
                var address = await _unitOfWorks.addressRepository.GetByIdAsync(id);
                if (address == null || address.CustomerID != user.AccountId)
                {
                    return false;
                }
                else
                {
                    await _unitOfWorks.addressRepository.DeleteAsync(address);
                    if (await _unitOfWorks.SaveChangesAsync() < 1)
                    {
                        return false;
                    }
                    return true;
                }
            }
        }

        public async Task<List<AddressReponse>> GetAddressAsync()
        {
            var user = await _jWTUtils.GetCurrentUserAsync();
            if (user == null)
            {
                return null;
            }
            else
            {
                var address = await _unitOfWorks.addressRepository.GetListAddressByUserID(user.AccountId);
                if (address == null)
                {
                    return null;
                }
                else
                {
                    var result = _mapper.Map<List<AddressReponse>>(address);
                    var account = await _unitOfWorks.accountRepository.GetByIdAsync(user.AccountId);
                    return result;
                }
            }
        }

        public async Task<AddressReponse> GetAddressReponseByIdAsync(long id)
        {
            var user = await _jWTUtils.GetCurrentUserAsync();
            if (user == null)
            {
                return null;
            }
            else
            {
                var address = await _unitOfWorks.addressRepository.GetByIdAsync(id);
                if (address == null || address.CustomerID != user.AccountId)
                {
                    return null;
                }
                else
                {
                    var result = _mapper.Map<AddressReponse>(address);
                    var account = await _unitOfWorks.accountRepository.GetByIdAsync(user.AccountId);
                    return result;
                }
            }

        }

        public async Task<AddressReponse> UpdateAddressAsync(long id, AddressRequest addressRequest)
        {
            var user = await _jWTUtils.GetCurrentUserAsync();
            var address = await _unitOfWorks.addressRepository.GetByIdAsync(id);
            if (user == null || address == null)
            {
                return null;
            }
            else
            {
                var addressUpdate = _mapper.Map(addressRequest, address);
                address.UpdatedAt = DateTime.UtcNow;
                var result = _mapper.Map<AddressReponse>(addressUpdate);
                await _unitOfWorks.addressRepository.UpdateAsync(addressUpdate);
                if (await _unitOfWorks.SaveChangesAsync() < 1)
                {
                    return null;
                }
                return result;
            }
        }
    }
}
