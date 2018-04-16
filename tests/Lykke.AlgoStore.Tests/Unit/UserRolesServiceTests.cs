using AutoFixture;
using Lykke.AlgoStore.AzureRepositories.Repositories;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Tests.Infrastructure;
using Lykke.Service.PersonalData.Client;
using Lykke.Service.PersonalData.Contract;
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
    public class UserRolesServiceTests
    {
        private readonly string ClientId = Guid.NewGuid().ToString();
        private readonly string RoleId = Guid.NewGuid().ToString();
        private readonly UserRolesService service = Given_Correct_UserRolesService();        

        [Test]
        public void GetAllRolesTest()
        {
            var result = When_Invoke_GetAllRoles();
            Then_Result_ShouldNotBeEmpty(result);
            Then_Result_ShouldHavePermissions(result);
        }
        
        [Test]
        public void GetRoleByIdTest()
        {
            var result = When_Invoke_GetById();
            Then_Result_ShouldNotBeNull(result);
            Then_Result_ShouldHavePermissions(result);
        }
        
        [Test]
        public void GetRolesByClientIdTest()
        {
            var result = When_Invoke_GetByClientId();
            Then_Result_ShouldNotBeEmpty(result);
            Then_Result_ShouldHavePermissions(result);
        }

        [Test]
        public void AssignRoleToUserTest()
        {
            When_Invoke_AssignRoleToUser();

            var result = When_Invoke_GetByClientId();
            Then_Result_ShouldNotBeEmpty(result);
            Then_Result_ShouldHavePermissions(result);
        }

        private void When_Invoke_AssignRoleToUser()
        {
            var roleMatchData = new UserRoleMatchData()
            {
                RoleId = RoleId,
                ClientId = ClientId
            };
            service.AssignRoleToUser(roleMatchData).Wait();
        }

        private List<UserRoleData> When_Invoke_GetByClientId()
        {
            return service.GetRolesByClientIdAsync(ClientId).Result;
        }

        private UserRoleData When_Invoke_GetById()
        {
            return service.GetRoleByIdAsync(RoleId).Result;
        }

        private List<UserRoleData> When_Invoke_GetAllRoles()
        {
            return service.GetAllRolesAsync().Result;
        }

        public static IUserRolesRepository Given_Correct_UserRolesRepository()
        {
            var fixture = new Fixture();
            var result = new Mock<IUserRolesRepository>();

            result.Setup(repo => repo.GetAllRolesAsync()).Returns(() =>
            {
                var roles = new List<UserRoleData>();
                roles.AddRange(fixture.Build<UserRoleData>().CreateMany<UserRoleData>());
                return Task.FromResult(roles);
            });

            result.Setup(repo => repo.GetRoleByIdAsync(It.IsAny<string>())).Returns((string roleId) =>
            {
                var role = fixture.Build<UserRoleData>()
                .With(r => r.Id, roleId)
                .Create();
                return Task.FromResult(role);
            });

            result.Setup(repo => repo.SaveRoleAsync(It.IsAny<UserRoleData>())).Returns((UserRoleData data) =>
            {
                return Task.FromResult(data);
            });

            result.Setup(repo => repo.DeleteRoleAsync(It.IsAny<UserRoleData>())).Returns((UserRoleData data) =>
            {
                return Task.CompletedTask;
            });

            return result.Object;
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

        public static IUserRoleMatchRepository Given_Correct_UserRoleMatchRepository()
        {
            var fixture = new Fixture();
            var result = new Mock<IUserRoleMatchRepository>();

            result.Setup(repo => repo.GetUserRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Returns((string clientId, string roleId) =>
            {
                var role = fixture.Build<UserRoleMatchData>().With(d => d.ClientId, clientId).With(d => d.RoleId, roleId).Create();
                return Task.FromResult(role);
            });

            result.Setup(repo => repo.GetUserRolesAsync(It.IsAny<string>())).Returns((string clientId) =>
            {
                var roles = fixture.Build<UserRoleMatchData>().With(d => d.ClientId, clientId).CreateMany().ToList();
                return Task.FromResult(roles);
            });

            result.Setup(repo => repo.SaveUserRoleAsync(It.IsAny<UserRoleMatchData>())).Returns((UserRoleMatchData data) =>
            {
                return Task.FromResult(data);
            });

            result.Setup(repo => repo.RevokeUserRole(It.IsAny<string>(), It.IsAny<string>())).Returns(() =>
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

        public static UserRolesService Given_Correct_UserRolesService()
        {
            var userRolesRepository = Given_Correct_UserRolesRepository();
            var userPermissionsRepository = Given_Correct_UserPermissionsRepository();
            var userRoleMatchRepository = Given_Correct_UserRoleMatchRepository();
            var userPermissionsService = Given_Correct_PermissionsService();
            var rolePermissionMatchRepository = Given_Correct_RolePermissionMatchRepository();
            var personalDataService = Given_Correct_PersonalDataservice();
            return new UserRolesService(userRolesRepository, userPermissionsRepository, userRoleMatchRepository, userPermissionsService, rolePermissionMatchRepository, personalDataService, new LogMock());
        }

        public static IUserPermissionsService Given_Correct_PermissionsService()
        {
            var userPermissionsRepository = Given_Correct_UserPermissionsRepository();
            var rolePermissionMatchRepository = Given_Correct_RolePermissionMatchRepository();
            var userRolesRepository = Given_Correct_UserRolesRepository();
            return new UserPermissionsService(userPermissionsRepository, rolePermissionMatchRepository, userRolesRepository, new LogMock());
        }

        public static IPersonalDataService Given_Correct_PersonalDataservice()
        {
            var fixture = new Fixture();
            var result = new Mock<IPersonalDataService>();

            return result.Object;                
        }

        private void Then_Result_ShouldHavePermissions(List<UserRoleData> result)
        {
            foreach (var item in result)
            {
                Assert.NotNull(item.Permissions);
                Assert.NotZero(item.Permissions.Count);
            }
        }

        private void Then_Result_ShouldNotBeEmpty(List<UserRoleData> result)
        {
            Assert.NotNull(result);
            Assert.NotZero(result.Count);
        }

        private void Then_Result_ShouldHavePermissions(UserRoleData result)
        {
            Assert.NotNull(result);
            Assert.NotZero(result.Permissions.Count);
        }

        private void Then_Result_ShouldNotBeNull(UserRoleData result)
        {
            Assert.NotNull(result);
        }
    }
}
