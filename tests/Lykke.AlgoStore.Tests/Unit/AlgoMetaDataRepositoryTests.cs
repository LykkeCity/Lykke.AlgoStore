using System.Collections.Generic;
using AutoFixture;
using AzureStorage.Tables;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Repositories;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Tests.Infrastructure;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class AlgoMetaDataRepositoryTests
    {
        private const string ClientId = "066ABDEF-F1CB-4B24-8EE6-6ACAF1FD623D";

        private readonly Fixture _fixture = new Fixture();
        private AlgoClientMetaData _entity;
        private static bool _entitySaved;

        [SetUp]
        public void SetUp()
        {
            _entity = new AlgoClientMetaData
            {
                ClientId = ClientId,
                AlgoMetaData = new List<AlgoMetaData>
                {
                    _fixture.Build<AlgoMetaData>().Create()
                }
            };

        }

        [TearDown]
        public void CleanUp()
        {
            var repo = Given_AlgoMetaData_Repository();

            if (_entitySaved)
            {
                repo.DeleteAlgoMetaDataAsync(_entity.ClientId, _entity.AlgoMetaData[0].AlgoId).Wait();
                _entitySaved = false;
            }

            _entity = null;
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void AlgoMetaData_Save_Test()
        {
            var repo = Given_AlgoMetaData_Repository();
            When_Invoke_Save(repo, _entity);
            Then_Data_ShouldBe_Saved(repo, _entity);
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void AlgoMetaData_GetAll_Test()
        {
            var repo = Given_AlgoMetaData_Repository();
            When_Invoke_Save(repo, _entity);
            var all = When_Ivoke_GetAll(repo, ClientId);
            Then_Result_ShouldNotBe_Null(all);
        }

        #region Private Methods

        private static AlgoMetaDataRepository Given_AlgoMetaData_Repository()
        {
            return new AlgoMetaDataRepository(AzureTableStorage<AlgoMetaDataEntity>.Create(SettingsMock.GetSettings(), AlgoMetaDataRepository.TableName, new LogMock()));
        }

        private static void When_Invoke_Save(AlgoMetaDataRepository repository, AlgoClientMetaData data)
        {
            repository.SaveAlgoMetaDataAsync(data).Wait();
            _entitySaved = true;
        }

        private static void Then_Data_ShouldBe_Saved(AlgoMetaDataRepository repository, AlgoClientMetaData data)
        {
            var saved = repository.GetAlgoMetaDataAsync(ClientId, data.AlgoMetaData[0].AlgoId).Result;
            Assert.NotNull(saved);
        }

        private static AlgoClientMetaData When_Ivoke_GetAll(AlgoMetaDataRepository repository, string clientId)
        {
            return repository.GetAllClientAlgoMetaDataAsync(clientId).Result;
        }

        private static void Then_Result_ShouldNotBe_Null(AlgoClientMetaData data)
        {
            Assert.NotNull(data);
        }

        #endregion
    }
}
