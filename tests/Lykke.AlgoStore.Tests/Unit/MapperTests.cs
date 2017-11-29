using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Mapper;
using Lykke.AlgoStore.Core.Domain.Entities;
using Newtonsoft.Json;
using Xunit;

namespace Lykke.AlgoStore.Tests.Unit
{
    public class MapperTests
    {
        private const string PartitionKey = "PartitionKey";

        #region Data Generation
        public static class MockedAlgoClientMetaData
        {
            public static IEnumerable<object[]> TestAlgoClientMetaData()
            {
                int numberOfElements = 10;
                var fixture = new Fixture();
                var mock = Enumerable.Repeat(new object[1] { fixture.Build<AlgoClientMetaData>().Create() }, numberOfElements).ToList();
                return mock;
            }
        }
        public static class MockedAlgoMetaDataEntity
        {
            public static IEnumerable<object[]> TestAlgoMetaDataEntity()
            {
                int numberOfElements = 10;
                var fixture = new Fixture();
                var mock = Enumerable.Repeat(new object[1] { CreateData(fixture) }, numberOfElements).ToList();
                return mock;
            }

            private static AlgoMetaDataEntity CreateData(Fixture fixture)
            {
                return fixture.Build<AlgoMetaDataEntity>()
                    .With(entity => entity.PartitionKey, PartitionKey)
                    .With(entity => entity.ETag, "*")
                    .Without(entity => entity.Timestamp)
                    .Create();
            }
        }
        #endregion

        [Theory]
        [MemberData(nameof(MockedAlgoClientMetaData.TestAlgoClientMetaData), MemberType = typeof(MockedAlgoClientMetaData))]
        public void Mapper_ToEntity_Test(AlgoClientMetaData data)
        {
            AlgoClientMetaData metadata = data;

            var entities = When_Invoke_ToEntity(data);
            var result = When_Invoke_ToModel(entities);

            Then_Data_ShouldBe_Equal(metadata, result);
        }

        [Theory]
        [MemberData(nameof(MockedAlgoMetaDataEntity.TestAlgoMetaDataEntity), MemberType = typeof(MockedAlgoMetaDataEntity))]
        public void Mapper_ToModel_Test(AlgoMetaDataEntity data)
        {
            AlgoMetaDataEntity entity = data;

            var model = When_Invoke_ToModel(new List<AlgoMetaDataEntity> { entity });
            var result = When_Invoke_ToEntity(model);


            Then_Entity_ShouldBe_Equal(entity, result[0]);
        }

        private static List<AlgoMetaDataEntity> When_Invoke_ToEntity(AlgoClientMetaData data)
        {
            return data.ToEntity(PartitionKey);
        }
        private static AlgoClientMetaData When_Invoke_ToModel(List<AlgoMetaDataEntity> entities)
        {
            return entities.ToModel();
        }
        private static void Then_Data_ShouldBe_Equal(AlgoClientMetaData first, AlgoClientMetaData second)
        {
            string serializedFirst = JsonConvert.SerializeObject(first);
            string serializedSecond = JsonConvert.SerializeObject(second);

            Assert.Equal(serializedFirst, serializedSecond);
        }
        private static void Then_Entity_ShouldBe_Equal(AlgoMetaDataEntity first, AlgoMetaDataEntity second)
        {
            string serializedFirst = JsonConvert.SerializeObject(first);
            string serializedSecond = JsonConvert.SerializeObject(second);

            Assert.Equal(serializedFirst, serializedSecond);
        }
    }
}
