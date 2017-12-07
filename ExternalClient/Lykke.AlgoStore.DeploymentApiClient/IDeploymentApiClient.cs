﻿using System.Threading.Tasks;

namespace Lykke.AlgoStore.DeploymentApiClient
{
    public interface IDeploymentApiClient : IDeploymentApiReadOnlyClient
    {
        Task<string> BuildAlgoImageFromBinary(byte[] data, string algoUsername, string algoName);
        Task<bool> CreateTestAlgo(long imageId, string algoId);
        Task<bool> StartTestAlgo(long imageId);
        Task<bool> StopTestAlgo(long imageId);
    }
}
