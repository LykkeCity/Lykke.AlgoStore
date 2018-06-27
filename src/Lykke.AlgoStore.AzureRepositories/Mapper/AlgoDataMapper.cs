﻿using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class AlgoDataMapper
    {
        public static AlgoDataInformation ToAlgoDataInformation(this AlgoEntity entity)
        {
           var result = AutoMapper.Mapper.Map<AlgoDataInformation>(entity);

            if (!string.IsNullOrEmpty(entity.AlgoMetaDataInformationJSON))
            {
                result.AlgoMetaDataInformation = JsonConvert.DeserializeObject<AlgoMetaDataInformation>(entity.AlgoMetaDataInformationJSON);
            }

            return result;
        }
    }
}
