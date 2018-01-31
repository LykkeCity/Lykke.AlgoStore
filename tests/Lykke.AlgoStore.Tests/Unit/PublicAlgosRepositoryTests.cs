using AutoFixture;
using AzureStorage.Tables;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Repositories;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Tests.Infrastructure;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Tests.Unit
{
    class PublicAlgosRepositoryTests
    {
        private const string ClientId = "066ABDEF-F1CB-4B24-8EE6-6ACAF1FD623D";

        private readonly Fixture _fixture = new Fixture();
        private PublicAlgoData _entity;
        private static bool _entitySaved;

        [SetUp]
        public void SetUp()
        {
            _entity = new PublicAlgoData
            {
                ClientId = ClientId,
                AlgoId = Guid.NewGuid().ToString()
            };

        }

        [TearDown]
        public void CleanUp()
        {
            var repo = Given_AlgoMetaData_Repository();

            if (_entitySaved)
            {
                repo.DeletePublicAlgoAsync(_entity).Wait();
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
            var all = When_Ivoke_GetAll(repo);
            Then_Result_ShouldNotBe_Null(all);
        }

        #region Private Methods

        private static PublicAlgosRepository Given_AlgoMetaData_Repository()
        {
            return new PublicAlgosRepository(AzureTableStorage<PublicAlgoEntity>.Create(SettingsMock.GetSettings(), PublicAlgosRepository.TableName, new LogMock()));
        }

        private static void When_Invoke_Save(PublicAlgosRepository repository, PublicAlgoData data)
        {
            repository.SavePublicAlgoAsync(data).Wait();
            _entitySaved = true;
        }

        private static void Then_Data_ShouldBe_Saved(PublicAlgosRepository repository, PublicAlgoData data)
        {
            var saved = repository.ExistsPublicAlgoAsync(ClientId, data.AlgoId).Result;
            Assert.NotNull(saved);
        }

        private static List<PublicAlgoData> When_Ivoke_GetAll(PublicAlgosRepository repository)
        {
            return repository.GetAllPublicAlgosAsync().Result;
        }

        private static void Then_Result_ShouldNotBe_Null(List<PublicAlgoData> data)
        {
            Assert.NotNull(data);
        }

        #endregion
    }
}
