using Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Request
{
    public class AccountRequest
    {
        public class LoginRequestDTO
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            public string Password { get; set; }
        }
        public class AccountForm
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            public Gender? Gender { get; set; }
            public string? Phone { get; set; }
            public string? Fullname { get; set; }
            public string? Address { get; set; }
            public string? Images { get; set; }
        }
        public class ChangePasswordDTO
        {
            public string OldPassword { get; set; }
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
            public string NewPassword { get; set; }
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
            public string ConfirmPassword { get; set; }

        }
            public class RegisterRequestDTO
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
            public string Password { get; set; }
            [Required]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }
        }
    }
}
