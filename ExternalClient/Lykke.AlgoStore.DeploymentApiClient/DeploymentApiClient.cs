using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Lykke.AlgoStore.DeploymentApiClient.Models;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.DeploymentApiClient
{
    public class DeploymentApiClient : ApiDocumentation, IDeploymentApiClient
    {
        public async Task<string> BuildAlgoImageFromBinary(byte[] data, string algoUsername, string algoName)
        {
            using (var stream = new MemoryStream(data))
            {
                var response = await BuildAlgoImageAsync(algoUsername, algoName, stream);

                return response.Body.Id.ToString();
            }
        }
        public async Task<bool> DeleteAlgo(long imageId)
        {
            var response = await RemoveALgoUsingDELETEWithHttpMessagesAsync(imageId);

            return response.Response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<long> CreateTestAlgo(long imageId, string algoId)
        {
            var response = await TestAlgoUsingPOSTWithHttpMessagesAsync(imageId, algoId);

            AlgoTest test = response.Body;
            if ((response.Response.StatusCode == HttpStatusCode.OK)
                && test != null
                && MapToStatusEnum(test.Status) == ClientAlgoRuntimeStatuses.Created)
                return test.Id.GetValueOrDefault();

            return 0;
        }
        public async Task<bool> StartTestAlgo(long imageId)
        {
            var response = await StartUsingPUTWithHttpMessagesAsync(imageId);

            return response.Response.StatusCode == HttpStatusCode.OK;
        }
        public async Task<bool> StopTestAlgo(long imageId)
        {
            var response = await StopTestAlgoUsingPUTWithHttpMessagesAsync(imageId);

            return response.Response.StatusCode == HttpStatusCode.OK;
        }
        public async Task<bool> DeleteTestAlgo(long imageId)
        {
            var response = await DeleteTestAlgoUsingDELETEWithHttpMessagesAsync(imageId);

            return response.Response.StatusCode == HttpStatusCode.OK;
        }
        public async Task<string> GetTestAlgoLog(long imageId)
        {
            var response = await GetAlgoLogUsingGETWithHttpMessagesAsync(imageId);

            return response.Body.Log;
        }
        public async Task<string> GetTestAlgoTailLog(long imageId, int tail)
        {
            var response = await GetTailLogUsingGETWithHttpMessagesAsync(imageId, tail);

            return response.Body.Log;
        }
        public async Task<ClientAlgoRuntimeStatuses> GetAlgoTestAdministrativeStatus(long id)
        {
            HttpOperationResponse<AdminStatusResponse> response = await GetAdministrativeStatusAsync(id);

            if (response.Response.StatusCode != HttpStatusCode.OK)
                return MapToStatusEnum(response.Response.StatusCode);

            return MapToStatusEnum(response.Body.Status);
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
        private async Task<HttpOperationResponse<AdminStatusResponse>> GetAdministrativeStatusAsync(long id)
        {
            // Construct URL
            var baseUrl = BaseUri.AbsoluteUri;
            var url = new System.Uri(new System.Uri(baseUrl + (baseUrl.EndsWith("/") ? "" : "/")), "algo/test/{id}/getAdministrativeStatus").ToString();
            url = url.Replace("{id}", System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(id, SerializationSettings).Trim('"')));
            // Create HTTP transport objects
            HttpOperationResponse<AdminStatusResponse> result;
            using (var httpRequest = new HttpRequestMessage())
            {
                httpRequest.Method = new HttpMethod("GET");
                httpRequest.RequestUri = new System.Uri(url);

                using (var httpResponse = await HttpClient.SendAsync(httpRequest, CancellationToken.None).ConfigureAwait(false))
                {
                    // Create Result
                    result = new HttpOperationResponse<AdminStatusResponse>();
                    result.Request = httpRequest;
                    result.Response = httpResponse;
                    // Deserialize Response
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                        try
                        {
                            result.Body = SafeJsonConvert.DeserializeObject<AdminStatusResponse>(responseContent, DeserializationSettings);
                        }
                        catch (JsonException ex)
                        {
                            throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                        }
                    }
                }
            }
            return result;
        }
        private async Task<HttpOperationResponse<Algo>> BuildAlgoImageAsync(string algoUserName, string algoName, MemoryStream file)
        {
            if (file == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "file");
            }
            if (algoUserName == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "algoUserName");
            }
            if (algoName == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "algoName");
            }

            // Construct URL
            var baseUrl = BaseUri.AbsoluteUri;
            var url = new System.Uri(new System.Uri(baseUrl + (baseUrl.EndsWith("/") ? "" : "/")), "algo/build/upload-java-binary").ToString();
            var queryParameters = new List<string>();

            queryParameters.Add(string.Format("algoUserName={0}", System.Uri.EscapeDataString(algoUserName)));
            queryParameters.Add(string.Format("algoName={0}", System.Uri.EscapeDataString(algoName)));

            if (queryParameters.Count > 0)
            {
                url += "?" + string.Join("&", queryParameters);
            }
            // Create HTTP transport objects
            using (var httpRequest = new HttpRequestMessage())
            {
                httpRequest.Method = new HttpMethod("POST");
                httpRequest.RequestUri = new System.Uri(url);

                // Serialize Request
                var multiPartContent = new MultipartFormDataContent();

                var streamContent = new StreamContent(file);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var contentDispositionHeaderValue = new ContentDispositionHeaderValue("form-data");
                contentDispositionHeaderValue.Name = "file";
                contentDispositionHeaderValue.FileName = algoName;
                streamContent.Headers.ContentDisposition = contentDispositionHeaderValue;
                multiPartContent.Add(streamContent, "file");

                httpRequest.Content = multiPartContent;

                using (var httpResponse =
                    await HttpClient.SendAsync(httpRequest, CancellationToken.None).ConfigureAwait(false))
                {
                    HttpStatusCode statusCode = httpResponse.StatusCode;
                    string responseContent;
                    if ((int)statusCode != 200 && (int)statusCode != 201 && (int)statusCode != 401 &&
                        (int)statusCode != 403 && (int)statusCode != 404)
                    {
                        var ex = new HttpOperationException(string.Format(
                            "Operation returned an invalid status code '{0}'",
                            statusCode));
                        if (httpResponse.Content != null)
                        {
                            responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                        }
                        else
                        {
                            responseContent = string.Empty;
                        }
                        ex.Request = new HttpRequestMessageWrapper(httpRequest, null);
                        ex.Response = new HttpResponseMessageWrapper(httpResponse, responseContent);
                        httpRequest.Dispose();
                        throw ex;
                    }
                    // Create Result
                    var result = new HttpOperationResponse<Algo>();
                    result.Request = httpRequest;
                    result.Response = httpResponse;
                    // Deserialize Response
                    if ((int)statusCode == 200)
                    {
                        responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                        try
                        {
                            result.Body =
                                SafeJsonConvert.DeserializeObject<Algo>(responseContent, DeserializationSettings);
                        }
                        catch (JsonException ex)
                        {
                            httpRequest.Dispose();
                            throw new SerializationException("Unable to deserialize the response.", responseContent,
                                ex);
                        }
                    }
                    return result;
                }
            }
        }
    }
}
