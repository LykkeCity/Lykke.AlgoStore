using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.DataTypes
{
    public class BaseDataModel
    {
        [IgnoreMember]
        public string InstanceId { get; set; }
    }
}
