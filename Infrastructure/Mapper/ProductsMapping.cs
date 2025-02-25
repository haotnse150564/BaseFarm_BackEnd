using AutoMapper;
using Domain;
using Infrastructure.Repositories.Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Response.ProductResponse;

namespace Infrastructure.Mapper
{
    public class ProductsMapping : Profile
    {
        public ProductsMapping()
        {
            CreateMap<Product, ViewProductDTO>().ReverseMap();
            CreateMap<Product, ProductDetailDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ReverseMap();

        }
    }
}
