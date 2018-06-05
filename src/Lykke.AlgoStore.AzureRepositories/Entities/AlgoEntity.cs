﻿using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Enumerators;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Lykke.AlgoStore.AzureRepositories.Entities
{
    public class AlgoEntity : TableEntity, IAlgo
    {
        public string AlgoId
        {
            get => RowKey;
            set { }
        }

        public string ClientId
        {
            get => PartitionKey;
            set { }
        }

        public string Name { get; set; }
        public DateTime DateModified => Timestamp.UtcDateTime;
        public DateTime DateCreated { get; set; }


        public string Description { get; set; }
        public string AlgoVisibilityValue { get; set; }
        public string AlgoMetaDataInformationJSON { get; set; }

        public AlgoVisibility AlgoVisibility
        {
            get
            {
                Enum.TryParse(AlgoVisibilityValue, out AlgoVisibility visibility);
                return visibility;
            }
            set => AlgoVisibilityValue = value.ToString();
        }

        public static AlgoEntity Create(IAlgo algo)
        {
            AlgoEntity enitity = AutoMapper.Mapper.Map<AlgoEntity>(algo);

            enitity.PartitionKey = algo.ClientId;
            enitity.RowKey = algo.AlgoId;

            return enitity;
        }
    }
}
