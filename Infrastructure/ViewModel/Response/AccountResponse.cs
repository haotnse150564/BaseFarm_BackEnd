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
    }
}
