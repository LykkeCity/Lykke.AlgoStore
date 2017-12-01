using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Lykke.AlgoStore.AzureRepositories.Repositories;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Tests.Infrastructure;
using Lykke.SettingsReader;
using Moq;
using Xunit;

namespace Lykke.AlgoStore.Tests.Unit
{
    public class AlgoRuntimeDataRepositoryTests : IDisposable
    {
        private const string ClientAlgoId = "{F5385D58-137B-4E3D-BD75-E577A8EB38AA}";

        private Fixture _fixture = new Fixture();
        private AlgoClientRuntimeData _entity = null;

        public AlgoRuntimeDataRepositoryTests()
        {
            SetUp();
        }
        public void Dispose()
        {
            CleanUp();
        }


        [RunnableInDebugOnly("Should run manually only. Manipulate data in Table Storage")]
        public void AlgoMetaData_Save_Test()
        {
            var repo = Given_AlgoRuntimeData_Repository();
            When_Invoke_Save(repo, _entity);
            Then_Data_ShouldBe_Saved(repo, _entity);
            Then_Result_ShouldNotBe_Null(repo, _entity);
        }

        private static AlgoRuntimeDataRepository Given_AlgoRuntimeData_Repository()
        {
            return new AlgoRuntimeDataRepository(GetSettings(), new LogMock());
        }
        private static void When_Invoke_Save(AlgoRuntimeDataRepository repository, AlgoClientRuntimeData data)
        {
            repository.SaveAlgoRuntimeData(data).Wait();
        }
        private static void Then_Data_ShouldBe_Saved(AlgoRuntimeDataRepository repository, AlgoClientRuntimeData data)
        {
            var saved = repository.GetAlgoRuntimeDataByAlgo(data.ClientAlgoId).Result;
            Assert.NotNull(saved);
        }        
        private static void Then_Result_ShouldNotBe_Null(AlgoRuntimeDataRepository repository, AlgoClientRuntimeData data)
        {
            var saved = repository.GetAlgoRuntimeData(data.RuntimeData[0].ImageId).Result;
            Assert.NotNull(saved);
        }

        private static IReloadingManager<string> GetSettings()
        {
            var reloadingMock = new Mock<IReloadingManager<string>>();
            reloadingMock
                .Setup(x => x.Reload())
                .Returns(() => Task.FromResult("DefaultEndpointsProtocol=https;AccountName=lkedevmain;AccountKey=l0W0CaoNiRZQIqJ536sIScSV5fUuQmPYRQYohj/UjO7+ZVdpUiEsRLtQMxD+1szNuAeJ351ndkOsdWFzWBXmdw=="));
            return reloadingMock.Object;
        }

        private void SetUp()
        {
            _entity = new AlgoClientRuntimeData();
            _entity.ClientAlgoId = ClientAlgoId;
            _entity.RuntimeData = new List<AlgoRuntimeData>();

            _entity.RuntimeData.Add(_fixture.Build<AlgoRuntimeData>().Create());
        }
        private void CleanUp()
        {
            var repo = Given_AlgoRuntimeData_Repository();
            repo.DeleteAlgoRuntimeData(_entity.RuntimeData[0].ImageId).Wait();
            _entity = null;
        }


    }
}
