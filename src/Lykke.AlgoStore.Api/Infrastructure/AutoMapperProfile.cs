using AutoMapper;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.Api.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AlgoMetaData, AlgoMetaDataModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AlgoId))
                .ForMember(dest => dest.Author, opt => opt.Ignore());


            CreateMap<AlgoClientMetaDataInformation, AlgoClientMetaDataInformationModel>();

            CreateMap<AlgoMetaDataModel, AlgoMetaData>()
                .ForMember(dest => dest.AlgoId, opt => opt.MapFrom(src => src.Id))
                .ForSourceMember(src => src.Author, opt => opt.Ignore())
            .ForMember(dest => dest.AlgoMetaDataInformationJSON, opt => opt.Ignore());

            CreateMap<UploadAlgoBinaryModel, UploadAlgoBinaryData>();
            CreateMap<UploadAlgoStringModel, UploadAlgoStringData>();

            CreateMap<ManageImageData, ManageImageModel>().ReverseMap();
            CreateMap<TailLogData, TailLogModel>().ReverseMap();

            CreateMap<AlgoClientInstanceData, AlgoClientInstanceModel>()
                .ForSourceMember(src => src.ClientId, opt => opt.Ignore());

            CreateMap<AlgoClientInstanceData, AlgoBackTestInstanceModel>()
                .ForSourceMember(src => src.ClientId, opt => opt.Ignore());

            CreateMap<AlgoClientInstanceModel, AlgoClientInstanceData>()
                .ForMember(dest => dest.ClientId, opt => opt.Ignore())
                .ForMember(dest => dest.AssetPair, opt => opt.Ignore())
                .ForMember(dest => dest.HftApiKey, opt => opt.Ignore())
                .ForMember(dest => dest.Margin, opt => opt.Ignore())
                .ForMember(dest => dest.Volume, opt => opt.Ignore())
                .ForMember(dest => dest.TradedAsset, opt => opt.Ignore())
                .ForMember(dest => dest.IsStraight, opt => opt.Ignore())
                .ForMember(dest => dest.BackTestTradingAssetBalance, opt => opt.Ignore())
                .ForMember(dest => dest.BackTestAssetTwoBalance, opt => opt.Ignore());

            CreateMap<AlgoBackTestInstanceModel, AlgoClientInstanceData>()
                .ForMember(dest => dest.ClientId, opt => opt.Ignore())
                .ForMember(dest => dest.AssetPair, opt => opt.Ignore())
                .ForMember(dest => dest.HftApiKey, opt => opt.Ignore())
                .ForMember(dest => dest.Margin, opt => opt.Ignore())
                .ForMember(dest => dest.Volume, opt => opt.Ignore())
                .ForMember(dest => dest.TradedAsset, opt => opt.Ignore())
                .ForMember(dest => dest.IsStraight, opt => opt.Ignore())
                .ForMember(dest => dest.WalletId, opt => opt.Ignore());

            CreateMap<AlgoRatingMetaDataModel, AlgoRatingMetaData>()
                .IncludeBase<AlgoMetaDataModel, AlgoMetaData>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.RatedUsersCount, opt => opt.MapFrom(src => src.RatedUsersCount));

            CreateMap<AlgoRatingMetaData, AlgoRatingMetaDataModel>()
                .IncludeBase<AlgoMetaData, AlgoMetaDataModel>()
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
        }
    }
}
