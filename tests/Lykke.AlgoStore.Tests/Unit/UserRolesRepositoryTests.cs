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
    public class UserRolesRepositoryTests
    {
        private const string ClientId = "066ABDEF-F1CB-4B24-8EE6-6ACAF1FD623D";
        private UserRoleData _entity;
        private readonly Fixture _fixture = new Fixture();
        private static bool _entitySaved;
        private readonly UserRolesRepository repo = new UserRolesRepository(AzureTableStorage<UserRoleEntity>.Create(SettingsMock.GetSettings(), UserRolesRepository.TableName, new LogMock()));

        [SetUp]
        public void SetUp()
        {
            _entity = _fixture.Build<UserRoleData>().With(role => role.Id, "TestID").With(role => role.Name, "TestName").Create();

        }

        [TearDown]
        public void CleanUp()
        {
            if (_entitySaved)
            {
                repo.DeleteRoleAsync(_entity).Wait();
                _entitySaved = false;
            }

            _entity = null;
        }

        [Test]
        public void CreateRoleTest()
        {
            When_Invoke_Save();
            Then_Data_ShouldBeSaved();
        }
        
        [Test]
        public void GetAllRolesTest()
        {
            var result = When_Invoke_GetAll();
            Then_Result_ShouldNotBe_Null(result);           
        }

        [Test]
        public void GetByIdTest()
        {
            var result = When_Invoke_GetById();
            Then_Data_ShouldNotBeNull(result);            
        }

        [Test]
        public void DeleteRoleTest()
        {
            When_Invoke_DeleteRole();
            Then_Role_ShouldNotExist();
        }

        private void Then_Role_ShouldNotExist()
        {
            var result = repo.GetRoleByIdAsync(_entity.Id);
            Assert.IsNull(result);
        }

        private void When_Invoke_DeleteRole()
        {
            repo.DeleteRoleAsync(_entity).Wait();
        }

        private void Then_Data_ShouldNotBeNull(UserRoleData result)
        {
            Assert.NotNull(result);
        }

        private UserRoleData When_Invoke_GetById()
        {
            return repo.GetRoleByIdAsync(_entity.Id).Result;
        }

        private List<UserRoleData> When_Invoke_GetAll()
        {
            return repo.GetAllRolesAsync().Result;
        }

        private UserRoleData When_Invoke_Save()
        {
            _entitySaved = true;
            return repo.SaveRoleAsync(_entity).Result;
        }

        private void Then_Data_ShouldBeSaved()
        {
            var result = repo.GetRoleByIdAsync(_entity.Id).Result;
            Assert.NotNull(result);
        }

        private void Then_Result_ShouldNotBe_Null(List<UserRoleData> data)
        {
            Assert.NotNull(data);
            Assert.NotZero(data.Count);
        }
    }
}
