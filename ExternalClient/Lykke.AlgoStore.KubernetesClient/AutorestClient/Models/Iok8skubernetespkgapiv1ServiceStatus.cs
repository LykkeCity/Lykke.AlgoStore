// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.KubernetesClient.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// ServiceStatus represents the current status of a service.
    /// </summary>
    public partial class Iok8skubernetespkgapiv1ServiceStatus
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1ServiceStatus class.
        /// </summary>
        public Iok8skubernetespkgapiv1ServiceStatus()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1ServiceStatus class.
        /// </summary>
        /// <param name="loadBalancer">LoadBalancer contains the current status
        /// of the load-balancer, if one is present.</param>
        public Iok8skubernetespkgapiv1ServiceStatus(Iok8skubernetespkgapiv1LoadBalancerStatus loadBalancer = default(Iok8skubernetespkgapiv1LoadBalancerStatus))
        {
            LoadBalancer = loadBalancer;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets loadBalancer contains the current status of the
        /// load-balancer, if one is present.
        /// </summary>
        [JsonProperty(PropertyName = "loadBalancer")]
        public Iok8skubernetespkgapiv1LoadBalancerStatus LoadBalancer { get; set; }

    }
}