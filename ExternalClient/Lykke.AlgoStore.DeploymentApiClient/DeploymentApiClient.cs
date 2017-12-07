using System.IO;
using System.Net;
using System.Threading.Tasks;
using Lykke.AlgoStore.DeploymentApiClient.Models;
using Microsoft.Rest;

namespace Lykke.AlgoStore.DeploymentApiClient
{
    public class DeploymentApiClient : IDeploymentApiClient
    {
        private readonly IApiDocumentation _externalClient;

        public DeploymentApiClient(IApiDocumentation externalClient)
        {
            _externalClient = externalClient;
        }

        public async Task<string> BuildAlgoImageFromBinary(byte[] data, string algoUsername, string algoName)
        {
            using (var stream = new MemoryStream(data))
            {
                var response = await _externalClient
                    .BuildAlgoImageFromBinaryUsingPOSTWithHttpMessagesAsync(stream, algoUsername, algoName);

                return response.Body.Id.ToString();
            }
        }

        public async Task<bool> CreateTestAlgo(long imageId, string algoId)
        {
            var response = await _externalClient.TestAlgoUsingPUTWithHttpMessagesAsync(imageId, algoId);

            if ((response.Response.StatusCode == HttpStatusCode.OK)
                && response.Body != null)
                return true;

            return false;
        }
        public async Task<bool> StartTestAlgo(long imageId)
        {
            var response = await _externalClient.StartUsingPUTWithHttpMessagesAsync(imageId);

            return response.Response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<AlgoRuntimeStatuses> GetAlgoTestStatus(long id)
        {
            HttpOperationResponse<string> response = await _externalClient.GetTestAlgoStatusUsingGETWithHttpMessagesAsync(id);

            if (response.Response.StatusCode != HttpStatusCode.OK)
                return MapToStatusEnum(response.Response.StatusCode);

            return MapToStatusEnum(response.Body);
        }

        private static AlgoRuntimeStatuses MapToStatusEnum(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    return AlgoRuntimeStatuses.NotFound;
                case HttpStatusCode.Forbidden:
                    return AlgoRuntimeStatuses.Forbidden;
                case HttpStatusCode.Unauthorized:
                    return AlgoRuntimeStatuses.Unauthorized;
                default:
                    return AlgoRuntimeStatuses.InternalError;
            }
        }
        private static AlgoRuntimeStatuses MapToStatusEnum(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return AlgoRuntimeStatuses.NotFound;

            switch (status.ToUpper())
            {
                case "RUNNING":
                    return AlgoRuntimeStatuses.Running;
                case "STOPPED":
                    return AlgoRuntimeStatuses.Stopped;
                case "PAUSED":
                    return AlgoRuntimeStatuses.Paused;
                case "CREATED":
                    return AlgoRuntimeStatuses.Created;
                default:
                    return AlgoRuntimeStatuses.Success;
            }
        }
    }
}
