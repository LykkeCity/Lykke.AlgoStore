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
        //ListCoreV1NodeAsync

        /// <summary>
        /// This method is similar as <see cref="KubernetesExtensions.CreateAppsV1beta1NamespacedDeploymentAsync"/>
        /// parameters are same, but code is changed cause API return 201 and in swagger and docs it is stated that API will return 200.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="namespaceParameter"></param>
        /// <param name="pretty"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Iok8skubernetespkgapisappsv1beta1Deployment> CreateDeploymentAsync(
            Iok8skubernetespkgapisappsv1beta1Deployment body, string namespaceParameter,
            string pretty = default(string), CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _result =
                await CreateDeploymentWithHttpMessagesAsync(body, namespaceParameter, pretty, null, cancellationToken)
                    .ConfigureAwait(false))
            {
                return _result.Body;
            }
        }

        /// <summary>
        /// This method is similar as <see cref="KubernetesExtensions.CreateCoreV1NamespaceAsync"/>
        /// parameters are same, but code is changed cause API return 201 and in swagger and docs it is stated that API will return 200.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="pretty"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Iok8skubernetespkgapiv1Namespace> CreateNamespaceAsync(Iok8skubernetespkgapiv1Namespace body,
            string pretty = default(string),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _result = await CreateNamespaceWithHttpMessagesAsync(body, pretty, null, cancellationToken)
                .ConfigureAwait(false))
            {
                return _result.Body;
            }
        }

        /// <summary>
        /// This method is similar as <see cref="KubernetesExtensions.DeleteCoreV1NamespaceAsync"/>
        /// parameters are same, but code is changed cause API return 201 and in swagger and docs it is stated that API will return 200.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="name"></param>
        /// <param name="gracePeriodSeconds"></param>
        /// <param name="orphanDependents"></param>
        /// <param name="propagationPolicy"></param>
        /// <param name="pretty"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<DeleteNamespaceResponse> DeleteNamespaceAsync(
            Iok8sapimachinerypkgapismetav1DeleteOptions body, string name,
            int? gracePeriodSeconds = default(int?), bool? orphanDependents = default(bool?),
            string propagationPolicy = default(string), string pretty = default(string),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _result = await DeleteNamespaceWithHttpMessagesAsync(body, name,
                    gracePeriodSeconds, orphanDependents, propagationPolicy, pretty, null, cancellationToken)
                .ConfigureAwait(false))
            {
                return _result.Body;
            }
        }

        public async Task<string> ReadPodLogAsync(string name, string namespaceParameter,
            string container = default(string),
            bool? follow = default(bool?), int? limitBytes = default(int?), string pretty = default(string),
            bool? previous = default(bool?), int? sinceSeconds = default(int?), int? tailLines = default(int?),
            bool? timestamps = default(bool?), CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _result = await ReadPodLogWithHttpMessagesAsync(name, namespaceParameter, container, follow,
                    limitBytes, pretty, previous, sinceSeconds, tailLines, timestamps, null, cancellationToken)
                .ConfigureAwait(false))
            {
                return _result.Body;
            }
        }

        /// <summary>
        /// This method is similar as <see cref="Kubernetes.DeleteCoreV1NamespaceWithHttpMessagesAsync"/>
        /// parameters are same, but code is changed cause API return 201 and in swagger and docs it is stated that API will return 200.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="name"></param>
        /// <param name="gracePeriodSeconds"></param>
        /// <param name="orphanDependents"></param>
        /// <param name="propagationPolicy"></param>
        /// <param name="pretty"></param>
        /// <param name="customHeaders"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<HttpOperationResponse<DeleteNamespaceResponse>>
            DeleteNamespaceWithHttpMessagesAsync(Iok8sapimachinerypkgapismetav1DeleteOptions body, string name,
                int? gracePeriodSeconds = default(int?), bool? orphanDependents = default(bool?),
                string propagationPolicy = default(string), string pretty = default(string),
                Dictionary<string, List<string>> customHeaders = null,
                CancellationToken cancellationToken = default(CancellationToken))
        {
            if (body == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "body");
            }
            if (name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "name");
            }
            // Tracing
            bool _shouldTrace = ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("body", body);
                tracingParameters.Add("gracePeriodSeconds", gracePeriodSeconds);
                tracingParameters.Add("orphanDependents", orphanDependents);
                tracingParameters.Add("propagationPolicy", propagationPolicy);
                tracingParameters.Add("name", name);
                tracingParameters.Add("pretty", pretty);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(_invocationId, this, "DeleteCoreV1Namespace", tracingParameters);
            }
            // Construct URL
            var _baseUrl = BaseUri.AbsoluteUri;
            var _url = new System.Uri(new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")),
                "api/v1/namespaces/{name}").ToString();
            _url = _url.Replace("{name}", System.Uri.EscapeDataString(name));
            List<string> _queryParameters = new List<string>();
            if (gracePeriodSeconds != null)
            {
                _queryParameters.Add(string.Format("gracePeriodSeconds={0}",
                    System.Uri.EscapeDataString(SafeJsonConvert
                        .SerializeObject(gracePeriodSeconds, SerializationSettings).Trim('"'))));
            }
            if (orphanDependents != null)
            {
                _queryParameters.Add(string.Format("orphanDependents={0}",
                    System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(orphanDependents, SerializationSettings)
                        .Trim('"'))));
            }
            if (propagationPolicy != null)
            {
                _queryParameters.Add(string.Format("propagationPolicy={0}",
                    System.Uri.EscapeDataString(propagationPolicy)));
            }
            if (pretty != null)
            {
                _queryParameters.Add(string.Format("pretty={0}", System.Uri.EscapeDataString(pretty)));
            }
            if (_queryParameters.Count > 0)
            {
                _url += "?" + string.Join("&", _queryParameters);
            }
            // Create HTTP transport objects
            var _httpRequest = new HttpRequestMessage();
            HttpResponseMessage _httpResponse = null;
            _httpRequest.Method = new HttpMethod("DELETE");
            _httpRequest.RequestUri = new System.Uri(_url);
            // Set Headers


            if (customHeaders != null)
            {
                foreach (var _header in customHeaders)
                {
                    if (_httpRequest.Headers.Contains(_header.Key))
                    {
                        _httpRequest.Headers.Remove(_header.Key);
                    }
                    _httpRequest.Headers.TryAddWithoutValidation(_header.Key, _header.Value);
                }
            }

            // Serialize Request
            string _requestContent = null;
            if (body != null)
            {
                _requestContent = SafeJsonConvert.SerializeObject(body, SerializationSettings);
                _httpRequest.Content = new StringContent(_requestContent, System.Text.Encoding.UTF8);
                _httpRequest.Content.Headers.ContentType =
                    System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
            }
            // Send Request
            if (_shouldTrace)
            {
                ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
            }
            cancellationToken.ThrowIfCancellationRequested();
            _httpResponse = await HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
            if (_shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
            }
            HttpStatusCode _statusCode = _httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string _responseContent = null;
            if ((int)_statusCode != 200 && (int)_statusCode != 401 && (int)_statusCode != 201 &&
                (int)_statusCode != 202)
            {
                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'",
                    _statusCode));
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
                if (_shouldTrace)
                {
                    ServiceClientTracing.Error(_invocationId, ex);
                }
                _httpRequest.Dispose();
                if (_httpResponse != null)
                {
                    _httpResponse.Dispose();
                }
                throw ex;
            }
            // Create Result
            var _result = new HttpOperationResponse<DeleteNamespaceResponse>();
            _result.Request = _httpRequest;
            _result.Response = _httpResponse;
            // Deserialize Response
            if ((int)_statusCode == 200 || (int)_statusCode != 201 || (int)_statusCode != 202)
            {
                _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    _result.Body =
                        SafeJsonConvert.DeserializeObject<DeleteNamespaceResponse>(_responseContent,
                            DeserializationSettings);
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
            if (_shouldTrace)
            {
                ServiceClientTracing.Exit(_invocationId, _result);
            }
            return _result;
        }

        /// <summary>
        /// This method is similar as <see cref="Kubernetes.CreateAppsV1beta1NamespacedDeploymentWithHttpMessagesAsync"/>
        /// parameters are same, but code is changed cause API return 201 and in swagger and docs it is stated that API will return 200.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="namespaceParameter"></param>
        /// <param name="pretty"></param>
        /// <param name="customHeaders"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<HttpOperationResponse<Iok8skubernetespkgapisappsv1beta1Deployment>>
            CreateDeploymentWithHttpMessagesAsync(Iok8skubernetespkgapisappsv1beta1Deployment body,
                string namespaceParameter, string pretty = default(string),
                Dictionary<string, List<string>> customHeaders = null,
                CancellationToken cancellationToken = default(CancellationToken))
        {
            if (body == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "body");
            }
            if (body != null)
            {
                body.Validate();
            }
            if (namespaceParameter == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "namespaceParameter");
            }
            // Tracing
            bool _shouldTrace = ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("body", body);
                tracingParameters.Add("namespaceParameter", namespaceParameter);
                tracingParameters.Add("pretty", pretty);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(_invocationId, this, "CreateAppsV1beta1NamespacedDeployment",
                    tracingParameters);
            }
            // Construct URL
            var _baseUrl = BaseUri.AbsoluteUri;
            var _url = new System.Uri(new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")),
                "apis/apps/v1beta1/namespaces/{namespace}/deployments").ToString();
            _url = _url.Replace("{namespace}", System.Uri.EscapeDataString(namespaceParameter));
            List<string> _queryParameters = new List<string>();
            if (pretty != null)
            {
                _queryParameters.Add(string.Format("pretty={0}", System.Uri.EscapeDataString(pretty)));
            }
            if (_queryParameters.Count > 0)
            {
                _url += "?" + string.Join("&", _queryParameters);
            }
            // Create HTTP transport objects
            var _httpRequest = new HttpRequestMessage();
            HttpResponseMessage _httpResponse = null;
            _httpRequest.Method = new HttpMethod("POST");
            _httpRequest.RequestUri = new System.Uri(_url);
            // Set Headers


            if (customHeaders != null)
            {
                foreach (var _header in customHeaders)
                {
                    if (_httpRequest.Headers.Contains(_header.Key))
                    {
                        _httpRequest.Headers.Remove(_header.Key);
                    }
                    _httpRequest.Headers.TryAddWithoutValidation(_header.Key, _header.Value);
                }
            }

            // Serialize Request
            string _requestContent = null;
            if (body != null)
            {
                _requestContent = SafeJsonConvert.SerializeObject(body, SerializationSettings);
                _httpRequest.Content = new StringContent(_requestContent, System.Text.Encoding.UTF8);
                _httpRequest.Content.Headers.ContentType =
                    System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
            }
            // Send Request
            if (_shouldTrace)
            {
                ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
            }
            cancellationToken.ThrowIfCancellationRequested();
            _httpResponse = await HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
            if (_shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
            }
            HttpStatusCode _statusCode = _httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string _responseContent = null;
            if ((int)_statusCode != 200 && (int)_statusCode != 401 && (int)_statusCode != 201)
            {
                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'",
                    _statusCode));
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
                if (_shouldTrace)
                {
                    ServiceClientTracing.Error(_invocationId, ex);
                }
                _httpRequest.Dispose();
                if (_httpResponse != null)
                {
                    _httpResponse.Dispose();
                }
                throw ex;
            }
            // Create Result
            var _result = new HttpOperationResponse<Iok8skubernetespkgapisappsv1beta1Deployment>();
            _result.Request = _httpRequest;
            _result.Response = _httpResponse;
            // Deserialize Response
            if ((int)_statusCode == 200 || (int)_statusCode == 201)
            {
                _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    _result.Body =
                        SafeJsonConvert.DeserializeObject<Iok8skubernetespkgapisappsv1beta1Deployment>(_responseContent,
                            DeserializationSettings);
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
            if (_shouldTrace)
            {
                ServiceClientTracing.Exit(_invocationId, _result);
            }
            return _result;
        }

        /// <summary>
        /// This method is similar as <see cref="Kubernetes.CreateCoreV1NamespaceWithHttpMessagesAsync"/>
        /// parameters are same, but code is changed cause API return 201 and in swagger and docs it is stated that API will return 200.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="pretty"></param>
        /// <param name="customHeaders"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<HttpOperationResponse<Iok8skubernetespkgapiv1Namespace>>
            CreateNamespaceWithHttpMessagesAsync(
                Iok8skubernetespkgapiv1Namespace body, string pretty = default(string),
                Dictionary<string, List<string>> customHeaders = null,
                CancellationToken cancellationToken = default(CancellationToken))
        {
            if (body == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "body");
            }
            if (body != null)
            {
                body.Validate();
            }
            // Tracing
            bool _shouldTrace = ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("body", body);
                tracingParameters.Add("pretty", pretty);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(_invocationId, this, "CreateCoreV1Namespace", tracingParameters);
            }
            // Construct URL
            var _baseUrl = BaseUri.AbsoluteUri;
            var _url = new System.Uri(new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")),
                "api/v1/namespaces").ToString();
            List<string> _queryParameters = new List<string>();
            if (pretty != null)
            {
                _queryParameters.Add(string.Format("pretty={0}", System.Uri.EscapeDataString(pretty)));
            }
            if (_queryParameters.Count > 0)
            {
                _url += "?" + string.Join("&", _queryParameters);
            }
            // Create HTTP transport objects
            var _httpRequest = new HttpRequestMessage();
            HttpResponseMessage _httpResponse = null;
            _httpRequest.Method = new HttpMethod("POST");
            _httpRequest.RequestUri = new System.Uri(_url);
            // Set Headers


            if (customHeaders != null)
            {
                foreach (var _header in customHeaders)
                {
                    if (_httpRequest.Headers.Contains(_header.Key))
                    {
                        _httpRequest.Headers.Remove(_header.Key);
                    }
                    _httpRequest.Headers.TryAddWithoutValidation(_header.Key, _header.Value);
                }
            }

            // Serialize Request
            string _requestContent = null;
            if (body != null)
            {
                _requestContent = SafeJsonConvert.SerializeObject(body, SerializationSettings);
                _httpRequest.Content = new StringContent(_requestContent, System.Text.Encoding.UTF8);
                _httpRequest.Content.Headers.ContentType =
                    System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
            }
            // Send Request
            if (_shouldTrace)
            {
                ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
            }
            cancellationToken.ThrowIfCancellationRequested();
            _httpResponse = await HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
            if (_shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
            }
            HttpStatusCode _statusCode = _httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string _responseContent = null;
            if ((int)_statusCode != 200 && (int)_statusCode != 401 && (int)_statusCode != 201 &&
                (int)_statusCode != 202)
            {
                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'",
                    _statusCode));
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
                if (_shouldTrace)
                {
                    ServiceClientTracing.Error(_invocationId, ex);
                }
                _httpRequest.Dispose();
                if (_httpResponse != null)
                {
                    _httpResponse.Dispose();
                }
                throw ex;
            }
            // Create Result
            var _result = new HttpOperationResponse<Iok8skubernetespkgapiv1Namespace>();
            _result.Request = _httpRequest;
            _result.Response = _httpResponse;
            // Deserialize Response
            if ((int)_statusCode == 200 || (int)_statusCode != 201 || (int)_statusCode != 202)
            {
                _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    _result.Body =
                        SafeJsonConvert.DeserializeObject<Iok8skubernetespkgapiv1Namespace>(_responseContent,
                            DeserializationSettings);
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
            if (_shouldTrace)
            {
                ServiceClientTracing.Exit(_invocationId, _result);
            }
            return _result;
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
        /// <param name="customHeaders"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<HttpOperationResponse<string>> ReadPodLogWithHttpMessagesAsync(string name,
            string namespaceParameter, string container = default(string), bool? follow = default(bool?),
            int? limitBytes = default(int?), string pretty = default(string), bool? previous = default(bool?),
            int? sinceSeconds = default(int?), int? tailLines = default(int?), bool? timestamps = default(bool?),
            Dictionary<string, List<string>> customHeaders = null,
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
            // Tracing
            bool _shouldTrace = ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("container", container);
                tracingParameters.Add("follow", follow);
                tracingParameters.Add("limitBytes", limitBytes);
                tracingParameters.Add("name", name);
                tracingParameters.Add("namespaceParameter", namespaceParameter);
                tracingParameters.Add("pretty", pretty);
                tracingParameters.Add("previous", previous);
                tracingParameters.Add("sinceSeconds", sinceSeconds);
                tracingParameters.Add("tailLines", tailLines);
                tracingParameters.Add("timestamps", timestamps);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(_invocationId, this, "ReadCoreV1NamespacedPodLog", tracingParameters);
            }
            // Construct URL
            var _baseUrl = BaseUri.AbsoluteUri;
            var _url = new System.Uri(new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")),
                "api/v1/namespaces/{namespace}/pods/{name}/log").ToString();
            _url = _url.Replace("{name}", System.Uri.EscapeDataString(name));
            _url = _url.Replace("{namespace}", System.Uri.EscapeDataString(namespaceParameter));
            List<string> _queryParameters = new List<string>();
            if (container != null)
            {
                _queryParameters.Add(string.Format("container={0}", System.Uri.EscapeDataString(container)));
            }
            if (follow != null)
            {
                _queryParameters.Add(string.Format("follow={0}",
                    System.Uri.EscapeDataString(
                        SafeJsonConvert.SerializeObject(follow, SerializationSettings).Trim('"'))));
            }
            if (limitBytes != null)
            {
                _queryParameters.Add(string.Format("limitBytes={0}",
                    System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(limitBytes, SerializationSettings)
                        .Trim('"'))));
            }
            if (pretty != null)
            {
                _queryParameters.Add(string.Format("pretty={0}", System.Uri.EscapeDataString(pretty)));
            }
            if (previous != null)
            {
                _queryParameters.Add(string.Format("previous={0}",
                    System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(previous, SerializationSettings)
                        .Trim('"'))));
            }
            if (sinceSeconds != null)
            {
                _queryParameters.Add(string.Format("sinceSeconds={0}",
                    System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(sinceSeconds, SerializationSettings)
                        .Trim('"'))));
            }
            if (tailLines != null)
            {
                _queryParameters.Add(string.Format("tailLines={0}",
                    System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(tailLines, SerializationSettings)
                        .Trim('"'))));
            }
            if (timestamps != null)
            {
                _queryParameters.Add(string.Format("timestamps={0}",
                    System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(timestamps, SerializationSettings)
                        .Trim('"'))));
            }
            if (_queryParameters.Count > 0)
            {
                _url += "?" + string.Join("&", _queryParameters);
            }
            // Create HTTP transport objects
            var _httpRequest = new HttpRequestMessage();
            HttpResponseMessage _httpResponse = null;
            _httpRequest.Method = new HttpMethod("GET");
            _httpRequest.RequestUri = new System.Uri(_url);
            // Set Headers


            if (customHeaders != null)
            {
                foreach (var _header in customHeaders)
                {
                    if (_httpRequest.Headers.Contains(_header.Key))
                    {
                        _httpRequest.Headers.Remove(_header.Key);
                    }
                    _httpRequest.Headers.TryAddWithoutValidation(_header.Key, _header.Value);
                }
            }

            // Serialize Request
            string _requestContent = null;
            // Send Request
            if (_shouldTrace)
            {
                ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
            }
            cancellationToken.ThrowIfCancellationRequested();
            _httpResponse = await HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
            if (_shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
            }
            HttpStatusCode _statusCode = _httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string _responseContent = null;
            if ((int)_statusCode != 200 && (int)_statusCode != 401)
            {
                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'",
                    _statusCode));
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
                if (_shouldTrace)
                {
                    ServiceClientTracing.Error(_invocationId, ex);
                }
                _httpRequest.Dispose();
                if (_httpResponse != null)
                {
                    _httpResponse.Dispose();
                }
                throw ex;
            }
            // Create Result
            var _result = new HttpOperationResponse<string>();
            _result.Request = _httpRequest;
            _result.Response = _httpResponse;
            // Deserialize Response
            if ((int)_statusCode == 200)
            {
                _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    _result.Body = _responseContent;// SafeJsonConvert.DeserializeObject<string>(_responseContent, DeserializationSettings);
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
            if (_shouldTrace)
            {
                ServiceClientTracing.Exit(_invocationId, _result);
            }
            return _result;
        }
    }
}
