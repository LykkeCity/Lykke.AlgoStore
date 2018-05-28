// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.KubernetesClient.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// IngressStatus describe the current state of the Ingress.
    /// </summary>
    public partial class Iok8skubernetespkgapisextensionsv1beta1IngressStatus
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisextensionsv1beta1IngressStatus class.
        /// </summary>
        public Iok8skubernetespkgapisextensionsv1beta1IngressStatus()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisextensionsv1beta1IngressStatus class.
        /// </summary>
        /// <param name="loadBalancer">LoadBalancer contains the current status
        /// of the load-balancer.</param>
        public Iok8skubernetespkgapisextensionsv1beta1IngressStatus(Iok8skubernetespkgapiv1LoadBalancerStatus loadBalancer = default(Iok8skubernetespkgapiv1LoadBalancerStatus))
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
        /// load-balancer.
        /// </summary>
        [JsonProperty(PropertyName = "loadBalancer")]
        public Iok8skubernetespkgapiv1LoadBalancerStatus LoadBalancer { get; set; }

    }
}
