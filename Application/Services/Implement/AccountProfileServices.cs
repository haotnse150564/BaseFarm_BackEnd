using Application.Utils;
using Application;
using AutoMapper;
using Application.Services;
using static Infrastructure.ViewModel.Response.AccountProfileResponse;
using Infrastructure.ViewModel.Request;

namespace WebAPI.Services
{
    public class AccountProfileServices : IAccountProfileServices
    {
        protected readonly IUnitOfWorks _unitOfWork;
        protected readonly IMapper _mapper;
        protected readonly JWTUtils _jwtUtils;
        public AccountProfileServices(IUnitOfWorks unitOfWork, IMapper mapper, JWTUtils jwtUtils)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtUtils = jwtUtils;
        }

        public async Task<ProfileResponseDTO> ViewProfileAsync()
        {
            var user = await _jwtUtils.GetCurrentUserAsync();
            if (user == null)
                throw new Exception("User not found");

            var profile = await _unitOfWork.accountProfileRepository.GetByIdAsync(user.AccountId);
            var getCurrentUser = await _unitOfWork.accountRepository.GetByIdAsync(user.AccountId);
            if (profile == null)
                throw new Exception("Profile not found");

            var profileResponse = _mapper.Map<ProfileResponseDTO>(profile);
            profileResponse.Role = getCurrentUser.Role.Value.ToString();
            profileResponse.Email = getCurrentUser.Email;
            ///profileResponse.Email = user.Email;
            return profileResponse;
        }

        public async Task<ProfileResponseDTO> UpdateProfileAsync(AccountProfileRequest.ProfileRequestDTO request)
        {
            var user = await _jwtUtils.GetCurrentUserAsync();
           var profile = await _unitOfWork.accountProfileRepository.GetByIdAsync(user.AccountId);

            // Không tìm thấy profile
            if (profile == null)
                throw new Exception(); 

            _mapper.Map(request, profile);

            profile.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);

            await _unitOfWork.accountProfileRepository.UpdateAsync(profile);
            var result = _mapper.Map<ProfileResponseDTO>(profile);
            return result;
        }
    }
}
