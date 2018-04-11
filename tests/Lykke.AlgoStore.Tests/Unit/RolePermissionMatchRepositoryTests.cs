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
    public class RolePermissionMatchRepositoryTests
    {
        private const string ClientId = "066ABDEF-F1CB-4B24-8EE6-6ACAF1FD623D";
        private RolePermissionMatchData _entity;
        private readonly Fixture _fixture = new Fixture();
        private static bool _entitySaved;
        private readonly RolePermissionMatchRepository repo = new RolePermissionMatchRepository(AzureTableStorage<RolePermissionMatchEntity>.Create(SettingsMock.GetSettings(), RolePermissionMatchRepository.TableName, new LogMock()));

        [SetUp]
        public void SetUp()
        {
            _entity = _fixture.Build<RolePermissionMatchData>().With(data => data.RoleId, "TestRoleId").With(data => data.PermissionId, "TestPermissionId").Create();
        }

        [TearDown]
        public void CleanUp()
        {
            if (_entitySaved)
            {
                repo.RevokePermission(_entity).Wait();
                _entitySaved = false;
            }

            _entity = null;
        }

        [Test]
        public void GetUserRolesTest()
        {
            var result = When_Invoke_GetPermissionsByRoleId();
            Then_Result_ShouldNotBe_Null(result);

        }

        [Test]
        public void RevokePermissionTest()
        {
            When_Invoke_RevokePermission();
            Then_Permission_ShouldNotExist();
        }

        private void Then_Permission_ShouldNotExist()
        {
            var result = repo.GetPermissionIdsByRoleIdAsync(_entity.RoleId).Result;
            Assert.IsNull(result);
        }

        private void When_Invoke_RevokePermission()
        {
            repo.RevokePermission(_entity).Wait();
        }

        private void Then_Data_ShouldNotBeNull(UserRoleMatchData result)
        {
            Assert.NotNull(result);
        }

        private List<RolePermissionMatchData> When_Invoke_GetPermissionsByRoleId()
        {
            return repo.GetPermissionIdsByRoleIdAsync(_entity.RoleId).Result;
        }

        private RolePermissionMatchData When_Invoke_AssignPermissionToRole()
        {
            _entitySaved = true;
            return repo.AssignPermissionToRoleAsync(_entity).Result;
        }

        private void Then_Data_ShouldBeSaved()
        {
            var result = repo.GetPermissionIdsByRoleIdAsync(_entity.RoleId).Result;
            Assert.NotNull(result);
        }

        private void Then_Result_ShouldNotBe_Null(List<RolePermissionMatchData> data)
        {
            Assert.NotNull(data);
            Assert.NotZero(data.Count);
        }
    }
}
