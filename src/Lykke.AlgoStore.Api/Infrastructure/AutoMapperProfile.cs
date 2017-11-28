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
        }
    }
}
