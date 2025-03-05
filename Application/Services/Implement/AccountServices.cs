using Application;
using Application.Services;
using Application.Utils;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using static Infrastructure.ViewModel.Request.AccountRequest;
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
                .GetByEmailAsync(email);

            if (account == null)
            {
                throw new UnauthorizedAccessException("Invalid email.");
            }
            // Kiểm tra mật khẩu có khớp không (dùng BCrypt)
            if (!PasswordHelper.VerifyPassword(password, account.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid password.");
            }
            // Tạo JWT Token
            string token = _jwtUtils.GenerateToken(account);

            // Trả về thông tin đăng nhập
            return new LoginResponseDTO
            {
                Token = token
            };
        }

        public async Task<ResponseDTO> RegisterAsync(RegisterRequestDTO request)
        {
            //Kiểm tra email đã tồn tại chưa
            var existingAccount = await _unitOfWork.accountRepository.GetByEmailAsync(request.Email);
            if (existingAccount != null)
            {
                return new ResponseDTO(400, "Email already exists.");
            }

            //Hash mật khẩu
            string hashedPassword = PasswordHelper.HashPassword(request.Password);

            //Tạo tài khoản mới
            var newAccount = new Account
            {
                Email = request.Email,
                PasswordHash = hashedPassword,
                Role = Roles.Customer,
                Status = 1, // Active
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            
            await _unitOfWork.accountRepository.AddAsync(newAccount);

            // Tạo profile rỗng với CreatedAt
            var newProfile = new AccountProfile
            {
                AccountProfileId = newAccount.AccountId,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            await _unitOfWork.accountProfileRepository.AddAsync(newProfile);

            return new ResponseDTO(201, "Registration successful.");
        }
    }
}
