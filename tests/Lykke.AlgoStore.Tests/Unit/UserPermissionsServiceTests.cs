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
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class UserPermissionsServiceTests
    {
        private readonly string _permissionId = Guid.NewGuid().ToString();
        private readonly string _roleId = Guid.NewGuid().ToString();
        private readonly UserPermissionsService _service = Given_Correct_PermissionsService();

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
            return _service.GetPermissionsByRoleIdAsync(_roleId).Result;
        }

        private UserPermissionData When_Invoke_GetPermissionById()
        {
            return _service.GetPermissionByIdAsync(_permissionId).Result;
        }

        private List<UserPermissionData> When_Invoke_GetAllPermissions()
        {
            return _service.GetAllPermissionsAsync().Result;
        }

        public static UserPermissionsService Given_Correct_PermissionsService()
        {
            var userPermissionsRepository = Given_Correct_UserPermissionsRepository();
            var rolePermissionMatchRepository = Given_Correct_RolePermissionMatchRepository();
            var rolesRepository = Given_Correct_UserRolesRepository();
            return new UserPermissionsService(userPermissionsRepository, rolePermissionMatchRepository, rolesRepository,
                new LogMock());
        }

        public static IUserRolesRepository Given_Correct_UserRolesRepository()
        {
            var fixture = new Fixture();
            var result = new Mock<IUserRolesRepository>();

            result.Setup(repo => repo.GetAllRolesAsync()).Returns(() =>
            {
                var roles = new List<UserRoleData>();
                roles.AddRange(fixture.Build<UserRoleData>().CreateMany());
                return Task.FromResult(roles);
            });

            result.Setup(repo => repo.GetRoleByIdAsync(It.IsAny<string>())).Returns((string roleId) =>
            {
                var role = fixture.Build<UserRoleData>()
                    .With(r => r.Id, roleId)
                    .Create();
                return Task.FromResult(role);
            });

            result.Setup(repo => repo.SaveRoleAsync(It.IsAny<UserRoleData>()))
                .Returns((UserRoleData data) => Task.FromResult(data));

            result.Setup(repo => repo.DeleteRoleAsync(It.IsAny<UserRoleData>()))
                .Returns((UserRoleData data) => Task.CompletedTask);

            return result.Object;
        }

        public static IUserPermissionsRepository Given_Correct_UserPermissionsRepository()
        {
            var fixture = new Fixture();
            var result = new Mock<IUserPermissionsRepository>();

            result.Setup(repo => repo.GetAllPermissionsAsync()).Returns(() =>
            {
                var permissions = new List<UserPermissionData>();
                permissions.AddRange(fixture.Build<UserPermissionData>().CreateMany());
                return Task.FromResult(permissions);
            });

            result.Setup(repo => repo.GetPermissionByIdAsync(It.IsAny<string>())).Returns((string permissionId) =>
            {
                var permission = fixture.Build<UserPermissionData>().With(p => p.Id, permissionId).Create();
                return Task.FromResult(permission);
            });

            result.Setup(repo => repo.SavePermissionAsync(It.IsAny<UserPermissionData>()))
                .Returns((UserPermissionData data) => { return Task.FromResult(data); });

            result.Setup(repo => repo.DeletePermissionAsync(It.IsAny<UserPermissionData>()))
                .Returns(() => { return Task.CompletedTask; });

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

            result.Setup(repo => repo.AssignPermissionToRoleAsync(It.IsAny<RolePermissionMatchData>()))
                .Returns((RolePermissionMatchData data) => Task.FromResult(data));

            result.Setup(repo => repo.RevokePermission(It.IsAny<RolePermissionMatchData>()))
                .Returns(() => Task.CompletedTask);

            return result.Object;
        }

        private static void Then_Result_ShouldNotBeEmpty(List<UserPermissionData> result)
        {
            Assert.NotNull(result);
            Assert.NotZero(result.Count);
        }

        private static void Then_Result_ShouldNotBeNull(UserPermissionData result)
        {
            Assert.NotNull(result);
        }
    }
}
