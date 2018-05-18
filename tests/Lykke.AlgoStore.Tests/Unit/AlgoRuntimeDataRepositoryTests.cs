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
    public class AlgoRuntimeDataRepositoryTests
    {
        private const string ClientId = "F5385D58-137B-4E3D-BD75-E577A8EB38AA";
        private const string AlgoId = "26A253A1-7A16-495C-8EB9-F6AE1F310A33";

        private readonly Fixture _fixture = new Fixture();
        private AlgoClientRuntimeData _entity;
        private static bool _entitySaved;

        [SetUp]
        public void SetUp()
        {
            _entity = _fixture.Build<AlgoClientRuntimeData>()
                .With(d => d.AlgoId, AlgoId)
                .With(d => d.ClientId, ClientId)
                .Create();
        }

        [TearDown]
        public void CleanUp()
        {
            var repo = Given_AlgoRuntimeData_Repository();

            if (_entitySaved)
            {
                repo.DeleteAlgoRuntimeDataAsync(_entity.ClientId, _entity.AlgoId).Wait();
                _entitySaved = false;
            }

            _entity = null;
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void AlgoRuntimeData_Save_Test()
        {
            var repo = Given_AlgoRuntimeData_Repository();
            When_Invoke_Save(repo, _entity);
            Then_Data_ShouldBe_Saved(repo, _entity);
            Then_Result_ShouldNotBe_Null(repo, _entity);
        }

        #region Private Methods

        private static AlgoRuntimeDataRepository Given_AlgoRuntimeData_Repository()
        {
            return new AlgoRuntimeDataRepository(AzureTableStorage<AlgoRuntimeDataEntity>.Create(SettingsMock.GetTableStorageConnectionString(), AlgoRuntimeDataRepository.TableName, new LogMock()));
        }

        private static void When_Invoke_Save(AlgoRuntimeDataRepository repository, AlgoClientRuntimeData data)
        {
            repository.SaveAlgoRuntimeDataAsync(data).Wait();
            _entitySaved = true;
        }

        private static void Then_Data_ShouldBe_Saved(AlgoRuntimeDataRepository repository, AlgoClientRuntimeData data)
        {
            var saved = repository.GetAlgoRuntimeDataAsync(data.ClientId, data.AlgoId).Result;
            Assert.NotNull(saved);
        }

        private static void Then_Result_ShouldNotBe_Null(AlgoRuntimeDataRepository repository, AlgoClientRuntimeData data)
        {
            var saved = repository.GetAlgoRuntimeDataAsync(data.ClientId, data.AlgoId).Result;
            Assert.NotNull(saved);
        }

        #endregion
    }
}
