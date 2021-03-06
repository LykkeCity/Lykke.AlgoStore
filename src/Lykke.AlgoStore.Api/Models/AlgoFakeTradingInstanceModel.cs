﻿using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Newtonsoft.Json;
using System;

namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoFakeTradingInstanceModel
    {
        public string InstanceId { get; set; }
        public string AlgoId { get; set; }
        public string InstanceName { get; set; }        
        public double FakeTradingTradingAssetBalance { get; set; }
        public double FakeTradingAssetTwoBalance { get; set; }
        [JsonConverter(typeof(DefaultDateTimeConverter))]
        public DateTime? AlgoInstanceRunDate { get; set; }
        public bool IsAlgoInstanceDeployed{ get; set; }
        public AlgoMetaDataInformationModel AlgoMetaDataInformation { get; set; }
        public AlgoInstanceStatus AlgoInstanceStatus { get; set; }
        public AlgoInstanceType AlgoInstanceType { get; set; }
    }
}
