using Application;
using Application.Commons;
using Application.Interfaces;
using Application.Services;
using Application.Utils;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using System.Drawing.Printing;
using static Infrastructure.ViewModel.Request.AccountRequest;
using static Infrastructure.ViewModel.Response.AccountResponse;
using ResponseDTO = Infrastructure.ViewModel.Response.AccountResponse.ResponseDTO;


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

        public async Task<ViewAccount> CreateAccountAsync(AccountForm request)
        {
            var existingAccount = await _unitOfWork.accountRepository.GetByEmailAsync(request.Email);
            if (existingAccount != null)
            {
                return null;//new ResponseDTO(400, "Email already exists.");
            }

            //Hash mật khẩu
            string hashedPassword = PasswordHelper.HashPassword(request.Password);

            //Tạo tài khoản mới
            var newAccount = new Account
            {
                Email = request.Email,
                PasswordHash = hashedPassword,
                Role = (Roles)request.Role, // Explicit cast to Roles enum
                Status = Status.ACTIVE, // Active
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };
            await _unitOfWork.accountRepository.AddAsync(newAccount);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ViewAccount>(newAccount);
        }

        public async Task<Pagination<ViewAccount>> GetAllAccountAsync(int pageSize, int pageIndex)
        {
            try
            {
                var account = await _unitOfWork.accountRepository.GetAllAsync();

                if (account == null)
                {
                    return null;
                }
                var userAccount = account.Where(x => x.Role != Roles.Admin).ToList();
                ;
                var result = _mapper.Map<List<ViewAccount>>(userAccount.OrderBy(x => x.Role));
                var tolalItem = result.Count;
                // Map dữ liệu sang DTO
                var pagination = new Pagination<ViewAccount>
                {
                    TotalItemCount = tolalItem,
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    Items = result.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
                };

                return pagination;
            }
            catch (Exception ex)
            {
                return null;
            }
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
                Status = Status.ACTIVE, // Active
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

        public async Task<ViewAccount> UpdateAccountAsync(long id, AccountForm request)
        {
            var account = await _unitOfWork.accountRepository.GetByIdAsync(id);
            if (account == null)
            {
                return null;//new ResponseDTO(400, "Email already exists.");
            }

            //Hash mật khẩu
            string hashedPassword = PasswordHelper.HashPassword(request.Password);

            //Tạo tài khoản mới

            account.Email = request.Email;
            account.PasswordHash = hashedPassword;
            account.Role = (Roles)request.Role; // Explicit cast to Roles enum
            account.Status = Status.ACTIVE; // Active
            account.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);


            await _unitOfWork.accountRepository.UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ViewAccount>(account);
        }

        public async Task<ViewAccount> UpdateAccountStatusAsync(long id)
        {
            try
            {
                var account = await _unitOfWork.accountRepository.GetByIdAsync(id);

                if (account == null)
                {
                    return null;
                }

                // Map dữ liệu sang DTO
                if (account.Status == Status.ACTIVE)
                {
                    account.Status = Status.BANNED;
                }
                else
                {
                    account.Status = Status.ACTIVE;
                }
                await _unitOfWork.accountRepository.UpdateAsync(account);
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.Map<ViewAccount>(account);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
