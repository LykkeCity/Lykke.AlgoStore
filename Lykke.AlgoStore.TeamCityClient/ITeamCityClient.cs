using System.Threading.Tasks;
using Lykke.AlgoStore.TeamCityClient.Models;

namespace Lykke.AlgoStore.TeamCityClient
{
    public interface ITeamCityClient
    {
        Task<BuildBase> StartBuild(TeamCityClientBuildData buildData);
        Task<Build> GetBuildStatus(int buildId);
    }
}
