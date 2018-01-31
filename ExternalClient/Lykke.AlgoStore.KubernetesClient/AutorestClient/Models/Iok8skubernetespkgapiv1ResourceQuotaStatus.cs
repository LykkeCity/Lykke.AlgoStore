// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.KubernetesClient.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// ResourceQuotaStatus defines the enforced hard limits and observed use.
    /// </summary>
    public partial class Iok8skubernetespkgapiv1ResourceQuotaStatus
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1ResourceQuotaStatus class.
        /// </summary>
        public Iok8skubernetespkgapiv1ResourceQuotaStatus()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1ResourceQuotaStatus class.
        /// </summary>
        /// <param name="hard">Hard is the set of enforced hard limits for each
        /// named resource. More info:
        /// https://git.k8s.io/community/contributors/design-proposals/admission_control_resource_quota.md</param>
        /// <param name="used">Used is the current observed total usage of the
        /// resource in the namespace.</param>
        public Iok8skubernetespkgapiv1ResourceQuotaStatus(IDictionary<string, string> hard = default(IDictionary<string, string>), IDictionary<string, string> used = default(IDictionary<string, string>))
        {
            Hard = hard;
            Used = used;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets hard is the set of enforced hard limits for each named
        /// resource. More info:
        /// https://git.k8s.io/community/contributors/design-proposals/admission_control_resource_quota.md
        /// </summary>
        [JsonProperty(PropertyName = "hard")]
        public IDictionary<string, string> Hard { get; set; }

        /// <summary>
        /// Gets or sets used is the current observed total usage of the
        /// resource in the namespace.
        /// </summary>
        [JsonProperty(PropertyName = "used")]
        public IDictionary<string, string> Used { get; set; }

    }
}
