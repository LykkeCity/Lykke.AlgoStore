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
    /// LocalObjectReference contains enough information to let you locate the
    /// referenced object inside the same namespace.
    /// </summary>
    public partial class Iok8skubernetespkgapiv1LocalObjectReference
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1LocalObjectReference class.
        /// </summary>
        public Iok8skubernetespkgapiv1LocalObjectReference()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1LocalObjectReference class.
        /// </summary>
        /// <param name="name">Name of the referent. More info:
        /// https://kubernetes.io/docs/concepts/overview/working-with-objects/names/#names</param>
        public Iok8skubernetespkgapiv1LocalObjectReference(string name = default(string))
        {
            Name = name;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets name of the referent. More info:
        /// https://kubernetes.io/docs/concepts/overview/working-with-objects/names/#names
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

    }
}
