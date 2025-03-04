using Application.Commons;
using AutoMapper;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Request.OrderRequest;
using static Application.ViewModel.Response.OrderResponse;
using static Application.ViewModel.Response.ProductResponse;

namespace Infrastructure.Mapper
{
    public class OrderMapping : Profile
    {
        public OrderMapping()
        {
            // Mapping từ CreateOrderDTO -> Order
            CreateMap<CreateOrderDTO, Order>()
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore()) // Chi tiết đơn hàng sẽ được xử lý riêng
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore()) // Tổng tiền sẽ được tính sau
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateOnly.FromDateTime(DateTime.UtcNow)));

            // Mapping từ SelectProductDTO -> OrderDetail
            CreateMap<SelectProductDTO, OrderDetail>()
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.StockQuantity))
                .ForMember(dest => dest.UnitPrice, opt => opt.Ignore()) // Giá sẽ được lấy từ bảng Product
                .ForMember(dest => dest.OrderId, opt => opt.Ignore()); // OrderId sẽ được gán sau khi tạo Order

            CreateMap<OrderDetail, ViewProductDTO>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<Pagination<Order>, Pagination<OrderResultDTO>>()
                .ReverseMap();

            CreateMap<Order, OrderResultDTO>()
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Customer.AccountProfile.Phone))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderDetails))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.HasValue ? src.CreatedAt.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null))
                .ReverseMap();


            CreateMap<Product, ViewProductDTO>(); // 🔥 Mapping trực tiếp từ Product sang ViewProductDTO

            CreateMap<UpdateOrderStatusDTO, Order>()
                .ReverseMap();

            CreateMap<Order, CreateOrderResultDTO>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.HasValue ? src.CreatedAt.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null));
        }

    }
}
