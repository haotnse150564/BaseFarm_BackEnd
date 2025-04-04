using AutoMapper;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.AccountResponse;

namespace Infrastructure.Mapper
{
    public class AccountMapping : Profile
    {
        public AccountMapping()
        {
            CreateMap<Account, ViewAccount>().ReverseMap();
        }
    }
}
