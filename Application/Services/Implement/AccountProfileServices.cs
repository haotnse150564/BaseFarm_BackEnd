using Application.Utils;
using Application;
using AutoMapper;
using Application.Services;
using static Infrastructure.ViewModel.Response.AccountProfileResponse;

namespace WebAPI.Services
{
    public class AccountProfileServices : IAccountProfileServices
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly IMapper _mapper;
        private readonly JWTUtils _jwtUtils;
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
            if (profile == null)
                throw new Exception("Profile not found");

            var profileResponse = _mapper.Map<ProfileResponseDTO>(profile);
            profileResponse.Email = user.Email; // Thêm email từ bảng Account
            return profileResponse;
        }
    }
}
