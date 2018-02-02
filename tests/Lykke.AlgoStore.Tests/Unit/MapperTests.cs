using System;
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
    [TestFixture]
    public class MapperTests
    {
        private const string PartitionKey = "PartitionKey";
        private const string CompositePartitionKey = "ClientId_AlgoId";

        #region Data Generation

        private static IEnumerable<object[]> AlgoClientMetaData
        {
            get
            {
                var numberOfElements = 10;
                var fixture = new Fixture();

                var mock = Enumerable.Repeat(new object[]
                    {
                        new AlgoClientMetaData
                        {
                            ClientId = Guid.NewGuid().ToString(),
                            AlgoMetaData = new List<AlgoMetaData>
                            {
                                fixture.Build<AlgoMetaData>()
                                .With(data => data.Date, DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss"))
                                .Without(data => data.Status)
                                .Create()
                            }
                        }
                    },
                    numberOfElements
                ).ToList();

                return mock;
            }
        }
        private static IEnumerable<object[]> AlgoClientRuntimeData
        {
            get
            {
                var numberOfElements = 10;
                var fixture = new Fixture();
                var mock = Enumerable.Repeat(new object[] { fixture.Build<AlgoClientRuntimeData>().Create() }, numberOfElements).ToList();
                return mock;
            }
        }
        private static IEnumerable<object[]> AlgoClientInstanceData
        {
            get
            {
                var numberOfElements = 10;
                var fixture = new Fixture();
                var mock = Enumerable.Repeat(new object[] { fixture.Build<AlgoClientInstanceData>().Create() }, numberOfElements).ToList();
                return mock;
            }
        }


        private static IEnumerable<object[]> AlgoMetaDataEntity => TestEntity<AlgoMetaDataEntity>();
        private static IEnumerable<object[]> AlgoRuntimeDataEntity => TestEntity<AlgoRuntimeDataEntity>();
        private static IEnumerable<object[]> AlgoClientInstanceEntity
        {
            get
            {
                var numberOfElements = 10;
                var fixture = new Fixture();
                var mock = Enumerable.Repeat(new object[]
                {
                    fixture.Build<AlgoClientInstanceEntity>()
                    .With(entity => entity.PartitionKey, CompositePartitionKey)
                    .With(entity => entity.ETag, "*")
                    .Without(entity => entity.Timestamp)
                    .Create()
                }, numberOfElements).ToList();
                return mock;
            }
        }

        private static IEnumerable<object[]> TestEntity<T>() where T : TableEntity
        {
            int numberOfElements = 10;
            var fixture = new Fixture();
            var mock = Enumerable.Repeat(new object[] { CreateData<T>(fixture) }, numberOfElements).ToList();
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

        #endregion
        
        [TestCaseSource("AlgoClientMetaData")]
        public void Mapper_AlgoClientMetaData_ToEntity_Test(AlgoClientMetaData data)
        {
            AlgoClientMetaData metadata = data;

            var entities = When_Invoke_ToEntity(data);
            var result = When_Invoke_ToModel(entities);

            Then_Data_ShouldBe_Equal(metadata, result);
        }

        [TestCaseSource("AlgoMetaDataEntity")]
        public void Mapper_AlgoMetaDataEntity_ToModel_Test(AlgoMetaDataEntity data)
        {
            AlgoMetaDataEntity entity = data;

            var model = When_Invoke_ToModel(new List<AlgoMetaDataEntity> { entity });
            var result = When_Invoke_ToEntity(model);


            Then_Entity_ShouldBe_Equal(entity, result[0]);
        }

        [TestCaseSource("AlgoClientRuntimeData")]
        public void Mapper_AlgoClientRuntimeData_ToEntity_Test(AlgoClientRuntimeData data)
        {
            AlgoClientRuntimeData metadata = data;

            var entities = When_Invoke_ToEntity(data);
            var result = When_Invoke_ToModel(entities);

            Then_Data_ShouldBe_Equal(metadata, result);
        }

        [TestCaseSource("AlgoRuntimeDataEntity")]
        public void Mapper_AlgoRuntimeDataEntity_ToModel_Test(AlgoRuntimeDataEntity data)
        {
            AlgoRuntimeDataEntity entity = data;

            var model = When_Invoke_ToModel(entity);
            var result = When_Invoke_ToEntity(model);


            Then_Entity_ShouldBe_Equal(entity, result);
        }

        [TestCaseSource("AlgoClientInstanceData")]
        public void Mapper_AlgoClientInstanceData_ToEntity_Test(AlgoClientInstanceData data)
        {
            AlgoClientInstanceData metadata = data;

            var entities = When_Invoke_ToEntity(data);
            var result = When_Invoke_ToModel(entities);

            Then_Data_ShouldBe_Equal(metadata, result);
        }

        [TestCaseSource("AlgoClientInstanceEntity")]
        public void Mapper_AlgoClientInstanceEntity_ToModel_Test(AlgoClientInstanceEntity data)
        {
            AlgoClientInstanceEntity entity = data;

            var model = When_Invoke_ToModel(entity);
            var result = When_Invoke_ToEntity(model);


            Then_Entity_ShouldBe_Equal(entity, result);
        }

        #region Private Methods

        private static List<AlgoMetaDataEntity> When_Invoke_ToEntity(AlgoClientMetaData data)
        {
            return data.ToEntity();
        }
        private static AlgoClientMetaData When_Invoke_ToModel(List<AlgoMetaDataEntity> entities)
        {
            return entities.ToModel();
        }

        private static AlgoRuntimeDataEntity When_Invoke_ToEntity(AlgoClientRuntimeData data)
        {
            return data.ToEntity();
        }
        private static AlgoClientRuntimeData When_Invoke_ToModel(AlgoRuntimeDataEntity entities)
        {
            return entities.ToModel();
        }

        private static AlgoClientInstanceEntity When_Invoke_ToEntity(AlgoClientInstanceData data)
        {
            return data.ToEntity();
        }
        private static AlgoClientInstanceData When_Invoke_ToModel(AlgoClientInstanceEntity entities)
        {
            return entities.ToModel();
        }

        private static void Then_Data_ShouldBe_Equal(AlgoClientMetaData first, AlgoClientMetaData second)
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

        private static void Then_Data_ShouldBe_Equal(AlgoClientRuntimeData first, AlgoClientRuntimeData second)
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

        private static void Then_Data_ShouldBe_Equal(AlgoClientInstanceData first, AlgoClientInstanceData second)
        {
            string serializedFirst = JsonConvert.SerializeObject(first);
            string serializedSecond = JsonConvert.SerializeObject(second);

            Assert.AreEqual(serializedFirst, serializedSecond);
        }
        private static void Then_Entity_ShouldBe_Equal(AlgoClientInstanceEntity first, AlgoClientInstanceEntity second)
        {
            string serializedFirst = JsonConvert.SerializeObject(first);
            string serializedSecond = JsonConvert.SerializeObject(second);

            Assert.AreEqual(serializedFirst, serializedSecond);
        }
        #endregion
    }
}
