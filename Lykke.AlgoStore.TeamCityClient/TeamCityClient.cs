using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Settings.ServiceSettings;
using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.TeamCityClient.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lykke.AlgoStore.TeamCityClient
{
    public class TeamCityClient : ITeamCityClient
    {
        private const string BlobAuthorizationHeader = "CODE_Blob_AuthorizationHeader";
        private const string BlobDateHeader = "CODE_Blob_DateHeader";
        private const string BlobUrl = "CODE_Blob_Url";
        private const string BlobVersionHeader = "CODE_Blob_VersionHeader";
        private const string AlgoId = "CODE_ALGO_ID";
        private const string TradedAsset = "CODE_ASSET";
        private const string AssetPair = "CODE_ASSET_PAIR";
        private const string HftKey = "CODE_HFT_KEY";
        private const string Margin = "CODE_MARGIN";
        private const string Volume = "CODE_VOLUME";
        private const string HftApiUrl = "CODE_HFT_API_BASE_PATH";
        private const string WalletKey = "CODE_WALLET_KEY";
        private const string ServiceName = "service.name";

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()

        };

        private readonly TeamCitySettings _settings;

        public TeamCityClient(TeamCitySettings settings)
        {
            _settings = settings;
        }


        public async Task<BuildBase> StartBuild(TeamCityClientBuildData buildData)
        {
            ParametersResponse parametersResponse = await GetParameters(_settings.BuildConfigurationId);

            var request = new BuildRequest
            {
                Personal = true,
                BuildType = new BuildTypeBase { Id = _settings.BuildConfigurationId },
                Properties = new Properties { Property = new List<PropertyBase>() }
            };

            SetRequestParameter(request, parametersResponse, buildData);

            return await StartBuild(request);
        }

        public async Task<Build> GetBuildStatus(int buildId)
        {
            const string url = "/app/rest/buildQueue/id:{0} ";

            using (HttpClient client = CreateClient(_settings))
            using (HttpResponseMessage responseMessage = await client.GetAsync(string.Format(url, buildId)))
            {
                if (responseMessage.StatusCode != HttpStatusCode.OK)
                    return null;

                var content = await responseMessage.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<Build>(content, JsonSerializerSettings);
            }
        }



        private async Task<BuildTypeResponse> GetBuildTypes()
        {
            const string url = "/app/rest/buildTypes";

            using (HttpClient client = CreateClient(_settings))
            using (HttpResponseMessage responseMessage = await client.GetAsync(url, CancellationToken.None))
            {
                if (responseMessage.StatusCode != HttpStatusCode.OK)
                    return null;

                var content = await responseMessage.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<BuildTypeResponse>(content, JsonSerializerSettings);
            }
        }



        private async Task<List<ProblemInfo>> GetBuildProblemDetails(int buildId)
        {
            const string urlProblems = "/app/rest/problemOccurrences?locator=build:(id:{0})";

            var result = new List<ProblemInfo>();

            using (HttpClient client = CreateClient(_settings))
            using (HttpResponseMessage responseMessage = await client.GetAsync(string.Format(urlProblems, buildId)))
            {
                if (responseMessage.StatusCode != HttpStatusCode.OK)
                    return null;

                var content = await responseMessage.Content.ReadAsStringAsync();

                var problems = JsonConvert.DeserializeObject<ProblemResponse>(content, JsonSerializerSettings);
                if ((responseMessage.StatusCode != HttpStatusCode.OK) ||
                    (problems == null) || (problems.ProblemOccurrence.IsNullOrEmptyCollection()))
                    return null;

                foreach (var problem in problems.ProblemOccurrence)
                {
                    using (HttpResponseMessage problemMessage = await client.GetAsync(problem.Href))
                    {
                        if (responseMessage.StatusCode != HttpStatusCode.OK)
                            continue;

                        content = await problemMessage.Content.ReadAsStringAsync();
                        var problemInfo = JsonConvert.DeserializeObject<ProblemInfo>(content, JsonSerializerSettings);
                        if (problemInfo != null)
                            result.Add(problemInfo);
                    }
                    //problem.Href
                }
            }

            return result;
        }

        #region Private Methods

        private async Task<BuildBase> StartBuild(BuildRequest request)
        {
            const string url = "/app/rest/buildQueue ";

            using (HttpClient client = CreateClient(_settings))
            using (HttpResponseMessage responseMessage = await client.PostAsync(url, CreateJsonContent(request)))
            {
                if (responseMessage.StatusCode != HttpStatusCode.OK)
                    return null;

                var content = await responseMessage.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<BuildBase>(content, JsonSerializerSettings);
            }
        }

        private void SetRequestParameter(BuildRequest request, ParametersResponse parametersResponse, TeamCityClientBuildData buildData)
        {
            foreach (Property responsePropery in parametersResponse.Properies)
            {
                var propertyBase = new PropertyBase
                {
                    Name = responsePropery.Name
                };

                switch (responsePropery.Name)
                {
                    case BlobAuthorizationHeader:
                        propertyBase.Value = buildData.BlobAuthorizationHeader;
                        break;
                    case BlobDateHeader:
                        propertyBase.Value = buildData.BlobDateHeader;
                        break;
                    case BlobUrl:
                        propertyBase.Value = buildData.BlobUrl;
                        break;
                    case BlobVersionHeader:
                        propertyBase.Value = buildData.BlobVersionHeader;
                        break;
                    case AlgoId:
                        propertyBase.Value = buildData.AlgoId;
                        break;
                    case TradedAsset:
                        propertyBase.Value = buildData.TradedAsset;
                        break;
                    case AssetPair:
                        propertyBase.Value = buildData.AssetPair;
                        break;
                    case HftKey:
                        propertyBase.Value = buildData.HftApiKey;
                        break;
                    case Margin:
                        propertyBase.Value = buildData.Margin.ToString(CultureInfo.InvariantCulture);
                        break;
                    case Volume:
                        propertyBase.Value = buildData.Volume.ToString(CultureInfo.InvariantCulture);
                        break;
                    case HftApiUrl:
                        propertyBase.Value = buildData.HftApiUrl;
                        break;
                    case WalletKey:
                        propertyBase.Value = buildData.WalletApiKey;
                        break;
                    case ServiceName:
                        propertyBase.Value = buildData.AlgoId;
                        break;
                }

                if (!string.IsNullOrWhiteSpace(propertyBase.Value))
                    request.Properties.Property.Add(propertyBase);
            }
        }

        private async Task<ParametersResponse> GetParameters(string buildTypeId)
        {
            const string url = "/app/rest/buildTypes/id:{0}/parameters";

            using (HttpClient client = CreateClient(_settings))
            using (HttpResponseMessage responseMessage = await client.GetAsync(string.Format(url, buildTypeId), CancellationToken.None))
            {
                if (responseMessage.StatusCode != HttpStatusCode.OK)
                    return null;

                var content = await responseMessage.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<ParametersResponse>(content, JsonSerializerSettings);
            }
        }
        #endregion

        #region Infrastructure Methods
        private static HttpClient CreateClient(TeamCitySettings settings)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(settings.Url);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", settings.BasicAuthenticationValue);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        private static HttpContent CreateJsonContent(object value)
        {
            var payload = JsonConvert.SerializeObject(value, JsonSerializerSettings);
            var httpContent = new StringContent(payload, Encoding.UTF8);
            httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            return httpContent;
        }

        #endregion
    }
}
