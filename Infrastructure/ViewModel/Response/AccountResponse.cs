using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Response
{
    public class AccountResponse
    {
        public class LoginResponseDTO
        {
            public string Token { get; set; }
            public string RefreshToken { get; set; }
        }
        public class ViewAccount
        {
            public long AccountId { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string Fullname { get; set; }
            public string Phone { get; set; }
            public string Status { get; set; }
            public string CreatedAt { get; set; }
            public string UpdatedAt { get; set; }
            public AccountProfileResponse.ProfileResponseDTO? AccountProfile { get; set; }
        }

        public class AvailableStaffDTO
        {
            public long AccountId { get; set; }
            public string Email { get; set; } = string.Empty;
            public string FullName { get; set; } = "Chưa có hồ sơ";
            public string Phone { get; set; } = string.Empty;
        }

        public class ResponseDTO
        {
            public int Status { get; set; }
            public string Message { get; set; }

            public ResponseDTO(int status, string message)
            {
                Status = status;
                Message = message;
            }
        }
    }
}
