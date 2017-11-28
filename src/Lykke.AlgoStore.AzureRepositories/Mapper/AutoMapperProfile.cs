using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using AutoMapper;
using Common;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    //REMARK: This is work in progress.
    //If we decide to do it like below we still need to do mappings from AlgoTemplateDataMapper
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //To entities
            CreateMap<AlgoData, AlgoDataEntity>()
                .ForMember(dest => dest.RowKey, opt => opt.MapFrom(src => src.ClientAlgoId));

            CreateMap<AlgoClientMetaData, List<AlgoMetaDataEntity>>()
                .ConvertUsing(src => src.AlgoMetaData?.Select(x => new AlgoMetaDataEntity
                {
                    RowKey = x.ClientAlgoId,
                    ClientId = src.ClientId,
                    Description = x.Description,
                    Name = x.Name,
                    TemplateId = x.TemplateId
                }).ToList());

            CreateMap<AlgoClientRuntimeData, List<AlgoRuntimeDataEntity>>()
                .ConvertUsing(src => src.RuntimeData?.Select(x => new AlgoRuntimeDataEntity
                {
                    ClientAlgoId = src.ClientAlgoId,
                    RowKey = x.ImageId,
                    Version = x.Version,
                    AssetAccuracy = x.Asset.Accuracy,
                    AssetBaseAssetId = x.Asset.BaseAssetId,
                    AssetId = x.Asset.Id,
                    AssetInvertedAccuracy = x.Asset.InvertedAccuracy,
                    AssetName = x.Asset.Name,
                    AssetQuotingAssetId = x.Asset.QuotingAssetId,
                    TradingAmountAmount = x.TradingAmount.Amount,
                    TradingAmountAssetId = x.TradingAmount.AssetId
                }).ToList());

            //

            ForAllMaps((map, cfg) =>
            {
                if (map.DestinationType.IsSubclassOf(typeof(TableEntity)))
                {
                    cfg.ForMember("ETag", opt => opt.Ignore());
                    cfg.ForMember("PartitionKey", opt => opt.Ignore());
                    cfg.ForMember("RowKey", opt => opt.Ignore());
                    cfg.ForMember("Timestamp", opt => opt.Ignore());
                }
            });

            //From entities
            CreateMap<AlgoDataEntity, AlgoData>()
                .ForMember(dest => dest.ClientAlgoId, opt => opt.MapFrom(src => src.RowKey));

            CreateMap<AlgoTemplateDataEntity, AlgoTemplateData>()
                .ForMember(dest => dest.TemplateId, opt => opt.MapFrom(src => src.RowKey));

            CreateMap<AlgoMetaDataEntity, AlgoMetaData>()
                .ForMember(dest => dest.ClientAlgoId, opt => opt.MapFrom(src => src.RowKey));

            CreateMap<List<AlgoMetaDataEntity>, AlgoClientMetaData>()
                .ConvertUsing(
                    src =>
                    {
                        var response = new AlgoClientMetaData
                        {
                            AlgoMetaData = new List<AlgoMetaData>()
                        };

                        foreach (var algoEntity in src)
                        {
                            if (algoEntity != null)
                            {
                                response.ClientId = algoEntity.ClientId;
                                response.AlgoMetaData.Add(AutoMapper.Mapper.Map<AlgoMetaData>(algoEntity));
                            }
                        }

                        return response;
                    });

            CreateMap<AlgoRuntimeDataEntity, TradingAmountData>()
                .ForMember(dest => dest.AssetId, opt => opt.MapFrom(src => src.TradingAmountAssetId))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.TradingAmountAmount));

            CreateMap<AlgoRuntimeDataEntity, TradingAssetData>()
                .ForMember(dest => dest.Accuracy, opt => opt.MapFrom(src => src.AssetAccuracy))
                .ForMember(dest => dest.BaseAssetId, opt => opt.MapFrom(src => src.AssetBaseAssetId))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AssetId))
                .ForMember(dest => dest.InvertedAccuracy, opt => opt.MapFrom(src => src.AssetAccuracy))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AssetName))
                .ForMember(dest => dest.QuotingAssetId, opt => opt.MapFrom(src => src.AssetQuotingAssetId));

            CreateMap<AlgoRuntimeDataEntity, AlgoRuntimeData>()
                .ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => src.RowKey))
                .ForMember(dest => dest.Asset, opt => opt.MapFrom(src => AutoMapper.Mapper.Map<TradingAssetData>(src)))
                .ForMember(dest => dest.TradingAmount, opt => opt.MapFrom(src => AutoMapper.Mapper.Map<TradingAmountData>(src)));

            CreateMap<List<AlgoRuntimeDataEntity>, AlgoClientRuntimeData>()
                .ConvertUsing(
                    src =>
                    {
                        var response = new AlgoClientRuntimeData { RuntimeData = new List<AlgoRuntimeData>() };

                        foreach (var algoEntity in src)
                        {
                            if (algoEntity != null)
                            {
                                response.ClientAlgoId = algoEntity.ClientAlgoId;
                                response.RuntimeData.Add(AutoMapper.Mapper.Map<AlgoRuntimeData>(algoEntity));
                            }
                        }

                        return response;
                    });
        }
    }
}
