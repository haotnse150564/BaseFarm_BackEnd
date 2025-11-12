using Application.ViewModel.Request;
using AutoMapper;
using Domain.Model;
using Infrastructure.Repositories.Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Request.ProductRequestDTO;
using static Application.ViewModel.Response.ProductResponse;

namespace Infrastructure.Mapper
{
    public class ProductsMapping : Profile
    {
        public ProductsMapping()
        {
            CreateMap<Product, ProductRequestDTO.CreateProductDTO>().ReverseMap();
            CreateMap<Product, ViewProductDTO>()
                .ForMember(dest => dest.CropName, opt => opt.MapFrom(src => src.ProductNavigation.CropName))
                .ForMember(dest => dest.CropId, opt => opt.MapFrom(src => src.ProductNavigation.CropId))
                .ForMember(dest => dest.Categoryname, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ReverseMap();
            CreateMap<Product, ProductDetailDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.CropName, opt => opt.MapFrom(src => src.ProductNavigation.CropName));
                //.ForMember(dest => dest.CropId, opt => opt.MapFrom(src => src.ProductNavigation.CropId))
                //.ReverseMap();
            //CreateMap<Product, UpdateProductDTO>()
            //    .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            //    .ReverseMap();
            //CreateMap<Product, CreateProductDTO>()
            //    .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            //    .ReverseMap();
            CreateMap<Product, UpdateQuantityDTO>().ReverseMap();
        }
    }
}
