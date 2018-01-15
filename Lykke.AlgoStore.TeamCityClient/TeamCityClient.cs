using System;
using System.Collections.Generic;
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
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()

        };

        private readonly TeamCitySettings _settings;

        public TeamCityClient(TeamCitySettings settings)
        {
            _settings = settings;
        }

        public async Task<BuildTypeResponse> GetBuildTypes()
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

        public async Task<ParametersResponse> GetParameters(string buildTypeId)
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

        public async Task<BuildBase> StartBuild(BuildRequest request)
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

        public async Task<List<ProblemInfo>> GetBuildProblemDetails(int buildId)
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
