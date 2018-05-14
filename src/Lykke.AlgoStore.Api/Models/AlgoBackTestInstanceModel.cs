﻿using System;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;

namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoBackTestInstanceModel
    {
        public string InstanceId { get; set; }
        public string AlgoId { get; set; }
        public string AlgoClientId { get; set; }
        public string InstanceName { get; set; }        
        public double BacktestTradingAssetBalance { get; set; }
        public double BacktestAssetTwoBalance { get; set; }
        public DateTime? AlgoInstanceRunDate { get; set; }

        public AlgoMetaDataInformationModel AlgoMetaDataInformation { get; set; }
        public AlgoInstanceStatus AlgoInstanceStatus { get; set; }
        public AlgoInstanceType AlgoInstanceType { get; set; }
    }
}