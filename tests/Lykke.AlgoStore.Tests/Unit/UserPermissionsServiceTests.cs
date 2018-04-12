using AutoFixture;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Tests.Infrastructure;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class UserPermissionsServiceTests
    {
        private readonly string PermissionId = Guid.NewGuid().ToString();
        private readonly string RoleId = Guid.NewGuid().ToString();
        private readonly UserPermissionsService service = Given_Correct_PermissionsService();

        [Test]
        public void GetAllPermissionsTest()
        {
            var result = When_Invoke_GetAllPermissions();
            Then_Result_ShouldNotBeEmpty(result);
        }

        [Test]
        public void GetPermissionByIdTest()
        {
            var result = When_Invoke_GetPermissionById();
            Then_Result_ShouldNotBeNull(result);
        }

        [Test]
        public void GetPermissionsByRoleIdTest()
        {
            var result = When_Invoke_GetPermissionsByRoleId();
            Then_Result_ShouldNotBeEmpty(result);
        }

        private List<UserPermissionData> When_Invoke_GetPermissionsByRoleId()
        {
            return service.GetPermissionsByRoleIdAsync(RoleId).Result;
        }

        private UserPermissionData When_Invoke_GetPermissionById()
        {
            return service.GetPermissionByIdAsync(PermissionId).Result;
        }

        private List<UserPermissionData> When_Invoke_GetAllPermissions()
        {
            return service.GetAllPermissionsAsync().Result;
        }

        public static UserPermissionsService Given_Correct_PermissionsService()
        {
            var userPermissionsRepository = Given_Correct_UserPermissionsRepository();
            var rolePermissionMatchRepository = Given_Correct_RolePermissionMatchRepository();
            return new UserPermissionsService(userPermissionsRepository, rolePermissionMatchRepository, new LogMock());
        }

        public static IUserPermissionsRepository Given_Correct_UserPermissionsRepository()
        {
            var fixture = new Fixture();
            var result = new Mock<IUserPermissionsRepository>();

            result.Setup(repo => repo.GetAllPermissionsAsync()).Returns(() =>
            {
                var permissions = new List<UserPermissionData>();
                permissions.AddRange(fixture.Build<UserPermissionData>().CreateMany<UserPermissionData>());
                return Task.FromResult(permissions);
            });

            result.Setup(repo => repo.GetPermissionByIdAsync(It.IsAny<string>())).Returns((string permissionId) =>
            {
                var permission = fixture.Build<UserPermissionData>().With(p => p.Id, permissionId).Create();
                return Task.FromResult(permission);
            });

            result.Setup(repo => repo.SavePermissionAsync(It.IsAny<UserPermissionData>())).Returns((UserPermissionData data) =>
            {
                return Task.FromResult(data);
            });

            result.Setup(repo => repo.DeletePermissionAsync(It.IsAny<UserPermissionData>())).Returns(() =>
            {
                return Task.CompletedTask;
            });

            return result.Object;
        }

        public static IRolePermissionMatchRepository Given_Correct_RolePermissionMatchRepository()
        {
            var fixture = new Fixture();
            var result = new Mock<IRolePermissionMatchRepository>();

            result.Setup(repo => repo.GetPermissionIdsByRoleIdAsync(It.IsAny<string>())).Returns((string roleId) =>
            {
                var role = fixture.Build<RolePermissionMatchData>().With(d => d.RoleId, roleId).CreateMany().ToList();
                return Task.FromResult(role);
            });

            result.Setup(repo => repo.AssignPermissionToRoleAsync(It.IsAny<RolePermissionMatchData>())).Returns((RolePermissionMatchData data) =>
            {
                return Task.FromResult(data);
            });

            result.Setup(repo => repo.RevokePermission(It.IsAny<RolePermissionMatchData>())).Returns(() =>
            {
                return Task.CompletedTask;
            });

            return result.Object;
        }

        private void Then_Result_ShouldNotBeEmpty(List<UserPermissionData> result)
        {
            Assert.NotNull(result);
            Assert.NotZero(result.Count);
        }

        private void Then_Result_ShouldNotBeNull(UserPermissionData result)
        {
            Assert.NotNull(result);
        }
    }
}
