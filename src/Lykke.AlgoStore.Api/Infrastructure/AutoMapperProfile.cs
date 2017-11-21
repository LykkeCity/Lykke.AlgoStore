using AutoMapper;

namespace Lykke.AlgoStore.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //CreateMap<ApiRequests.AuthRequestModel, AuthModel>()
            //    .ForMember(dest => dest.UserAgent, opt => opt.Ignore())
            //    .ForMember(dest => dest.Ip, opt => opt.Ignore());

            //CreateMap<AuthResponse, AuthResponseModel>()
            //    .ForMember(dest => dest.AccessToken, opt => opt.MapFrom(src => src.Token));

            //CreateMap<ApiRequests.CreatePledgeRequest, ClientModel.CreatePledgeRequest>()
            //    .ForMember(dest => dest.ClientId, opt => opt.Ignore());

            //CreateMap<ClientModel.CreatePledgeResponse, ApiResponses.CreatePledgeResponse>();
            //CreateMap<ClientModel.GetPledgeResponse, ApiResponses.GetPledgeResponse>();
            //CreateMap<ApiRequests.UpdatePledgeRequest, ClientModel.UpdatePledgeRequest>();
            //CreateMap<ClientModel.UpdatePledgeResponse, ApiResponses.UpdatePledgeResponse>();
        }
    }
}
