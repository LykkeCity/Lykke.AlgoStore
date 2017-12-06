using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoFixture;
using Lykke.AlgoStore.AzureRepositories.Repositories;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Tests.Infrastructure;
using Lykke.SettingsReader;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    public class AlgoMetaDataRepositoryTests : IDisposable
    {
        private const string ClientId = "{066ABDEF-F1CB-4B24-8EE6-6ACAF1FD623D}";

        private Fixture _fixture = new Fixture();
        private AlgoClientMetaData _entity = null;

        public AlgoMetaDataRepositoryTests()
        {
            SetUp();
        }
        public void Dispose()
        {
            CleanUp();
        }


        //[RunnableInDebugOnly("Should run manually only. Manipulate data in Table Storage")]
        [Conditional("DEBUG")]
        public void AlgoMetaData_Save_Test()
        {
            var repo = Given_AlgoMetaData_Repository();
            When_Invoke_Save(repo, _entity);
            Then_Data_ShouldBe_Saved(repo, _entity);
        }
        //[RunnableInDebugOnly("Should run manually only. Manipulate data in Table Storage")]
        [Conditional("DEBUG")]
        public void AlgoMetaData_GetAll_Test()
        {
            var repo = Given_AlgoMetaData_Repository();
            When_Invoke_Save(repo, _entity);
            var all = When_Ivoke_GetAll(repo, ClientId);
            Then_Result_ShouldNotBe_Null(all);
        }

        private static AlgoMetaDataRepository Given_AlgoMetaData_Repository()
        {
            return new AlgoMetaDataRepository(GetSettings(), new LogMock());
        }
        private static void When_Invoke_Save(AlgoMetaDataRepository repository, AlgoClientMetaData data)
        {
            repository.SaveAlgoMetaData(data).Wait();
        }
        private static void Then_Data_ShouldBe_Saved(AlgoMetaDataRepository repository, AlgoClientMetaData data)
        {
            var saved = repository.GetAlgoMetaData(data.AlgoMetaData[0].ClientAlgoId).Result;
            Assert.NotNull(saved);
        }
        private static AlgoClientMetaData When_Ivoke_GetAll(AlgoMetaDataRepository repository, string clientId)
        {
            return repository.GetAllClientAlgoMetaData(clientId).Result;
        }
        private static void Then_Result_ShouldNotBe_Null(AlgoClientMetaData data)
        {
            Assert.NotNull(data);
        }

        private static IReloadingManager<string> GetSettings()
        {
            var reloadingMock = new Mock<IReloadingManager<string>>();
            reloadingMock
                .Setup(x => x.Reload())
                .Returns(() => Task.FromResult("DefaultEndpointsProtocol=https;AccountName=algostoredev;AccountKey=d2VaBHrf8h8t622KvLeTPGwRP4Dxz9DTPeBT9H3zmjcQprQ1e+Z6Sx9RDqJc+zKwlSO900fzYF2Dg6oUBVDe1w=="));
            return reloadingMock.Object;
        }

        private void SetUp()
        {
            _entity = new AlgoClientMetaData();
            _entity.ClientId = ClientId;
            _entity.AlgoMetaData = new List<AlgoMetaData>();

            _entity.AlgoMetaData.Add(_fixture.Build<AlgoMetaData>().Create());
        }
        private void CleanUp()
        {
            var repo = Given_AlgoMetaData_Repository();
            repo.DeleteAlgoMetaData(_entity).Wait();
            _entity = null;
        }


    }
}
