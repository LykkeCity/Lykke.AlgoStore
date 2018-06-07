using AutoMapper;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.Api.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AlgoData, AlgoDataModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AlgoId));              

            CreateMap<AlgoEntity, IAlgo>()
                .ForMember(dest => dest.AlgoVisibility, opt => opt.Ignore());

            CreateMap<IAlgo, AlgoEntity>()
                .ForMember(dest => dest.PartitionKey, opt => opt.Ignore())
                .ForMember(dest => dest.RowKey, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.Ignore())
                .ForMember(dest => dest.ETag, opt => opt.Ignore())
                .ForMember(dest => dest.AlgoVisibilityValue, opt => opt.Ignore());

            CreateMap<AlgoData, IAlgo>();

            CreateMap<IAlgo, AlgoData>();        

            CreateMap<AlgoDataInformation, AlgoDataInformationModel>();

            CreateMap<AlgoDataModel, AlgoData>()
                .ForMember(dest => dest.AlgoId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AlgoMetaDataInformationJSON, opt => opt.Ignore());

            CreateMap<UploadAlgoBinaryModel, UploadAlgoBinaryData>();

            CreateMap<UploadAlgoStringModel, UploadAlgoStringData>();

            CreateMap<ManageImageData, ManageImageModel>().ReverseMap();
            CreateMap<TailLogData, TailLogModel>().ReverseMap();

            CreateMap<AlgoClientInstanceData, AlgoClientInstanceModel>()
                .ForSourceMember(src => src.ClientId, opt => opt.Ignore());

            CreateMap<AlgoClientInstanceData, AlgoFakeTradingInstanceModel>()
                .ForSourceMember(src => src.ClientId, opt => opt.Ignore());

            CreateMap<AlgoClientInstanceModel, AlgoClientInstanceData>()
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
                .ForMember(dest => dest.AuthToken, opt => opt.Ignore());

            CreateMap<AlgoFakeTradingInstanceModel, AlgoClientInstanceData>()
                .ForMember(dest => dest.ClientId, opt => opt.Ignore())
                .ForMember(dest => dest.AssetPairId, opt => opt.Ignore())
                .ForMember(dest => dest.HftApiKey, opt => opt.Ignore())
                .ForMember(dest => dest.Margin, opt => opt.Ignore())
                .ForMember(dest => dest.Volume, opt => opt.Ignore())
                .ForMember(dest => dest.TradedAssetId, opt => opt.Ignore())
                .ForMember(dest => dest.IsStraight, opt => opt.Ignore())
                .ForMember(dest => dest.WalletId, opt => opt.Ignore())
                .ForMember(dest => dest.OppositeAssetId, opt => opt.Ignore())
                .ForMember(dest => dest.AuthToken, opt => opt.Ignore());

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

            CreateMap<UserRoleCreateModel, UserRoleData>()
                .ForMember(dest => dest.CanBeDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CanBeModified, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Permissions, opt => opt.Ignore());

            CreateMap<UserRoleUpdateModel, UserRoleData>()
                .ForMember(dest => dest.Permissions, opt => opt.Ignore());

            CreateMap<CreateAlgoModel, AlgoData>()
                .ForMember(dest => dest.AlgoId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AlgoMetaDataInformationJSON, opt => opt.Ignore())
                .ForMember(dest => dest.AlgoVisibility, opt => opt.UseValue(AlgoVisibility.Private))
                .ForMember(dest => dest.ClientId, opt => opt.Ignore())
                .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
                .ForMember(dest => dest.DateModified, opt => opt.Ignore());

            CreateMap<ClientWalletData, ClientWalletDataModel>();
        }
    }
}
