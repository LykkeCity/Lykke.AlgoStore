using AutoMapper;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.Service.AlgoTrades.Client.AutorestClient.Models;
using Lykke.Service.Security.Client.AutorestClient.Models;

namespace Lykke.AlgoStore.Api.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AlgoData, AlgoDataModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AlgoId));

            CreateMap<AlgoDataInformation, AlgoDataInformationModel>();

            CreateMap<AlgoDataModel, AlgoData>()
                .ForMember(dest => dest.AlgoId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AlgoMetaDataInformationJSON, opt => opt.Ignore());

            CreateMap<UploadAlgoBinaryModel, UploadAlgoBinaryData>();

            CreateMap<UploadAlgoStringModel, UploadAlgoStringData>();

            CreateMap<ManageImageData, ManageImageModel>().ReverseMap();
            CreateMap<TailLogData, TailLogModel>().ReverseMap();

            CreateMap<AlgoClientInstanceData, AlgoClientInstanceModel>()
                .ForSourceMember(src => src.ClientId, opt => opt.Ignore())
                .ForMember(src => src.IsAlgoInstanceDeployed, opt => opt.Ignore());

            CreateMap<PublicAlgoData, PublicAlgoDataModel>()
                .ForSourceMember(src => src.ClientId, opt => opt.Ignore());

            CreateMap<PublicAlgoDataModel, PublicAlgoData>()
                .ForMember(src => src.ClientId, opt => opt.Ignore());

            CreateMap<AlgoClientInstanceData, AlgoFakeTradingInstanceModel>()
                .ForSourceMember(src => src.ClientId, opt => opt.Ignore())
                .ForMember(src => src.IsAlgoInstanceDeployed, opt => opt.Ignore());

            CreateMap<AlgoClientInstanceModel, AlgoClientInstanceData>()
                .ForSourceMember(src => src.IsAlgoInstanceDeployed, opt => opt.Ignore())
                .ForMember(dest => dest.ClientId, opt => opt.Ignore())
                .ForMember(dest => dest.AssetPairId, opt => opt.Ignore())
                .ForMember(dest => dest.HftApiKey, opt => opt.Ignore())
                .ForMember(dest => dest.Margin, opt => opt.Ignore())
                .ForMember(dest => dest.Volume, opt => opt.Ignore())
                .ForMember(dest => dest.TradedAssetId, opt => opt.Ignore())
                .ForMember(dest => dest.IsStraight, opt => opt.Ignore())
                .ForMember(dest => dest.FakeTradingTradingAssetBalance, opt => opt.Ignore())
                .ForMember(dest => dest.FakeTradingAssetTwoBalance, opt => opt.Ignore())
                .ForMember(dest => dest.OppositeAssetId, opt => opt.Ignore())
                .ForMember(dest => dest.AuthToken, opt => opt.Ignore())
                .ForMember(dest => dest.AlgoInstanceCreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.AlgoInstanceStopDate, opt => opt.Ignore())
                .ForMember(dest => dest.TcBuildId, opt => opt.Ignore());

            CreateMap<AlgoFakeTradingInstanceModel, AlgoClientInstanceData>()
                .ForSourceMember(src => src.IsAlgoInstanceDeployed, opt => opt.Ignore())
                .ForMember(dest => dest.ClientId, opt => opt.Ignore())
                .ForMember(dest => dest.AssetPairId, opt => opt.Ignore())
                .ForMember(dest => dest.HftApiKey, opt => opt.Ignore())
                .ForMember(dest => dest.Margin, opt => opt.Ignore())
                .ForMember(dest => dest.Volume, opt => opt.Ignore())
                .ForMember(dest => dest.TradedAssetId, opt => opt.Ignore())
                .ForMember(dest => dest.IsStraight, opt => opt.Ignore())
                .ForMember(dest => dest.WalletId, opt => opt.Ignore())
                .ForMember(dest => dest.OppositeAssetId, opt => opt.Ignore())
                .ForMember(dest => dest.AuthToken, opt => opt.Ignore())
                .ForMember(dest => dest.AlgoInstanceCreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.AlgoInstanceStopDate, opt => opt.Ignore())
                .ForMember(dest => dest.TcBuildId, opt => opt.Ignore());

            CreateMap<AlgoRatingMetaDataModel, AlgoRatingMetaData>()
                .IncludeBase<AlgoDataModel, AlgoData>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.RatedUsersCount, opt => opt.MapFrom(src => src.RatedUsersCount))
                .ForMember(dest => dest.AlgoVisibility, opt => opt.Ignore());

            CreateMap<AlgoRatingMetaData, AlgoRatingMetaDataModel>()
                .IncludeBase<AlgoData, AlgoDataModel>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.RatedUsersCount, opt => opt.MapFrom(src => src.RatedUsersCount))
                .ForSourceMember(src => src.AlgoMetaDataInformationJSON, opt => opt.Ignore());

            CreateMap<AlgoCommentData, AlgoCommentModel>()
                .ForSourceMember(src => src.Author, opt => opt.Ignore());

            CreateMap<AlgoCommentModel, AlgoCommentData>()
                .ForSourceMember(src => src.Author, opt => opt.Ignore());

            CreateMap<CreateAlgoModel, AlgoData>()
                .ForMember(dest => dest.AlgoId, opt => opt.Ignore())
                .ForMember(dest => dest.AlgoMetaDataInformationJSON, opt => opt.Ignore())
                .ForMember(dest => dest.AlgoVisibility, opt => opt.UseValue(AlgoVisibility.Private))
                .ForMember(dest => dest.ClientId, opt => opt.Ignore())
                .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
                .ForMember(dest => dest.DateModified, opt => opt.Ignore())
                .ForMember(dest => dest.DatePublished, opt => opt.Ignore());

            CreateMap<EditAlgoModel, AlgoData>()
                .ForMember(dest => dest.AlgoId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AlgoMetaDataInformationJSON, opt => opt.Ignore())
                .ForMember(dest => dest.AlgoVisibility, opt => opt.UseValue(AlgoVisibility.Private))
                .ForMember(dest => dest.ClientId, opt => opt.Ignore())
                .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
                .ForMember(dest => dest.DateModified, opt => opt.Ignore())
                .ForMember(dest => dest.DatePublished, opt => opt.Ignore());

            CreateMap<ClientWalletData, ClientWalletDataModel>();

            CreateMap<AlgoStoreUserData, AlgoStoreUserDataModel>();

            CreateMap<Lykke.Service.Security.Client.AutorestClient.Models.UserPermissionModel,
                Models.UserPermissionModel>();
            CreateMap<Lykke.Service.Security.Client.AutorestClient.Models.UserRoleModel, Models.UserRoleModel>();

            CreateMap<UserRoleCreateModel, Lykke.Service.Security.Client.AutorestClient.Models.UserRoleModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Permissions, opt => opt.Ignore());

            CreateMap<TeamCityWebHookResponseModel, TeamCityWebHookResponse>();

            CreateMap<Lykke.Service.CandlesHistory.Client.Models.Candle, Lykke.AlgoStore.Algo.Candle>();
            CreateMap<Lykke.AlgoStore.Algo.Candle, CandleChartingUpdate>()
                .ForMember(dest => dest.InstanceId, opt => opt.Ignore())
                .ForMember(dest => dest.AssetPair, opt => opt.Ignore())
                .ForMember(dest => dest.CandleTimeInterval, opt => opt.Ignore());

            CreateMap<AlgoInstanceTradeResponseModel, TradeChartingUpdate>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AssetPairId, opt => opt.MapFrom(src => src.AssetPair))
                .ForMember(dest => dest.AssetId, opt => opt.MapFrom(src => src.TradedAssetName));

            CreateMap<Service.History.Client.AutorestClient.Models.FunctionChartingUpdate,
               FunctionChartingUpdate>();

            CreateMap<Service.History.Client.AutorestClient.Models.QuoteChartingUpdate,
                QuoteChartingUpdate>();
        }
    }
}
