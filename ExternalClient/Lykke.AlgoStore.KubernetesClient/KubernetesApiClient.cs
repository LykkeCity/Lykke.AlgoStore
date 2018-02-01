using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Lykke.AlgoStore.KubernetesClient.Models;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.KubernetesClient
{
    public class KubernetesApiClient : Kubernetes, IKubernetesApiClient
    {

        public async Task<IList<Iok8skubernetespkgapiv1Pod>> ListPodsByAlgoIdAsync(string algoId)
        {
            using (var kubeResponse =
                await ListCoreV1PodForAllNamespacesWithHttpMessagesAsync(null, true, "app=" + algoId))
            {
                if (!kubeResponse.Response.IsSuccessStatusCode || kubeResponse.Body == null ||
                    kubeResponse.Body.Items == null)
                    return null;
                return kubeResponse.Body.Items;
            }
        }

        public async Task<Iok8sapimachinerypkgapismetav1Status> DeleteDeploymentAsync(string algoId,
            Iok8skubernetespkgapiv1Pod pod)
        {
            var options = new Iok8sapimachinerypkgapismetav1DeleteOptions();

            using (var kubeResponse =
                await DeleteAppsV1beta1NamespacedDeploymentWithHttpMessagesAsync(options, algoId,
                    pod.Metadata.NamespaceProperty))
            {
                if (!kubeResponse.Response.IsSuccessStatusCode || kubeResponse.Body == null)
                    return null;
                return kubeResponse.Body;
            }
        }

        public async Task<string> ReadPodLogAsync(Iok8skubernetespkgapiv1Pod pod, int? tailLines)
        {
            using (var kubeResponse = await ReadPodLogWithHttpMessagesAsync(pod.Metadata.Name, pod.Metadata.NamespaceProperty).ConfigureAwait(false))
            {
                if (!kubeResponse.Response.IsSuccessStatusCode || kubeResponse.Body == null)
                    return null;
                return kubeResponse.Body;
            }
        }

        /// <summary>
        /// This method is similar as <see cref="Kubernetes.ReadCoreV1NamespacedPodLogWithHttpMessagesAsync"/>
        /// parameters are same, but code is changed cause API return 201 and in swagger and docs it is stated that API will return 200.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="namespaceParameter"></param>
        /// <param name="container"></param>
        /// <param name="follow"></param>
        /// <param name="limitBytes"></param>
        /// <param name="pretty"></param>
        /// <param name="previous"></param>
        /// <param name="sinceSeconds"></param>
        /// <param name="tailLines"></param>
        /// <param name="timestamps"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<HttpOperationResponse<string>> ReadPodLogWithHttpMessagesAsync(string name,
            string namespaceParameter, string container = default(string), bool? follow = default(bool?),
            int? limitBytes = default(int?), string pretty = default(string), bool? previous = default(bool?),
            int? sinceSeconds = default(int?), int? tailLines = default(int?), bool? timestamps = default(bool?),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "name");
            }
            if (namespaceParameter == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "namespaceParameter");
            }
            // Construct URL
            var baseUrl = BaseUri.AbsoluteUri;
            var url = new System.Uri(new System.Uri(baseUrl + (baseUrl.EndsWith("/") ? "" : "/")),
                "api/v1/namespaces/{namespace}/pods/{name}/log").ToString();
            url = url.Replace("{name}", System.Uri.EscapeDataString(name));
            url = url.Replace("{namespace}", System.Uri.EscapeDataString(namespaceParameter));
            var queryParameters = new List<string>();
            if (container != null)
            {
                queryParameters.Add(string.Format("container={0}", System.Uri.EscapeDataString(container)));
            }
            if (follow != null)
            {
                queryParameters.Add(string.Format("follow={0}",
                    System.Uri.EscapeDataString(
                        SafeJsonConvert.SerializeObject(follow, SerializationSettings).Trim('"'))));
            }
            if (limitBytes != null)
            {
                queryParameters.Add(string.Format("limitBytes={0}",
                    System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(limitBytes, SerializationSettings)
                        .Trim('"'))));
            }
            if (pretty != null)
            {
                queryParameters.Add(string.Format("pretty={0}", System.Uri.EscapeDataString(pretty)));
            }
            if (previous != null)
            {
                queryParameters.Add(string.Format("previous={0}",
                    System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(previous, SerializationSettings)
                        .Trim('"'))));
            }
            if (sinceSeconds != null)
            {
                queryParameters.Add(string.Format("sinceSeconds={0}",
                    System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(sinceSeconds, SerializationSettings)
                        .Trim('"'))));
            }
            if (tailLines != null)
            {
                queryParameters.Add(string.Format("tailLines={0}",
                    System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(tailLines, SerializationSettings)
                        .Trim('"'))));
            }
            if (timestamps != null)
            {
                queryParameters.Add(string.Format("timestamps={0}",
                    System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(timestamps, SerializationSettings)
                        .Trim('"'))));
            }
            if (queryParameters.Count > 0)
            {
                url += "?" + string.Join("&", queryParameters);
            }
            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage();
            httpRequest.Method = new HttpMethod("GET");
            httpRequest.RequestUri = new System.Uri(url);

            // Send Request
            cancellationToken.ThrowIfCancellationRequested();
            var httpResponse = await HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            HttpStatusCode statusCode = httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string responseContent;
            if ((int)statusCode != 200 && (int)statusCode != 401)
            {
                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'",
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
                if (httpResponse != null)
                {
                    httpResponse.Dispose();
                }
                throw ex;
            }
            // Create Result
            var result = new HttpOperationResponse<string>();
            result.Request = httpRequest;
            result.Response = httpResponse;
            // Deserialize Response
            if ((int)statusCode == 200)
            {
                responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    result.Body = responseContent;// SafeJsonConvert.DeserializeObject<string>(_responseContent, DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    httpRequest.Dispose();
                    if (httpResponse != null)
                    {
                        httpResponse.Dispose();
                    }
                    throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                }
            }
            return result;
        }
    }
}
