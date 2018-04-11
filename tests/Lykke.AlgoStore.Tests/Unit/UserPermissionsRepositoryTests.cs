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
    [TestFixture]
    public class UserPermissionsRepositoryTests
    {
        private const string ClientId = "066ABDEF-F1CB-4B24-8EE6-6ACAF1FD623D";
        private UserPermissionData _entity;
        private readonly Fixture _fixture = new Fixture();
        private static bool _entitySaved;
        private readonly UserPermissionsRepository repo = new UserPermissionsRepository(AzureTableStorage<UserPermissionEntity>.Create(SettingsMock.GetSettings(), UserPermissionsRepository.TableName, new LogMock()));

        [SetUp]
        public void SetUp()
        {
            _entity = _fixture.Build<UserPermissionData>().With(data => data.Id, "TestPermissionId").With(data => data.Name, "TestPermission").With(data => data.DisplayName, "Test Permission").Create();
        }

        [TearDown]
        public void CleanUp()
        {
            if (_entitySaved)
            {
                repo.DeletePermissionAsync(_entity).Wait();
                _entitySaved = false;
            }

            _entity = null;
        }

        [Test]
        public void SavePermissionTest()
        {
            When_Invoke_SavePermission();
            Then_Data_ShouldBeSaved();
        }

        [Test]
        public void GetPermissionByIdTest()
        {
            var result = When_Invoke_GetPermissionById();
            Then_Data_ShouldNotBeNull(result);

        }

        [Test]
        public void GetAllPermissionsTest()
        {
            var result = When_Invoke_GetAllPermissions();
            Then_Result_ShouldNotBe_Null(result);
        }

        [Test]
        public void DeleteRoleTest()
        {
            When_Invoke_DeletePermission();
            Then_Permission_ShouldNotExist();
        }

        private void Then_Permission_ShouldNotExist()
        {
            var result = repo.GetPermissionByIdAsync(_entity.Id);
            Assert.IsNull(result);
        }

        private void When_Invoke_DeletePermission()
        {
            repo.DeletePermissionAsync(_entity).Wait();
        }

        private void Then_Data_ShouldNotBeNull(UserPermissionData result)
        {
            Assert.NotNull(result);
        }

        private UserPermissionData When_Invoke_GetPermissionById()
        {
            return repo.GetPermissionByIdAsync(_entity.Id).Result;
        }

        private List<UserPermissionData> When_Invoke_GetAllPermissions()
        {
            return repo.GetAllPermissionsAsync().Result;
        }

        private UserPermissionData When_Invoke_SavePermission()
        {
            _entitySaved = true;
            return repo.SavePermissionAsync(_entity).Result;
        }

        private void Then_Data_ShouldBeSaved()
        {
            var result = repo.GetPermissionByIdAsync(_entity.Id).Result;
            Assert.NotNull(result);
        }

        private void Then_Result_ShouldNotBe_Null(List<UserPermissionData> data)
        {
            Assert.NotNull(data);
            Assert.NotZero(data.Count);
        }
    }
}
