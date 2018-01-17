using System;
using System.Collections.Generic;
using Lykke.AlgoStore.AzureRepositories.Utils;
using Lykke.AlgoStore.Core.Settings.ServiceSettings;
using Lykke.AlgoStore.TeamCityClient.Models;
using Lykke.AlgoStore.Tests.Infrastructure;
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
            string key = "6edcd51b-09c7-4d9b-b78c-1e08bdd84623";

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

            var storageConnectionManager = new StorageConnectionManager(SettingsMock.GetSettings());
            var headers = storageConnectionManager.GetData(key);

            //CODE_Blob_AuthorizationHeader
            //    CODE_Blob_DateHeader
            //CODE_Blob_Url
            //    CODE_Blob_VersionHeader
            //CODE_Source_File
            foreach (Property responsePropery in parametersResponse.Properies)
            {
                var propertyBase = new PropertyBase
                {
                    Name = responsePropery.Name
                };

                switch (responsePropery.Name)
                {
                    case "CODE_Blob_AuthorizationHeader":
                        propertyBase.Value = headers.AuthorizationHeader;
                        break;
                    case "CODE_Blob_DateHeader":
                        propertyBase.Value = headers.DateHeader;
                        break;
                    case "CODE_Blob_Url":
                        propertyBase.Value = headers.Url;
                        break;
                    case "CODE_Blob_VersionHeader":
                        propertyBase.Value = headers.VersionHeader;
                        break;
                }

                if (!string.IsNullOrWhiteSpace(propertyBase.Value))
                    request.Properties.Property.Add(propertyBase);
            }

            var buildBase = client.StartBuild(request).Result;
            Assert.IsNotNull(buildBase);

            var build = client.GetBuildStatus(buildBase.Id).Result;

            var responseString = JsonConvert.SerializeObject(build);

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

        [Test]
        public void TeamCityParametersTest()
        {
            var storageConnectionManager = new StorageConnectionManager(SettingsMock.GetSettings());
            Assert.IsNotNull(storageConnectionManager);

            var headers = storageConnectionManager.GetData(Guid.NewGuid().ToString());
            Assert.IsNotNull(headers);
        }
    }
}
