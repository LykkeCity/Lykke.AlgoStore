namespace Lykke.AlgoStore.Core.Settings.ServiceSettings
{
    public class AlgoApiSettings
    {
        public DbSettings Db { get; set; }
        public ServiceSettings Services { get; set; }
        public KubernetesSettings Kubernetes { get; set; }
        public TeamCitySettings TeamCity { get; set; }
    }
}
