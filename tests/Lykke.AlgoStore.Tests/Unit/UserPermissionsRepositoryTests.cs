using AutoFixture;
using AzureStorage.Tables;
using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.AzureRepositories.Repositories;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Tests.Infrastructure;
using NUnit.Framework;
using System.Collections.Generic;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class UserPermissionsRepositoryTests
    {
        private UserPermissionData _entity;
        private readonly Fixture _fixture = new Fixture();

        private readonly UserPermissionsRepository _repo = new UserPermissionsRepository(
            AzureTableStorage<UserPermissionEntity>.Create(SettingsMock.GetTableStorageConnectionString(),
                UserPermissionsRepository.TableName, new LogMock()));

        [SetUp]
        public void SetUp()
        {
            _entity = _fixture.Build<UserPermissionData>().With(data => data.Id, "TestPermissionId")
                .With(data => data.Name, "TestPermission").With(data => data.DisplayName, "Test Permission").Create();
        }

        [TearDown]
        public void CleanUp()
        {
            _repo.DeletePermissionAsync(_entity).Wait();
            _entity = null;
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void SavePermissionTest()
        {
            When_Invoke_SavePermission();
            Then_Data_ShouldBeSaved();
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void GetPermissionByIdTest()
        {
            var result = When_Invoke_GetPermissionById();
            Then_Data_ShouldNotBeNull(result);
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void GetAllPermissionsTest()
        {
            When_Invoke_SavePermission();
            var result = When_Invoke_GetAllPermissions();
            Then_Result_ShouldNotBe_Null(result);
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void DeletePermissionTest()
        {
            When_Invoke_DeletePermission();
            Then_Permission_ShouldNotExist();
        }

        private void Then_Permission_ShouldNotExist()
        {
            var result = _repo.GetPermissionByIdAsync(_entity.Id).Result;
            Assert.IsNull(result);
        }

        private void When_Invoke_DeletePermission()
        {
            _repo.DeletePermissionAsync(_entity).Wait();
        }

        private static void Then_Data_ShouldNotBeNull(UserPermissionData result)
        {
            Assert.NotNull(result);
        }

        private UserPermissionData When_Invoke_GetPermissionById()
        {
            // be sure that the data is here
            _repo.SavePermissionAsync(_entity).Wait();

            return _repo.GetPermissionByIdAsync(_entity.Id).Result;
        }

        private List<UserPermissionData> When_Invoke_GetAllPermissions()
        {
            return _repo.GetAllPermissionsAsync().Result;
        }

        private UserPermissionData When_Invoke_SavePermission()
        {
            return _repo.SavePermissionAsync(_entity).Result;
        }

        private void Then_Data_ShouldBeSaved()
        {
            var result = _repo.GetPermissionByIdAsync(_entity.Id).Result;
            Assert.NotNull(result);
        }

        private static void Then_Result_ShouldNotBe_Null(List<UserPermissionData> data)
        {
            Assert.NotNull(data);
            Assert.NotZero(data.Count);
        }
    }
}
