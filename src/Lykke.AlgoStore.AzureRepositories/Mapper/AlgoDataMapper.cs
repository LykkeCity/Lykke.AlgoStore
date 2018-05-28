using System.Collections.Generic;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class AlgoDataMapper
    {
        public static AlgoDataInformation ToAlgoDataInformation(this AlgoEntity entity)
        {
            var result = new AlgoDataInformation();

            result.AlgoId = entity.RowKey;
            result.Description = entity.Description;
            result.Name = entity.Name;
            result.Date = entity.Timestamp.DateTime.ToString("yyyy-dd-MM HH:mm:ss");
            result.AlgoVisibility = entity.AlgoVisibility;

            if (!string.IsNullOrEmpty(entity.AlgoMetaDataInformationJSON))
            {
                result.AlgoMetaDataInformation = JsonConvert.DeserializeObject<AlgoMetaDataInformation>(entity.AlgoMetaDataInformationJSON);
            }

            return result;
        }
    }
}
