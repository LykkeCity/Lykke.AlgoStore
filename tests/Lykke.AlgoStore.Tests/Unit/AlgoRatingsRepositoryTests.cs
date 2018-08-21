using System.Threading.Tasks;
using AutoFixture;
using AzureStorage;
using AzureStorage.Tables;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Tests.Infrastructure;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    class AlgoRatingsRepositoryTests
    {

        private const string ClientId = "066ABDEF-F1CB-4B24-8EE6-6ACAF1FD623D";
        private const string PartitionKey = "066ABDEF-F1CB-4B24-8EE6-6ACAF1FD6264";

        private readonly Fixture _fixture = new Fixture();
        private AlgoRatingMetaData _entity;

        [SetUp]
        public void SetUp()
        {
            _entity = new AlgoRatingMetaData();
        }

        [Test]
        public void AlgoRating_GenerateRating_Test()
        {
            var repo = Given_AlgoRatings_Repository();
            When_Invoke_GetAlgoRating(repo, _entity);
            Then_Rating_ShouldNotBe_Null(_entity);
            Then_UsersCount_ShouldNotBe_Null(_entity);
        }

        private static AlgoRatingsRepository Given_AlgoRatings_Repository()
        {
            return new AlgoRatingsRepository(AzureTableStorage<AlgoRatingEntity>.Create(SettingsMock.GetTableStorageConnectionString(), AlgoRepository.TableName, new LogMock()));
        }

        private static void When_Invoke_GetAlgoRating(AlgoRatingsRepository repository, AlgoRatingMetaData data)
        {
            repository.GetAlgoRatingsAsync(data.AlgoId);
        }

        private static void Then_Result_ShouldNotBe_Null(AlgoRatingMetaData data)
        {
            Assert.NotNull(data);
        }

        private static void Then_Rating_ShouldNotBe_Null(AlgoRatingMetaData data)
        {
            Assert.NotNull(data.Rating);
        }

        private static void Then_UsersCount_ShouldNotBe_Null(AlgoRatingMetaData data)
        {
            Assert.NotNull(data.UsesCount);
        }
    }
}
