﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoStoreTradesService
    {
        Task<List<AlgoInstanceTrade>> GetAllTradesForAlgoInstanceAsync(string instanceId);
    }
}
