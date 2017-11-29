using System.Threading.Tasks;

namespace Lykke.AlgoStore.DockerClient
{
    public class ExternalClient : IExternalClient
    {
        public async Task<bool> UploadImage(byte[] data)
        {
            return await Task.FromResult(true);
        }
    }
}
