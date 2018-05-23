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
        //    //public static List<AlgoEntity> ToEntity(this AlgoClientMetaData data)
        //    //{
        //    //    var result = new List<AlgoEntity>();

        //    //    if ((data == null) || data.AlgoMetaData.IsNullOrEmptyCollection())
        //    //        return result;

        //    //    foreach (AlgoData algoData in data.AlgoMetaData)
        //    //    {
        //    //        var res = new AlgoEntity();

        //    //        res.PartitionKey = data.ClientId;
        //    //        res.RowKey = algoData.AlgoId;
        //    //        res.Description = algoData.Description;
        //    //        res.Name = algoData.Name;
        //    //        res.Author = data.Author;
        //    //        res.AlgoVisibility = algoData.AlgoVisibility;
        //    //        res.ETag = "*";
        //    //        res.AlgoMetaDataInformationJSON = algoData.AlgoMetaDataInformationJSON;

        //    //        result.Add(res);
        //    //    }

        //    //    return result;
        //    //}
        //    //public static AlgoClientMetaData ToModel(this ICollection<AlgoEntity> entities)
        //    //{
        //    //    var result = new AlgoClientMetaData { AlgoMetaData = new List<AlgoData>() };

        //    //    if ((entities == null) || entities.IsNullOrEmptyCollection())
        //    //        return result;

        //    //    foreach (var algoEntity in entities)
        //    //    {
        //    //        if (algoEntity == null)
        //    //            continue;

        //    //        result.ClientId = algoEntity.PartitionKey;
        //    //        if (!string.IsNullOrWhiteSpace(algoEntity.Author))
        //    //            result.Author = algoEntity.Author;
        //    //        result.AlgoMetaData.Add(algoEntity.ToAlgoMetaData());
        //    //    }

        //    //    return result;
        //    //}


        //    public static List<AlgoData> ToModel(this ICollection<AlgoEntity> entities)
        //    {
        //        var result = new List<AlgoData>();

        //        if ((entities == null) || entities.IsNullOrEmptyCollection())
        //            return result;

        //        foreach (var algoEntity in entities)
        //        {
        //            if (algoEntity == null)
        //                continue;

        //            result.Add(algoEntity.ToAlgoData());
        //        }

        //        return result;
        //    }

        //    public static AlgoData ToAlgoData(this AlgoEntity entity)
        //    {
        //        var result = new AlgoData();
        //        result.ClientId = entity.PartitionKey;
        //        result.AlgoId = entity.RowKey;
        //        result.Description = entity.Description;
        //        result.Name = entity.Name;
        //        result.Date = entity.Timestamp.DateTime.ToString("yyyy-dd-MM HH:mm:ss");
        //        result.AlgoVisibility = entity.AlgoVisibility;
        //        result.AlgoMetaDataInformationJSON = entity.AlgoMetaDataInformationJSON;

        //        return result;
        //    }

        public static AlgoClientMetaDataInformation ToAlgoMetaInformation(this AlgoEntity entity)
        {
            var result = new AlgoClientMetaDataInformation();

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
