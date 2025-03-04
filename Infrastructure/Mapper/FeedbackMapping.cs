using AutoMapper;
using Domain.Model;
using static Infrastructure.ViewModel.Request.FeedbackRequest;
using static Infrastructure.ViewModel.Response.FeedbackResponse;

namespace Infrastructure.Mapper
{
    public class FeedbackMapping : Profile
    {
        
        public FeedbackMapping()
        {
            CreateMap<Feedback, ViewFeedbackDTO>()
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Customer.AccountProfile.Phone))
            .ReverseMap();
            CreateMap<Feedback, CreateFeedbackDTO>().ReverseMap();
        }
    }
}
