using AutoMapper;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
        CreateMap<AlgoMetaData, AlgoMetaDataModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ClientAlgoId));

            CreateMap<AlgoMetaDataModel, AlgoMetaData>()
                .ForMember(dest => dest.ClientAlgoId, opt => opt.MapFrom(src => src.Id));
            CreateMap<AlgoMetaData, AlgoMetaDataModel>();
            CreateMap<AlgoMetaDataModel, AlgoMetaData>();
            CreateMap<AlgoTemplateModel, AlgoTemplateData>();
            CreateMap<AlgoTemplateData, AlgoTemplateModel>();
            CreateMap<AlgoData, AlgoDataModel>();
            CreateMap<AlgoDataModel, AlgoData>();
            CreateMap<TradingAssetDataModel, TradingAssetData>();
            CreateMap<TradingAssetData, TradingAssetDataModel>();
            CreateMap<TradingAmountDataModel, TradingAmountData>();
            CreateMap<TradingAmountData, TradingAmountDataModel>();
            CreateMap<AlgoRuntimeDataModel, AlgoRuntimeData>();
            CreateMap<AlgoRuntimeData, AlgoRuntimeDataModel>();

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
