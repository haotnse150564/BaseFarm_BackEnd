using AutoMapper;
using Domain.Model;
using static Application.ViewModel.Response.OrderResponse;
using static Infrastructure.ViewModel.Request.FeedbackRequest;
using static Infrastructure.ViewModel.Response.FeedbackResponse;

namespace Infrastructure.Mapper
{
    public class FeedbackMapping : Profile
    {
        
        public FeedbackMapping()
        {
            CreateMap<Feedback, ViewFeedbackDTO>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Customer.AccountProfile.Fullname))
            .ForMember(dest => dest.OrderDetail, opt => opt.MapFrom(src => src.OrderDetail));

            CreateMap<Feedback, CreateFeedbackDTO>().ReverseMap();

        }
    }
}
