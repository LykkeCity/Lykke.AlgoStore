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
        public async Task<bool> DeleteAlgo(long imageId)
        {
            var response = await _externalClient.RemoveALgoUsingDELETEWithHttpMessagesAsync(imageId);

            return response.Response.StatusCode == HttpStatusCode.OK;
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
        public async Task<bool> StopTestAlgo(long imageId)
        {
            var response = await _externalClient.StopTestAlgoUsingPUTWithHttpMessagesAsync(imageId);

            return response.Response.StatusCode == HttpStatusCode.OK;
        }
        public async Task<bool> DeleteTestAlgo(long imageId)
        {
            var response = await _externalClient.DeleteTestAlgoUsingDELETEWithHttpMessagesAsync(imageId);

            return response.Response.StatusCode == HttpStatusCode.OK;
        }
        public async Task<string> GetTestAlgoLog(long imageId)
        {
            var response = await _externalClient.GetAlgoLogUsingGET.OneWithHttpMessagesAsync(imageId);

            return response.Body;
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
            if (string.IsNullOrWhiteSpace(status))
                return ClientAlgoRuntimeStatuses.NotFound;

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
