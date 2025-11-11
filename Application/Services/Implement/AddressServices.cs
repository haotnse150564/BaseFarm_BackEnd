using Application.Utils;
using AutoMapper;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.AddressReponse;

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

        public async Task<ResponseDTO> CreateAddressAsync(AddressRequest addressRequest)
        {
            try
            {
                var user = await _jWTUtils.GetCurrentUserAsync();
                if (user == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không tồn tại.");
                }
                else
                {
                    var allAddress = await _unitOfWorks.addressRepository.GetListAddressByUserID(user.AccountId);
                    var address = _mapper.Map<Address>(addressRequest);

                    //địa chỉ đầu tiên
                    if (allAddress.IsNullOrEmpty())
                    {
                        address.IsDefault = true;
                    }
                    //nếu không phải địa chỉ đầu tiên, nhưng được chọn là mặc định
                    else if (address.IsDefault && allAddress.Count() >= 1)
                    {
                        foreach (var item in allAddress)
                        {
                            //tắt default các địa chỉ và lưu la
                            item.IsDefault = false;
                            await _unitOfWorks.addressRepository.UpdateAsync(item);
                            await _unitOfWorks.SaveChangesAsync();
                        }
                    }

                    address.CreatedAt = DateTime.UtcNow;
                    address.CustomerID = user.AccountId;
                    await _unitOfWorks.addressRepository.AddAsync(address);
                    await _unitOfWorks.SaveChangesAsync();

                    var result = _mapper.Map<AddressReponse>(address);
                    var account = await _unitOfWorks.accountRepository.GetByIdAsync(user.AccountId);
                    return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);

                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }


        public async Task<ResponseDTO> DeleteAddressAsync(long id)
        {
            try
            {
                var user = await _jWTUtils.GetCurrentUserAsync();
                if (user == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "User khng tồn tại");
                }
                else
                {
                    var address = await _unitOfWorks.addressRepository.GetByIdAsync(id);
                    if (address == null || address.CustomerID != user.AccountId)
                    {
                        return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Địa chỉ không tồn tại");
                    }
                    else
                    {

                        if (address.IsDefault == true)
                        {
                            var allAddress = await _unitOfWorks.addressRepository.GetListAddressByUserID(user.AccountId);
                            var firstAddress = allAddress.FirstOrDefault();
                            if (firstAddress != null)
                            {
                                firstAddress.IsDefault = true;
                                await _unitOfWorks.addressRepository.DeleteAsync(address);
                                await _unitOfWorks.SaveChangesAsync();

                                return new ResponseDTO(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG, "Xoá thành công, cập nhật lại địa chỉ mặc định");

                            }
                        }

                        await _unitOfWorks.addressRepository.DeleteAsync(address);
                        await _unitOfWorks.SaveChangesAsync();

                        return new ResponseDTO(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG, "Xoá thành công.");

                    }
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
        public async Task<ResponseDTO> GetAddressAsync()
        {
            try
            {
                var user = await _jWTUtils.GetCurrentUserAsync();
                if (user == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không tồn tại.");
                }
                else
                {
                    var address = await _unitOfWorks.addressRepository.GetListAddressByUserID(user.AccountId);
                    if (address == null)
                    {
                        return new ResponseDTO(Const.FAIL_READ_CODE, "Không có địa chỉ."); ;
                    }
                    else
                    {
                        var result = _mapper.Map<List<AddressReponse>>(address);
                        var account = await _unitOfWorks.accountRepository.GetByIdAsync(user.AccountId);
                        return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> GetAddressReponseByIdAsync(long id)
        {
            try
            {
                var user = await _jWTUtils.GetCurrentUserAsync();
                if (user == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không tồn tại.");
                }
                else
                {
                    var address = await _unitOfWorks.addressRepository.GetByIdAsync(id);
                    if (address == null || address.CustomerID != user.AccountId)
                    {
                        return new ResponseDTO(Const.FAIL_READ_CODE, "Địa chỉ không tồn tại"); ;
                    }
                    else
                    {
                        var result = _mapper.Map<AddressReponse>(address);
                        var account = await _unitOfWorks.accountRepository.GetByIdAsync(user.AccountId);
                        return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> UpdateAddressAsync(long id, AddressRequest addressRequest)
        {
            try
            {
                var user = await _jWTUtils.GetCurrentUserAsync();

                var address = await _unitOfWorks.addressRepository.GetByIdAsync(id);
                var allAddress = await _unitOfWorks.addressRepository.GetListAddressByUserID(user.AccountId);
                if (user == null || address == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không tồn tại.");
                }
                else
                {
                    var addressUpdate = _mapper.Map(addressRequest, address);
                    if (addressUpdate.IsDefault == true)
                    {
                        foreach (var item in allAddress)
                        {
                            //tắt default các địa chỉ và lưu la
                            item.IsDefault = false;
                            await _unitOfWorks.addressRepository.UpdateAsync(item);
                            await _unitOfWorks.SaveChangesAsync();
                        }
                    }
                    if (addressUpdate.IsDefault == false)
                    {
                        var haveDefault = allAddress.Any(a => a.IsDefault == true && a.AddressID != id);
                        if (!haveDefault)
                        {
                            addressUpdate.IsDefault = true;
                        }
                    }
                    addressUpdate.UpdatedAt = DateTime.UtcNow;
                    var result = _mapper.Map<AddressReponse>(addressUpdate);
                    await _unitOfWorks.addressRepository.UpdateAsync(addressUpdate);
                    await _unitOfWorks.SaveChangesAsync();

                    return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, result);
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
