namespace Lykke.AlgoStore.Core.Settings.ServiceSettings
{
    public class KubernetesSettings
    {
        /// <summary>
        /// The url of the Kubernetes instance to be used for deployments
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Bearer token for authentication against Kubernetes instance
        /// </summary>
        public string BasicAuthenticationValue { get; set; }

        /// <summary>
        /// The SHA1 hash value for the X.509v3 certificate of the Kubernetes 
        /// instance as a hexadecimal string
        /// </summary>
        public string CertificateHash { get; set; }
    }
}
