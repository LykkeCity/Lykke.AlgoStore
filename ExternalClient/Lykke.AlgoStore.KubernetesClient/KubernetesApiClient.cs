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
        /// <summary>
        /// Lists the pods by algo identifier asynchronous.
        /// </summary>
        /// <param name="algoId">The algo identifier.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Deletes the service and deployment asynchronous.
        /// </summary>
        /// <param name="algoId">The algo identifier.</param>
        /// <param name="pod">The pod.</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string algoId, Iok8skubernetespkgapiv1Pod pod)
        {
            await DeleteServiceAsync(algoId, pod);
            return await DeleteDeploymentAsync(algoId, pod);
        }

        /// <summary>
        /// Deletes the deployment asynchronous.
        /// </summary>
        /// <param name="algoId">The algo identifier.</param>
        /// <param name="pod">The pod.</param>
        /// <returns></returns>
        public async Task<bool> DeleteDeploymentAsync(string algoId, Iok8skubernetespkgapiv1Pod pod)
        {
            var options = new Iok8sapimachinerypkgapismetav1DeleteOptions
            {
                PropagationPolicy = "Foreground"
            };

            using (var kubeResponse =
                await DeleteAppsV1beta1NSDeploymentWithHttpMessagesAsync(options, algoId,
                    pod.Metadata.NamespaceProperty))
            {
                if (!kubeResponse.Response.IsSuccessStatusCode || kubeResponse.Body == null)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Deletes the service asynchronous.
        /// </summary>
        /// <param name="algoId">The algo identifier.</param>
        /// <param name="pod">The pod.</param>
        /// <returns></returns>
        public async Task<bool> DeleteServiceAsync(string algoId, Iok8skubernetespkgapiv1Pod pod)
        {
            using (var kubeResponse =
                await DeleteCoreV1NSServiceWithHttpMessagesAsync(algoId, pod.Metadata.NamespaceProperty))
            {
                if (!kubeResponse.Response.IsSuccessStatusCode || kubeResponse.Body == null)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Reads the pod log asynchronous.
        /// </summary>
        /// <param name="pod">The pod.</param>
        /// <param name="tailLines">The tail lines.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Deletes the deployment with HTTP messages asynchronous.
        /// this is modification of auto generated to avoid deserialization exception
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="name">The name.</param>
        /// <param name="namespaceParameter">The namespace parameter.</param>
        /// <returns></returns>
        /// <exception cref="ValidationException">
        /// body
        /// or
        /// name
        /// or
        /// namespaceParameter
        /// </exception>
        /// <exception cref="SerializationException">Unable to deserialize the response.</exception>
        private async Task<HttpOperationResponse<string>> DeleteAppsV1beta1NSDeploymentWithHttpMessagesAsync(Iok8sapimachinerypkgapismetav1DeleteOptions body, string name, string namespaceParameter)
        {
            if (body == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "body");
            }
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
            var url = new System.Uri(new System.Uri(baseUrl + (baseUrl.EndsWith("/") ? "" : "/")), "apis/apps/v1beta1/namespaces/{namespace}/deployments/{name}").ToString();
            url = url.Replace("{name}", System.Uri.EscapeDataString(name));
            url = url.Replace("{namespace}", System.Uri.EscapeDataString(namespaceParameter));
            List<string> _queryParameters = new List<string>();
            if (_queryParameters.Count > 0)
            {
                url += "?" + string.Join("&", _queryParameters);
            }
            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage();
            HttpResponseMessage _httpResponse = null;
            httpRequest.Method = new HttpMethod("DELETE");
            httpRequest.RequestUri = new System.Uri(url);

            // Serialize Request
            string _requestContent = null;
            if (body != null)
            {
                _requestContent = SafeJsonConvert.SerializeObject(body, SerializationSettings);
                httpRequest.Content = new StringContent(_requestContent, System.Text.Encoding.UTF8);
                httpRequest.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
            }
            _httpResponse = await HttpClient.SendAsync(httpRequest, default(CancellationToken)).ConfigureAwait(false);
            HttpStatusCode statusCode = _httpResponse.StatusCode;
            string responseContent = null;
            if ((int)statusCode != 200 && (int)statusCode != 401)
            {
                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'", statusCode));
                if (_httpResponse.Content != null)
                {
                    responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else
                {
                    responseContent = string.Empty;
                }
                ex.Request = new HttpRequestMessageWrapper(httpRequest, _requestContent);
                ex.Response = new HttpResponseMessageWrapper(_httpResponse, responseContent);
                httpRequest.Dispose();
                if (_httpResponse != null)
                {
                    _httpResponse.Dispose();
                }
                throw ex;
            }
            // Create Result
            var _result = new HttpOperationResponse<string>();
            _result.Request = httpRequest;
            _result.Response = _httpResponse;
            // Deserialize Response
            if ((int)statusCode == 200)
            {
                responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    _result.Body = responseContent;
                }
                catch (JsonException ex)
                {
                    httpRequest.Dispose();
                    if (_httpResponse != null)
                    {
                        _httpResponse.Dispose();
                    }
                    throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                }
            }

            return _result;
        }

        /// <summary>
        /// Deletes the service with HTTP messages asynchronous.
        /// this is modification of auto generated to avoid deserialization exception
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="namespaceParameter">The namespace parameter.</param>
        /// <returns></returns>
        /// <exception cref="ValidationException">
        /// name
        /// or
        /// namespaceParameter
        /// </exception>
        /// <exception cref="SerializationException">Unable to deserialize the response.</exception>
        private async Task<HttpOperationResponse<string>> DeleteCoreV1NSServiceWithHttpMessagesAsync(string name, string namespaceParameter)
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
            var url = new System.Uri(new System.Uri(baseUrl + (baseUrl.EndsWith("/") ? "" : "/")), "api/v1/namespaces/{namespace}/services/{name}").ToString();
            url = url.Replace("{name}", System.Uri.EscapeDataString(name));
            url = url.Replace("{namespace}", System.Uri.EscapeDataString(namespaceParameter));
            List<string> queryParameters = new List<string>();
            if (queryParameters.Count > 0)
            {
                url += "?" + string.Join("&", queryParameters);
            }
            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage();
            HttpResponseMessage httpResponse = null;
            httpRequest.Method = new HttpMethod("DELETE");
            httpRequest.RequestUri = new System.Uri(url);

            // Serialize Request
            string requestContent = null;
            // Send Request
            httpResponse = await HttpClient.SendAsync(httpRequest, default(CancellationToken)).ConfigureAwait(false);
            HttpStatusCode statusCode = httpResponse.StatusCode;
            string responseContent = null;
            // Create Result
            var result = new HttpOperationResponse<string>();
            result.Request = httpRequest;
            result.Response = httpResponse;
            // Deserialize Response
            responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
                result.Body = responseContent;
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
            return result;
        }

        /// <summary>
        /// Deletes the replica set with HTTP messages asynchronous.
        /// Not used - when delete deployment it cascade delete it
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="name">The name.</param>
        /// <param name="namespaceParameter">The namespace parameter.</param>
        /// <returns></returns>
        /// <exception cref="ValidationException">
        /// body
        /// or
        /// name
        /// or
        /// namespaceParameter
        /// </exception>
        /// <exception cref="SerializationException">Unable to deserialize the response.</exception>
        private async Task<HttpOperationResponse<Iok8sapimachinerypkgapismetav1Status>> DeleteExtensionsV1beta1NSReplicaSetWithHttpMessagesAsync(Iok8sapimachinerypkgapismetav1DeleteOptions body, string name, string namespaceParameter)
        {
            if (body == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "body");
            }
            if (name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "name");
            }
            if (namespaceParameter == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "namespaceParameter");
            }
            // Construct URL
            var _baseUrl = BaseUri.AbsoluteUri;
            var _url = new System.Uri(new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "apis/extensions/v1beta1/namespaces/{namespace}/replicasets/{name}").ToString();
            _url = _url.Replace("{name}", System.Uri.EscapeDataString(name));
            _url = _url.Replace("{namespace}", System.Uri.EscapeDataString(namespaceParameter));
            // Create HTTP transport objects
            var _httpRequest = new HttpRequestMessage();
            HttpResponseMessage _httpResponse = null;
            _httpRequest.Method = new HttpMethod("DELETE");
            _httpRequest.RequestUri = new System.Uri(_url);

            // Serialize Request
            string _requestContent = null;
            if (body != null)
            {
                _requestContent = SafeJsonConvert.SerializeObject(body, SerializationSettings);
                _httpRequest.Content = new StringContent(_requestContent, System.Text.Encoding.UTF8);
                _httpRequest.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
            }
            _httpResponse = await HttpClient.SendAsync(_httpRequest, default(CancellationToken)).ConfigureAwait(false);
            HttpStatusCode _statusCode = _httpResponse.StatusCode;
            string _responseContent = null;
            if ((int)_statusCode != 200 && (int)_statusCode != 401)
            {
                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'", _statusCode));
                if (_httpResponse.Content != null)
                {
                    _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else
                {
                    _responseContent = string.Empty;
                }
                ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
                ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
                _httpRequest.Dispose();
                if (_httpResponse != null)
                {
                    _httpResponse.Dispose();
                }
                throw ex;
            }
            // Create Result
            var _result = new HttpOperationResponse<Iok8sapimachinerypkgapismetav1Status>();
            _result.Request = _httpRequest;
            _result.Response = _httpResponse;
            // Deserialize Response
            if ((int)_statusCode == 200)
            {
                _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    _result.Body = SafeJsonConvert.DeserializeObject<Iok8sapimachinerypkgapismetav1Status>(_responseContent, DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    _httpRequest.Dispose();
                    if (_httpResponse != null)
                    {
                        _httpResponse.Dispose();
                    }
                    throw new SerializationException("Unable to deserialize the response.", _responseContent, ex);
                }
            }
            return _result;
        }
    }
}
