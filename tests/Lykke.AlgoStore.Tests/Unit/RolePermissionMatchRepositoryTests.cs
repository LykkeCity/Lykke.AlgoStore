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
    public class RolePermissionMatchRepositoryTests
    {
        private RolePermissionMatchData _entity;
        private readonly Fixture _fixture = new Fixture();

        private readonly RolePermissionMatchRepository _repo = new RolePermissionMatchRepository(
            AzureTableStorage<RolePermissionMatchEntity>.Create(SettingsMock.GetSettings(),
                RolePermissionMatchRepository.TableName, new LogMock()));

        [SetUp]
        public void SetUp()
        {
            _entity = _fixture.Build<RolePermissionMatchData>().With(data => data.RoleId, "TestRoleId")
                .With(data => data.PermissionId, "TestPermissionId").Create();
        }

        [TearDown]
        public void CleanUp()
        {
            _repo.RevokePermission(_entity).Wait();
            _entity = null;
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void GetPermissionsByRoleIdTest()
        {
            var result = When_Invoke_GetPermissionsByRoleId();
            Then_Result_ShouldNotBe_Null(result);
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void RevokePermissionFromRoleTest()
        {
            When_Invoke_RevokePermission();
            Then_Permission_ShouldNotExist();
        }

        private void Then_Permission_ShouldNotExist()
        {
            var result = _repo.GetPermissionIdsByRoleIdAsync(_entity.RoleId).Result;
            Assert.NotNull(result);
            Assert.Zero(result.Count);
        }

        private void When_Invoke_RevokePermission()
        {
            _repo.RevokePermission(_entity).Wait();
        }

        private List<RolePermissionMatchData> When_Invoke_GetPermissionsByRoleId()
        {
            // be sure to have a permission
            _repo.AssignPermissionToRoleAsync(_entity).Wait();

            return _repo.GetPermissionIdsByRoleIdAsync(_entity.RoleId).Result;
        }

        private static void Then_Result_ShouldNotBe_Null(List<RolePermissionMatchData> data)
        {
            Assert.NotNull(data);
            Assert.NotZero(data.Count);
        }
    }
}
