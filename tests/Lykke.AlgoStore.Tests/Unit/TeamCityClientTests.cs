using System.Collections.Generic;
using System.Linq;
using Lykke.AlgoStore.Core.Settings.ServiceSettings;
using Lykke.AlgoStore.TeamCityClient.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class TeamCityClientTests
    {
        private readonly TeamCitySettings _settings = new TeamCitySettings
        {
            Url = "http://tc-algo.westeurope.cloudapp.azure.com",
            BasicAuthenticationValue = "YXBpdXNlcjpQQHNzdzByZA==",
            BuildConfigurationId = "AlgoStoreArhetype_ArcheTypeBuild"
        };

        [Test]
        public void GetBuildTypes_Returns_Success()
        {
            var client = new TeamCityClient.TeamCityClient(_settings);

            //BuildTypeResponse buildTypeResponse = client.GetBuildTypes().Result;
            //Assert.IsNotNull(buildTypeResponse);

            //BuildType archeTypeBuild = buildTypeResponse.BuildTypes.FirstOrDefault(bt => bt.ProjectId == _settings.ProjectId);
            //Assert.IsNotNull(archeTypeBuild);

            ParametersResponse parametersResponse = client.GetParameters(_settings.BuildConfigurationId).Result;
            Assert.IsNotNull(parametersResponse);

            var request = new BuildRequest
            {
                Personal = true,
                BuildType = new BuildTypeBase { Id = _settings.BuildConfigurationId },
                Properties = new Properties { Property = new List<PropertyBase>() }
            };

            int i = 0;
            //for (int i = 0; i < 5; i++)
            //{
            foreach (Property responsePropery in parametersResponse.Properies)
            {
                var propertyBase = new PropertyBase
                {
                    Name = responsePropery.Name,
                    Value = responsePropery.Name + i
                };
                request.Properties.Property.Add(propertyBase);
            }

            var buildBase = client.StartBuild(request).Result;
            Assert.IsNotNull(buildBase);

            var build = client.GetBuildStatus(buildBase.Id).Result;

            var responseString = JsonConvert.SerializeObject(build);
            //}

            //var responseString = JsonConvert.SerializeObject(response);            
        }

        [Test]
        public void GetBuildStatus_Returns_Success()
        {
            var client = new TeamCityClient.TeamCityClient(_settings);

            var build = client.GetBuildStatus(40).Result;
            Assert.IsNotNull(build);

            if (build.GetBuildStatus() == BuildStatuses.Failure &&
                build.ProblemOccurrences.Count > 0)
            {
                var problems = client.GetBuildProblemDetails(40).Result;
                Assert.IsNotNull(problems);

                var responseString = JsonConvert.SerializeObject(problems);
            }
        }
    }
}
