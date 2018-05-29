using AutoFixture;
using AzureStorage.Tables;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Repositories;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Tests.Infrastructure;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class AlgoCommentsRepositoryTests
    {
        private const string ClientId = "066ABDEF-F1CB-4B24-8EE6-6ACAF1FD623D";
        private readonly string AlgoId = Guid.NewGuid().ToString();
        private AlgoCommentData _entity;
        private readonly Fixture _fixture = new Fixture();
        private static bool _entitySaved;

        [SetUp]
        public void SetUp()
        {
            _entity = _fixture.Build<AlgoCommentData>().With(e => e.Author, ClientId).With(e => e.AlgoId, AlgoId).Create();

        }

        [TearDown]
        public void CleanUp()
        {
            var repo = Given_AlgoCommentsRepository();

            if (_entitySaved)
            {
                repo.DeleteCommentAsync(AlgoId, _entity.CommentId).Wait();
                _entitySaved = false;
            }

            _entity = null;
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void AlgoMetaData_Save_Test()
        {
            var repo = Given_AlgoCommentsRepository();
            When_Invoke_Save(repo);

            Then_Data_ShouldBeSaved(repo);
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void AlgoMetaData_GetAll_Test()
        {
            var repo = Given_AlgoCommentsRepository();
            When_Invoke_Save(repo);
            var all = When_Invoke_GetAll(repo);
            Then_Result_ShouldNotBe_Null(all);
        }

        private AlgoCommentsRepository Given_AlgoCommentsRepository()
        {
            return new AlgoCommentsRepository(AzureTableStorage<AlgoCommentEntity>.Create(SettingsMock.GetTableStorageConnectionString(), AlgoCommentsRepository.TableName, new LogMock()));
        }

        private AlgoCommentData When_Invoke_Save(AlgoCommentsRepository repo)
        {
            return repo.SaveCommentAsync(_entity).Result;
        }

        private List<AlgoCommentData> When_Invoke_GetAll(AlgoCommentsRepository repo)
        {
            return repo.GetCommentsForAlgoAsync(AlgoId).Result;
        }

        private void Then_Data_ShouldBeSaved(AlgoCommentsRepository repo)
        {
            var entity = repo.GetCommentByIdAsync(_entity.AlgoId, _entity.CommentId).Result;
            Assert.NotNull(entity);
        }

        private void Then_Result_ShouldNotBe_Null(List<AlgoCommentData> data)
        {
            Assert.NotNull(data);
            Assert.NotZero(data.Count);
        }
    }
}
