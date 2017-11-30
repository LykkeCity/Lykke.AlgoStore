using System.Threading.Tasks;

namespace Lykke.AlgoStore.DockerClient
{
    public interface IExternalClient
    {
        Task<bool> UploadImage(byte[] data);
    }
}
