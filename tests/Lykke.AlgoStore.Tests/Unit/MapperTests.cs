using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Mapper;
using Lykke.AlgoStore.Core.Domain.Entities;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    public class MapperTests
    {
        private const string PartitionKey = "PartitionKey";

        #region Data Generation

        public static IEnumerable<object[]> AlgoClientMetaData => TestData<AlgoClientMetaData>();

        public static IEnumerable<object[]> AlgoClientRuntimeData => TestData<AlgoClientRuntimeData>();

        private static IEnumerable<object[]> TestData<T>()
        {
            int numberOfElements = 10;
            var fixture = new Fixture();
            var mock = Enumerable.Repeat(new object[1] { fixture.Build<T>().Create() }, numberOfElements).ToList();
            return mock;
        }

        //public static class MockedData
        //{
        //    public static IEnumerable<object[]> GetAlgoClientMetaData()
        //    {
        //        return TestData<AlgoClientMetaData>();
        //    }
        //    public static IEnumerable<object[]> GetAlgoClientRuntimeData()
        //    {
        //        return TestData<AlgoClientRuntimeData>();
        //    }

        //    private static IEnumerable<object[]> TestData<T>()
        //    {
        //        int numberOfElements = 10;
        //        var fixture = new Fixture();
        //        var mock = Enumerable.Repeat(new object[1] { fixture.Build<T>().Create() }, numberOfElements).ToList();
        //        return mock;
        //    }
        //}

        public static IEnumerable<object[]> AlgoMetaDataEntity => TestEntity<AlgoMetaDataEntity>();

        public static IEnumerable<object[]> AlgoRuntimeDataEntity => TestEntity<AlgoRuntimeDataEntity>();

        private static IEnumerable<object[]> TestEntity<T>() where T : TableEntity
        {
            int numberOfElements = 10;
            var fixture = new Fixture();
            var mock = Enumerable.Repeat(new object[1] { CreateData<T>(fixture) }, numberOfElements).ToList();
            return mock;
        }
        private static T CreateData<T>(Fixture fixture) where T : TableEntity
        {
            return fixture.Build<T>()
                .With(entity => entity.PartitionKey, PartitionKey)
                .With(entity => entity.ETag, "*")
                .Without(entity => entity.Timestamp)
                .Create();
        }

        //public static class MockedEntity
        //{
        //    public static IEnumerable<object[]> GetAlgoMetaDataEntity()
        //    {
        //        return TestEntity<AlgoMetaDataEntity>();
        //    }
        //    public static IEnumerable<object[]> GetAlgoRuntimeDataEntity()
        //    {
        //        return TestEntity<AlgoRuntimeDataEntity>();
        //    }

        //    private static IEnumerable<object[]> TestEntity<T>() where T : TableEntity
        //    {
        //        int numberOfElements = 10;
        //        var fixture = new Fixture();
        //        var mock = Enumerable.Repeat(new object[1] { CreateData<T>(fixture) }, numberOfElements).ToList();
        //        return mock;
        //    }
        //    private static T CreateData<T>(Fixture fixture) where T : TableEntity
        //    {
        //        return fixture.Build<T>()
        //            .With(entity => entity.PartitionKey, PartitionKey)
        //            .With(entity => entity.ETag, "*")
        //            .Without(entity => entity.Timestamp)
        //            .Create();
        //    }
        //}

        #endregion

        //[Theory]
        //[MemberData(nameof(MockedData.GetAlgoClientMetaData), MemberType = typeof(MockedData))]
        [TestCaseSource("AlgoClientMetaData")]
        public void Mapper_AlgoClientMetaData_ToEntity_Test(AlgoClientMetaData data)
        {
            AlgoClientMetaData metadata = data;

            var entities = When_Invoke_ToEntity(data);
            var result = When_Invoke_ToModel(entities);

            Then_Data_ShouldBe_Equal(metadata, result);
        }

        //[Theory]
        //[MemberData(nameof(MockedEntity.GetAlgoMetaDataEntity), MemberType = typeof(MockedEntity))]
        [TestCaseSource("AlgoMetaDataEntity")]
        public void Mapper_AlgoMetaDataEntity_ToModel_Test(AlgoMetaDataEntity data)
        {
            AlgoMetaDataEntity entity = data;

            var model = When_Invoke_ToModel(new List<AlgoMetaDataEntity> { entity });
            var result = When_Invoke_ToEntity(model);


            Then_Entity_ShouldBe_Equal(entity, result[0]);
        }

        //[Theory]
        //[MemberData(nameof(MockedData.GetAlgoClientRuntimeData), MemberType = typeof(MockedData))]
        [TestCaseSource("AlgoClientRuntimeData")]
        public void Mapper_AlgoClientRuntimeData_ToEntity_Test(AlgoClientRuntimeData data)
        {
            AlgoClientRuntimeData metadata = data;

            var entities = When_Invoke_ToEntity(data);
            var result = When_Invoke_ToModel(entities);

            Then_Data_ShouldBe_Equal(metadata, result);
        }

        //[Theory]
        //[MemberData(nameof(MockedEntity.GetAlgoRuntimeDataEntity), MemberType = typeof(MockedEntity))]
        [TestCaseSource("AlgoRuntimeDataEntity")]
        public void Mapper_AlgoRuntimeDataEntity_ToModel_Test(AlgoRuntimeDataEntity data)
        {
            AlgoRuntimeDataEntity entity = data;

            var model = When_Invoke_ToModel(new List<AlgoRuntimeDataEntity> { entity });
            var result = When_Invoke_ToEntity(model);


            Then_Entity_ShouldBe_Equal(entity, result[0]);
        }


        private static List<AlgoMetaDataEntity> When_Invoke_ToEntity(AlgoClientMetaData data)
        {
            return data.ToEntity(PartitionKey);
        }
        private static List<AlgoRuntimeDataEntity> When_Invoke_ToEntity(AlgoClientRuntimeData data)
        {
            return data.ToEntity(PartitionKey);
        }
        private static AlgoClientMetaData When_Invoke_ToModel(List<AlgoMetaDataEntity> entities)
        {
            return entities.ToModel();
        }
        private static AlgoClientRuntimeData When_Invoke_ToModel(List<AlgoRuntimeDataEntity> entities)
        {
            return entities.ToModel();
        }
        private static void Then_Data_ShouldBe_Equal(AlgoClientMetaData first, AlgoClientMetaData second)
        {
            string serializedFirst = JsonConvert.SerializeObject(first);
            string serializedSecond = JsonConvert.SerializeObject(second);

            Assert.AreEqual(serializedFirst, serializedSecond);
        }
        private static void Then_Data_ShouldBe_Equal(AlgoClientRuntimeData first, AlgoClientRuntimeData second)
        {
            string serializedFirst = JsonConvert.SerializeObject(first);
            string serializedSecond = JsonConvert.SerializeObject(second);

            Assert.AreEqual(serializedFirst, serializedSecond);
        }
        private static void Then_Entity_ShouldBe_Equal(AlgoMetaDataEntity first, AlgoMetaDataEntity second)
        {
            string serializedFirst = JsonConvert.SerializeObject(first);
            string serializedSecond = JsonConvert.SerializeObject(second);

            Assert.AreEqual(serializedFirst, serializedSecond);
        }
        private static void Then_Entity_ShouldBe_Equal(AlgoRuntimeDataEntity first, AlgoRuntimeDataEntity second)
        {
            string serializedFirst = JsonConvert.SerializeObject(first);
            string serializedSecond = JsonConvert.SerializeObject(second);

            Assert.AreEqual(serializedFirst, serializedSecond);
        }
    }
}
