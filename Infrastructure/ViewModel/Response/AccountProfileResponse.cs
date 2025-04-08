using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Response
{
    public class AccountProfileResponse
    {
        public class ProfileResponseDTO
        {
            public long AccountProfileId { get; set; }

            public string Gender { get; set; }

            public string? Phone { get; set; }

            public string? Fullname { get; set; }

            public string? Address { get; set; }

            public string? Images { get; set; }

            public string CreatedAt { get; set; }
            public string UpdatedAt { get; set; }

        }
    }
}
