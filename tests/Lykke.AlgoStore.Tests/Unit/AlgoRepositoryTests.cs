using AutoFixture;
using AutoMapper;
using AzureStorage.Tables;
using Lykke.AlgoStore.Api.Infrastructure;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Tests.Infrastructure;
using NUnit.Framework;
using System.Collections.Generic;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class AlgoRepositoryTests
    {
        private const string ClientId = "066ABDEF-F1CB-4B24-8EE6-6ACAF1FD623D";

        private readonly Fixture _fixture = new Fixture();
        private IAlgo _entity;
        private static bool _entitySaved;

        [SetUp]
        public void SetUp()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperProfile>());
            Mapper.AssertConfigurationIsValid();

            var algoEntity = _fixture.Build<AlgoEntity>()
                .With(e => e.PartitionKey, ClientId)
                .Create();

            _entity = AutoMapper.Mapper.Map<IAlgo>(algoEntity);
        }

        [TearDown]
        public void CleanUp()
        {
            var repo = Given_AlgoMetaData_Repository();

            if (_entitySaved)
            {
                repo.DeleteAlgoAsync(_entity.ClientId, _entity.AlgoId).Wait();
                _entitySaved = false;
            }

            _entity = null;
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void Algo_Save_Test()
        {
            var repo = Given_AlgoMetaData_Repository();
            When_Invoke_Save(repo, _entity);
            Then_Data_ShouldBe_Saved(repo, _entity);
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void Algo_GetAll_Test()
        {
            var repo = Given_AlgoMetaData_Repository();
            When_Invoke_Save(repo, _entity);
            var all = When_Ivoke_GetAll(repo, ClientId);
            Then_Result_ShouldNotBe_Null(all);
        }

        #region Private Methods

        private static AlgoRepository Given_AlgoMetaData_Repository()
        {
            return new AlgoRepository(AzureTableStorage<AlgoEntity>.Create(SettingsMock.GetTableStorageConnectionString(), AlgoRepository.TableName, new LogMock()));
        }

        private static void When_Invoke_Save(AlgoRepository repository, IAlgo data)
        {
            repository.SaveAlgoAsync(data).Wait();
            _entitySaved = true;
        }

        private static void Then_Data_ShouldBe_Saved(AlgoRepository repository, IAlgo data)
        {
            var saved = repository.GetAlgoAsync(ClientId, data.AlgoId).Result;
            Assert.NotNull(saved);
        }

        private static IEnumerable<IAlgo> When_Ivoke_GetAll(AlgoRepository repository, string clientId)
        {
            return repository.GetAllClientAlgosAsync(clientId).Result;
        }

        private static void Then_Result_ShouldNotBe_Null(IEnumerable<IAlgo> data)
        {
            Assert.NotNull(data);
        }

        #endregion
    }

}
