﻿using Lykke.AlgoStore.Core.Settings.ServiceSettings;
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

        #region Implemented
        //[Test]
        //public void GetBuildTypes_Returns_Success()
        //{
        //    string key = "6edcd51b-09c7-4d9b-b78c-1e08bdd84623";

        //    var client = new TeamCityClient.TeamCityClient(_settings);

        //    //BuildTypeResponse buildTypeResponse = client.GetBuildTypes().Result;
        //    //Assert.IsNotNull(buildTypeResponse);

        //    //BuildType archeTypeBuild = buildTypeResponse.BuildTypes.FirstOrDefault(bt => bt.ProjectId == _settings.ProjectId);
        //    //Assert.IsNotNull(archeTypeBuild);

        //    ParametersResponse parametersResponse = client.GetParameters(_settings.BuildConfigurationId).Result;
        //    Assert.IsNotNull(parametersResponse);

        //    var request = new BuildRequest
        //    {
        //        Personal = true,
        //        BuildType = new BuildTypeBase { Id = _settings.BuildConfigurationId },
        //        Properties = new Properties { Property = new List<PropertyBase>() }
        //    };

        //    var storageConnectionManager = new StorageConnectionManager(SettingsMock.GetSettings());
        //    var headers = storageConnectionManager.GetData(key);

        //    //CODE_Blob_AuthorizationHeader
        //    //    CODE_Blob_DateHeader
        //    //CODE_Blob_Url
        //    //    CODE_Blob_VersionHeader
        //    //CODE_Source_File
        //    foreach (Property responsePropery in parametersResponse.Properies)
        //    {
        //        var propertyBase = new PropertyBase
        //        {
        //            Name = responsePropery.Name
        //        };

        //        switch (responsePropery.Name)
        //        {
        //            case "CODE_Blob_AuthorizationHeader":
        //                propertyBase.Value = headers.AuthorizationHeader;
        //                break;
        //            case "CODE_Blob_DateHeader":
        //                propertyBase.Value = headers.DateHeader;
        //                break;
        //            case "CODE_Blob_Url":
        //                propertyBase.Value = headers.Url;
        //                break;
        //            case "CODE_Blob_VersionHeader":
        //                propertyBase.Value = headers.VersionHeader;
        //                break;
        //        }

        //        if (!string.IsNullOrWhiteSpace(propertyBase.Value))
        //            request.Properties.Property.Add(propertyBase);
        //    }

        //    var buildBase = client.StartBuild(request).Result;
        //    Assert.IsNotNull(buildBase);

        //    var build = client.GetBuildStatus(buildBase.Id).Result;

        //    var responseString = JsonConvert.SerializeObject(build);

        //}
        #endregion

        //[Test]
        //public void RunningABuildWithValidAlgo_Returns_Success()
        //{
        //    string algoKey = "6909ec8c-2491-4782-abcd-628d774a502c";
        //    var storageConnectionManager = new StorageConnectionManager(SettingsMock.GetSettings());
        //    var headers = storageConnectionManager.GetData(algoKey);

        //    var client = new TeamCityClient.TeamCityClient(_settings);
        //    ParametersResponse parametersResponse = client.GetParameters(_settings.BuildConfigurationId).Result;
        //    var request = BuildBuildRequest(parametersResponse, new Dictionary<string, string>
        //    {
        //        { "CODE_ALGO_ID",algoKey },
        //        { "CODE_Blob_AuthorizationHeader",headers.AuthorizationHeader },
        //        { "CODE_Blob_DateHeader",headers.DateHeader },
        //        { "CODE_Blob_Url",headers.Url },
        //        { "CODE_Blob_VersionHeader",headers.VersionHeader },
        //    });

        //    var buildBase = client.StartBuild(request).Result;
        //    Assert.IsNotNull(buildBase);

        //    var build = client.GetBuildStatus(buildBase.Id).Result;
        //    var responseString = JsonConvert.SerializeObject(build);
        //    Assert.IsNotNull(responseString);
        //    Assert.AreEqual("SUCCESS", JObject.Parse(responseString)["Status"].ToString());
        //}

        //private BuildRequest BuildBuildRequest(ParametersResponse parametersResponse, Dictionary<string, string> buildParams)
        //{
        //    var request = new BuildRequest
        //    {
        //        Personal = true,
        //        BuildType = new BuildTypeBase { Id = _settings.BuildConfigurationId },
        //        Properties = new Properties { Property = new List<PropertyBase>() }
        //    };
        //    foreach (Property responsePropery in parametersResponse.Properies)
        //    {
        //        var propertyBase = new PropertyBase
        //        {
        //            Name = responsePropery.Name
        //        };

        //        if (buildParams.ContainsKey(responsePropery.Name))
        //        {
        //            propertyBase.Value = buildParams[responsePropery.Name];
        //        }
        //        else
        //        {
        //            propertyBase.Value = "Auto generated value of: " + propertyBase.Name;
        //        }

        //        request.Properties.Property.Add(propertyBase);
        //    }

        //    return request;
        //}

        //[Test]
        //public void GetBuildStatus_Returns_Success()
        //{
        //    var client = new TeamCityClient.TeamCityClient(_settings);

        //    var build = client.GetBuildStatus(40).Result;
        //    Assert.IsNotNull(build);

        //    if (build.GetBuildStatus() == BuildStatuses.Failure &&
        //        build.ProblemOccurrences.Count > 0)
        //    {
        //        var problems = client.GetBuildProblemDetails(40).Result;
        //        Assert.IsNotNull(problems);

        //        var responseString = JsonConvert.SerializeObject(problems);
        //    }
        //}

        //[Test]
        //public void TeamCityParametersTest()
        //{
        //    var storageConnectionManager = new StorageConnectionManager(SettingsMock.GetSettings());
        //    Assert.IsNotNull(storageConnectionManager);

        //    var headers = storageConnectionManager.GetData(Guid.NewGuid().ToString());
        //    Assert.IsNotNull(headers);
        //}
    }
}
