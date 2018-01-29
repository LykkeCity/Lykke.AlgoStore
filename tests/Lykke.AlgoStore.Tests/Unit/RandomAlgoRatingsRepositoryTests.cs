using AutoFixture;
using AzureStorage.Tables;
using Lykke.AlgoStore.AzureRepositories.Repositories;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Tests.Infrastructure;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Tests.Unit
{
    class RandomAlgoRatingsRepositoryTests
    {

        private const string ClientId = "066ABDEF-F1CB-4B24-8EE6-6ACAF1FD623D";

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

        private static RandomAlgoRatingsRepository Given_AlgoRatings_Repository()
        {
            return new RandomAlgoRatingsRepository();
        }

        private static void When_Invoke_GetAlgoRating(RandomAlgoRatingsRepository repository, AlgoRatingMetaData data)
        {
            repository.GetAlgoRating(ClientId, data.AlgoId);
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
            Assert.NotNull(data.UsersCount);
        }
    }
}
