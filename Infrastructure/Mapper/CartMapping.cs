using AutoMapper;
using Domain.Model;
using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mapper
{
    public class CartMapping : Profile
    {
        public CartMapping()
        {
            CreateMap<CartResponse, Cart>().ReverseMap();
            CreateMap<CartItemResponse, CartItem>().ReverseMap();
        }

    }
}
