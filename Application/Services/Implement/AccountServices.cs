using Application;
using Application.Commons;
using Application.Services;
using Application.Utils;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Infrastructure.Repositories;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IAccountProfileRepository _accountProfileRepository;

        public AccountServices(IUnitOfWorks unitOfWork, IMapper mapper, JWTUtils jwtUtils, IAccountProfileRepository accountProfileRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtUtils = jwtUtils;
            _accountProfileRepository = accountProfileRepository;
        }

        public async Task<ViewAccount> CreateAccountAsync(AccountForm request)
        {
            var existingAccount = await _unitOfWork.accountRepository.GetByEmailAsync(request.Email);
            if (existingAccount != null)
            {
                throw new Exception("Email already exists!");
            }

            //Hash mật khẩu
            string hashedPassword = PasswordHelper.HashPassword("123123");

            //Tạo tài khoản mới
            var newAccount = new Account
            {
                Email = request.Email,
                PasswordHash = hashedPassword,
                Role = (Roles)request.Role, // Explicit cast to Roles enum
                Status = AccountStatus.ACTIVE, // Active
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };
            await _unitOfWork.accountRepository.AddAsync(newAccount);
            var newProfile = new AccountProfile
            {
                AccountProfileId = newAccount.AccountId,
                Phone = request.Phone,
                Fullname = request.Fullname,
                Gender = request.Gender,
                Address = request.Address,
                Images = request.Images,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };
            await _unitOfWork.accountProfileRepository.AddAsync(newProfile);
            var checkResut = await _unitOfWork.SaveChangesAsync();
            if (checkResut < 0)
            {
                throw new Exception("Create Fail!");
            }
            return _mapper.Map<ViewAccount>(newAccount);
        }

        public async Task<Pagination<ViewAccount>> GetAllAccountAsync(int pageSize, int pageIndex, AccountStatus? status, Roles? role)
        {
            try
            {
                // Lấy danh sách tài khoản từ repository với các bộ lọc status và role
                var account = await _unitOfWork.accountRepository.GetAllAccountWithProfiles(status, role);
                await _unitOfWork.accountProfileRepository.GetAllAsync();

                if (account == null || !account.Any())
                {
                    return null;
                }

                // Lọc tài khoản không phải Admin
                var userAccount = account.Where(x => x.Role != Roles.Admin).ToList();
                var result = _mapper.Map<List<ViewAccount>>(userAccount.OrderBy(x => x.Role));

                // Tổng số tài khoản
                var totalItem = result.Count;

                // Phân trang dữ liệu
                var pagination = new Pagination<ViewAccount>
                {
                    TotalItemCount = totalItem,
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    Items = result.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
                };

                return pagination;
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return null;
            }
        }

        public async Task<ViewAccount> GetAccountByEmail(string email)
        {
            try
            {
                var account = await _unitOfWork.accountRepository.GetByEmail(email);
                //await _unitOfWork.accountProfileRepository.GetAllAsync();
                if (account == null)
                {
                    return null;
                }
                //var oriPass = BCrypt.Net.BCrypt.HashPassword.(account.PasswordHash);
                var result = _mapper.Map<ViewAccount>(account);
                return result;
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
            if (account.Status != AccountStatus.ACTIVE)
            {
                throw new UnauthorizedAccessException("Account is not accitve.");
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
                Status = AccountStatus.ACTIVE, // Active
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };


            await _unitOfWork.accountRepository.AddAsync(newAccount);

            // Tạo profile rỗng với CreatedAt
            var newProfile = new AccountProfile
            {
                AccountProfileId = newAccount.AccountId,
                Fullname = request.Email.Split('@')[0],
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            await _unitOfWork.accountProfileRepository.AddAsync(newProfile);

            return new ResponseDTO(201, "Registration successful.");
        }

        public async Task<ViewAccount> UpdateAccountAsync(long id, AccountForm request)
        {
            try
            {
                var account = await _unitOfWork.accountRepository.GetByIdAsync(id);
                if (account == null)
                {
                    throw new Exception("Acoount not found");
                }
                else if (!request.Email.Equals(account.Email))
                {
                    var accounts = await _unitOfWork.accountRepository.GetAllAsync();
                    var emailExists = accounts.Select(x => x.Email).ToList();
                    var checkEmail = emailExists.Where(a => a.Equals(request.Email));
                    if (!checkEmail.IsNullOrEmpty())
                    {
                        throw new Exception("Email has been used");
                    }
                }
                //update account profile
                var accountProfile = await _unitOfWork.accountProfileRepository.GetByIdAsync(account.AccountProfile.AccountProfileId);
                accountProfile.Phone = request.Phone;
                accountProfile.Gender = request.Gender;
                accountProfile.Images = request.Images;
                accountProfile.Address = request.Address;
                accountProfile.Fullname = request.Fullname;
                accountProfile.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
                //update acccount
                account.Email = request.Email;
                account.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);


                // Cập nhật tài khoản và profile
                await _unitOfWork.accountProfileRepository.UpdateAsync(accountProfile);
                await _unitOfWork.accountRepository.UpdateAsync(account);
                if (await _unitOfWork.SaveChangesAsync() < 0)
                {
                    throw new Exception("Update Fail!");
                }
                else
                {
                    account.AccountProfile = accountProfile;
                    // Map dữ liệu sang DTO
                    return _mapper.Map<ViewAccount>(account);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
                if (account.Status == AccountStatus.ACTIVE)
                {
                    account.Status = AccountStatus.BANNED;
                }
                else
                {
                    account.Status = AccountStatus.ACTIVE;
                }
                await _unitOfWork.accountRepository.UpdateAsync(account);
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.Map<ViewAccount>(account);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Acoount not found");
            }
        }

        public async Task<ViewAccount> UpdateRoleForUser(long accountId, int roleId)
        {
            try
            {
                var account = await _unitOfWork.accountRepository.GetByIdAsync(accountId);
                if (account == null)
                {
                    return null;
                }
                else
                {
                    account.Role = (Roles)roleId;
                    await _unitOfWork.accountRepository.UpdateAsync(account);
                    await _unitOfWork.SaveChangesAsync();
                    var result = _mapper.Map<ViewAccount>(account);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Acoount not found");
            }
        }

        public async Task<ResponseDTO> ChangePassword(long id, ChangePasswordDTO request)
        {
            //Kiểm tra tài khoản có tồn tại hay không
            var account = await _unitOfWork.accountRepository.GetByIdAsync(id);
            if (account == null)
            {
                return new ResponseDTO(404, "Account not found");
            }
            else if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, account.PasswordHash))
            {
                return new ResponseDTO(400, "Old password is incorrect");
            }
            else if (!request.NewPassword.Equals(request.ConfirmPassword))
            {
                return new ResponseDTO(400, "New password and confirm password do not match");
            }
            else if (string.IsNullOrEmpty(request.NewPassword) || string.IsNullOrEmpty(request.ConfirmPassword)
                || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return new ResponseDTO(400, "Password do not have space or blank");
            }
            else
            {
                //Hash mật khẩu
                string hashedPassword = PasswordHelper.HashPassword(request.NewPassword);
                account.PasswordHash = hashedPassword;
                await _unitOfWork.accountRepository.UpdateAsync(account);
                await _unitOfWork.SaveChangesAsync();
                return new ResponseDTO(200, "Change password success");
            }
        }
    }
}
