using Application;
using Application.Services;
using Application.Utils;
using AutoMapper;
using static Infrastructure.ViewModel.Response.AccountResponse;

namespace WebAPI.Services
{
    public class AccountServices : IAccountServices
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly IMapper _mapper;
        private readonly JWTUtils _jwtUtils;
        public AccountServices(IUnitOfWorks unitOfWork, IMapper mapper, JWTUtils jwtUtils)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtUtils = jwtUtils;
        }

        public async Task<LoginResponseDTO> LoginAsync(string email, string password)
        {
            // Kiểm tra tài khoản có tồn tại hay không
            var account = await _unitOfWork.accountRepository
                .FirstOrDefaultAsync(a => a.Email == email && a.PasswordHash == password);

            if (account == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            // Tạo JWT Token
            string token = _jwtUtils.GenerateToken(account);

            // Trả về thông tin đăng nhập
            return new LoginResponseDTO
            {
                Token = token
            };
        }
    }
}
