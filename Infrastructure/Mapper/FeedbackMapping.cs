using AutoMapper;
using Domain;
using static Infrastructure.ViewModel.Response.FeedbackResponse;

namespace Infrastructure.Mapper
{
    public class FeedbackMapping : Profile
    {
        
        public FeedbackMapping()
        {
            CreateMap<Feedback, ViewFeedbackDTO>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Customer.AccountProfile.Email))
            .ReverseMap();
        }
    }
}
