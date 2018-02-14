using AutoMapper;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Domain.Entities;

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
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AlgoId))
            //.ForMember(dest => dest.Author, opt => opt.Ignore());

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
            CreateMap<AlgoClientInstanceModel, AlgoClientInstanceData>()
                .ForMember(dest => dest.ClientId, opt => opt.Ignore());

            CreateMap<AlgoRatingMetaDataModel, AlgoRatingMetaData>()
                .IncludeBase<AlgoMetaDataModel, AlgoMetaData>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author));

            CreateMap<AlgoRatingMetaData, AlgoRatingMetaDataModel>()
                .IncludeBase<AlgoMetaData, AlgoMetaDataModel>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForSourceMember(src => src.AlgoMetaDataInformationJSON, opt => opt.Ignore());
        }
    }
}
