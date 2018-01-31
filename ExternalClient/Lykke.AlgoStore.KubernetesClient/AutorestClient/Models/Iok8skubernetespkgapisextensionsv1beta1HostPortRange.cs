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
    /// Host Port Range defines a range of host ports that will be enabled by a
    /// policy for pods to use.  It requires both the start and end to be
    /// defined.
    /// </summary>
    public partial class Iok8skubernetespkgapisextensionsv1beta1HostPortRange
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisextensionsv1beta1HostPortRange class.
        /// </summary>
        public Iok8skubernetespkgapisextensionsv1beta1HostPortRange()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisextensionsv1beta1HostPortRange class.
        /// </summary>
        /// <param name="max">max is the end of the range, inclusive.</param>
        /// <param name="min">min is the start of the range, inclusive.</param>
        public Iok8skubernetespkgapisextensionsv1beta1HostPortRange(int max, int min)
        {
            Max = max;
            Min = min;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets max is the end of the range, inclusive.
        /// </summary>
        [JsonProperty(PropertyName = "max")]
        public int Max { get; set; }

        /// <summary>
        /// Gets or sets min is the start of the range, inclusive.
        /// </summary>
        [JsonProperty(PropertyName = "min")]
        public int Min { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            //Nothing to validate
        }
    }
}
