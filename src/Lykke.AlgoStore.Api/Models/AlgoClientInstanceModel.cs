using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;
using Lykke.AlgoStore.Core.Utils;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoClientInstanceModel
    {
        public string InstanceId { get; set; }
        public string WalletId { get; set; }
        public string AlgoId { get; set; }
        public string AlgoClientId { get; set; }
        public string InstanceName { get; set; }
        public AlgoMetaDataInformationModel AlgoMetaDataInformation { get; set; }
        public AlgoInstanceStatus AlgoInstanceStatus { get; set; }
        public AlgoInstanceType AlgoInstanceType { get; set; }

        [JsonConverter(typeof(DefaultDateTimeConverter))]
        public DateTime? AlgoInstanceRunDate { get; set; }
    }
}
