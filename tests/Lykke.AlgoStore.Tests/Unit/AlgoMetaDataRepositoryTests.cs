using System;
using System.Collections.Generic;
using AutoFixture;
using AzureStorage.Tables;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Repositories;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Tests.Infrastructure;
using Xunit;

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


        [RunnableInDebugOnly("Should run manually only. Manipulate data in Table Storage")]
        public void AlgoMetaData_Save_Test()
        {
            var repo = Given_AlgoMetaData_Repository();
            When_Invoke_Save(repo, _entity);
            Then_Data_ShouldBe_Saved(repo, _entity);
        }
        [RunnableInDebugOnly("Should run manually only. Manipulate data in Table Storage")]
        public void AlgoMetaData_GetAll_Test()
        {
            var repo = Given_AlgoMetaData_Repository();
            When_Invoke_Save(repo, _entity);
            var all = When_Ivoke_GetAll(repo, ClientId);
            Then_Result_ShouldNotBe_Null(all);
        }

        private static AlgoMetaDataRepository Given_AlgoMetaData_Repository()
        {
            return new AlgoMetaDataRepository(AzureTableStorage<AlgoMetaDataEntity>.Create(SettingsMock.GetSettings(), AlgoMetaDataRepository.TableName, new LogMock()));
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
