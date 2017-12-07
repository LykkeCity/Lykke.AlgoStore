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

        public async Task<ClientAlgoRuntimeStatuses> GetAlgoTestStatus(long id)
        {
            HttpOperationResponse<string> response = await _externalClient.GetTestAlgoStatusUsingGETWithHttpMessagesAsync(id);

            if (response.Response.StatusCode != HttpStatusCode.OK)
                return MapToStatusEnum(response.Response.StatusCode);

            return MapToStatusEnum(response.Body);
        }

        private static ClientAlgoRuntimeStatuses MapToStatusEnum(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    return ClientAlgoRuntimeStatuses.NotFound;
                case HttpStatusCode.Forbidden:
                    return ClientAlgoRuntimeStatuses.Forbidden;
                case HttpStatusCode.Unauthorized:
                    return ClientAlgoRuntimeStatuses.Unauthorized;
                default:
                    return ClientAlgoRuntimeStatuses.InternalError;
            }
        }
        private static ClientAlgoRuntimeStatuses MapToStatusEnum(string status)
        {
            switch (status.ToUpper())
            {
                case "RUNNING":
                    return ClientAlgoRuntimeStatuses.Running;
                case "STOPPED":
                    return ClientAlgoRuntimeStatuses.Stopped;
                case "PAUSED":
                    return ClientAlgoRuntimeStatuses.Paused;
                case "CREATED":
                    return ClientAlgoRuntimeStatuses.Created;
                default:
                    return ClientAlgoRuntimeStatuses.Success;
            }
        }
    }
}
