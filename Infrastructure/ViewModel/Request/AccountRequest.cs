﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Request
{
    public class AccountRequest
    {
        public class LoginRequestDTO
        {
            public int Phone { get; set; }
            public string Password { get; set; }
        }
    }
}
