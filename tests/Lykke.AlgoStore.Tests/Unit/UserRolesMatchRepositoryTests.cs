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
    public class UserRolesMatchRepositoryTests
    {
        private const string ClientId = "066ABDEF-F1CB-4B24-8EE6-6ACAF1FD623D";
        private UserRoleMatchData _entity;
        private readonly Fixture _fixture = new Fixture();
        private readonly UserRolesMatchRepository repo = new UserRolesMatchRepository(AzureTableStorage<UserRoleMatchEntity>.Create(SettingsMock.GetSettings(), UserRolesMatchRepository.TableName, new LogMock()));

        [SetUp]
        public void SetUp()
        {
            _entity = _fixture.Build<UserRoleMatchData>().With(data => data.RoleId, "TestRoleId").With(data => data.ClientId, ClientId).Create();
        }

        [TearDown]
        public void CleanUp()
        {
            repo.RevokeUserRole(_entity.ClientId, _entity.RoleId).Wait();
            _entity = null;
        }

        [Test]
        public void AssignUserRoleTest()
        {
            When_Invoke_AssignUserRole();
            Then_Data_ShouldBeSaved();
        }

        [Test]
        public void GetUserRolesTest()
        {
            var result = When_Invoke_GetUserRoles();
            Then_Result_ShouldNotBe_Null(result);

        }

        [Test]
        public void GetUserRoleTest()
        {
            var result = When_Invoke_GetUserRole();
            Then_Data_ShouldNotBeNull(result);
        }

        [Test]
        public void RevokeUserRoleTest()
        {
            When_Invoke_RevokeUserRole();
            Then_Role_ShouldNotExist();
        }

        private void Then_Role_ShouldNotExist()
        {
            var result = repo.GetUserRoleAsync(_entity.ClientId, _entity.RoleId).Result;
            Assert.IsNull(result);
        }

        private void When_Invoke_RevokeUserRole()
        {
            repo.RevokeUserRole(_entity.ClientId, _entity.RoleId).Wait();
        }

        private void Then_Data_ShouldNotBeNull(UserRoleMatchData result)
        {
            Assert.NotNull(result);
        }

        private UserRoleMatchData When_Invoke_GetUserRole()
        {
            // be sure to have a role
            repo.SaveUserRoleAsync(_entity).Wait();

            return repo.GetUserRoleAsync(_entity.ClientId, _entity.RoleId).Result;
        }

        private List<UserRoleMatchData> When_Invoke_GetUserRoles()
        {
            // be sure to have a role
            repo.SaveUserRoleAsync(_entity).Wait();

            return repo.GetUserRolesAsync(_entity.ClientId).Result;
        }

        private UserRoleMatchData When_Invoke_AssignUserRole()
        {
            return repo.SaveUserRoleAsync(_entity).Result;
        }

        private void Then_Data_ShouldBeSaved()
        {
            var result = repo.GetUserRoleAsync(_entity.ClientId, _entity.RoleId).Result;
            Assert.NotNull(result);
        }

        private void Then_Result_ShouldNotBe_Null(List<UserRoleMatchData> data)
        {
            Assert.NotNull(data);
            Assert.NotZero(data.Count);
        }
    }
}
