using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Api.Models
{
    public class UserInstanceModel
    {
        public string InstanceId { get; set; }
        public string AlgoClientId { get; set; }
        public string AlgoId { get; set; }
        public string InstanceName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? RunDate { get; set; }
        public DateTime? StopDate { get; set; }
        public ClientWalletData Wallet { get; set; }
        public AlgoInstanceStatus InstanceStatus { get; set; }
        public AlgoInstanceType InstanceType { get; set; }
    }
}
