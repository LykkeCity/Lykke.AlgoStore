using System.Threading.Tasks;
using Lykke.SettingsReader;
using Moq;

namespace Lykke.AlgoStore.Tests.Infrastructure
{
    public static class SettingsMock
    {
        public static IReloadingManager<string> GetSettings()
        {
            var reloadingMock = new Mock<IReloadingManager<string>>();
            reloadingMock
                .Setup(x => x.Reload())
                .Returns(() => Task.FromResult("DefaultEndpointsProtocol=https;AccountName=algostoredev;AccountKey=d2VaBHrf8h8t622KvLeTPGwRP4Dxz9DTPeBT9H3zmjcQprQ1e+Z6Sx9RDqJc+zKwlSO900fzYF2Dg6oUBVDe1w=="));
            return reloadingMock.Object;
        }
    }
}
