﻿using System.Linq;
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
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ClientAlgoId))
                    .ForSourceMember(src => src.TemplateId, opt => opt.Ignore());
            CreateMap<AlgoMetaDataModel, AlgoMetaData>()
                .ForMember(dest => dest.ClientAlgoId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TemplateId, opt => opt.Ignore());
            CreateMap<UploadAlgoBinaryModel, UploadAlgoBinaryData>();

            CreateMap<DeployImageData, DeployImageModel>().ReverseMap();

            CreateMap<AlgoClientMetaData, AlgoMetaDataModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AlgoMetaData.First().ClientAlgoId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AlgoMetaData.First().Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.AlgoMetaData.First().Description));
        }
    }
}
