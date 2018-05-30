// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.KubernetesClient.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// LoadBalancerStatus represents the status of a load-balancer.
    /// </summary>
    public partial class Iok8skubernetespkgapiv1LoadBalancerStatus
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1LoadBalancerStatus class.
        /// </summary>
        public Iok8skubernetespkgapiv1LoadBalancerStatus()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1LoadBalancerStatus class.
        /// </summary>
        /// <param name="ingress">Ingress is a list containing ingress points
        /// for the load-balancer. Traffic intended for the service should be
        /// sent to these ingress points.</param>
        public Iok8skubernetespkgapiv1LoadBalancerStatus(IList<Iok8skubernetespkgapiv1LoadBalancerIngress> ingress = default(IList<Iok8skubernetespkgapiv1LoadBalancerIngress>))
        {
            Ingress = ingress;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets ingress is a list containing ingress points for the
        /// load-balancer. Traffic intended for the service should be sent to
        /// these ingress points.
        /// </summary>
        [JsonProperty(PropertyName = "ingress")]
        public IList<Iok8skubernetespkgapiv1LoadBalancerIngress> Ingress { get; set; }

    }
}
