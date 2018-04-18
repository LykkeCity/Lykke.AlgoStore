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
    public class UserRolesRepositoryTests
    {
        private UserRoleData _entity;
        private readonly Fixture _fixture = new Fixture();

        private readonly UserRolesRepository _repo = new UserRolesRepository(
            AzureTableStorage<UserRoleEntity>.Create(SettingsMock.GetSettings(), UserRolesRepository.TableName,
                new LogMock()));

        [SetUp]
        public void SetUp()
        {
            _entity = _fixture.Build<UserRoleData>().With(role => role.Id, "TestID").With(role => role.Name, "TestName")
                .Create();
        }

        [TearDown]
        public void CleanUp()
        {
            _repo.DeleteRoleAsync(_entity).Wait();
            _entity = null;
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void CreateRoleTest()
        {
            When_Invoke_Save();
            Then_Data_ShouldBeSaved();
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void GetAllRolesTest()
        {
            var result = When_Invoke_GetAll();
            Then_Result_ShouldNotBe_Null(result);
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void GetByIdTest()
        {
            var result = When_Invoke_GetById();
            Then_Data_ShouldNotBeNull(result);
        }

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void DeleteRoleTest()
        {
            When_Invoke_DeleteRole();
            Then_Role_ShouldNotExist();
        }

        private void Then_Role_ShouldNotExist()
        {
            var result = _repo.GetRoleByIdAsync(_entity.Id).Result;
            Assert.IsNull(result);
        }

        private void When_Invoke_DeleteRole()
        {
            _repo.DeleteRoleAsync(_entity).Wait();
        }

        private void Then_Data_ShouldNotBeNull(UserRoleData result)
        {
            Assert.NotNull(result);
        }

        private UserRoleData When_Invoke_GetById()
        {
            // be sure the item is here
            _repo.SaveRoleAsync(_entity).Wait();

            return _repo.GetRoleByIdAsync(_entity.Id).Result;
        }

        private List<UserRoleData> When_Invoke_GetAll()
        {
            return _repo.GetAllRolesAsync().Result;
        }

        private UserRoleData When_Invoke_Save()
        {
            return _repo.SaveRoleAsync(_entity).Result;
        }

        private void Then_Data_ShouldBeSaved()
        {
            var result = _repo.GetRoleByIdAsync(_entity.Id).Result;
            Assert.NotNull(result);
        }

        private static void Then_Result_ShouldNotBe_Null(List<UserRoleData> data)
        {
            Assert.NotNull(data);
            Assert.NotZero(data.Count);
        }
    }
}
