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
