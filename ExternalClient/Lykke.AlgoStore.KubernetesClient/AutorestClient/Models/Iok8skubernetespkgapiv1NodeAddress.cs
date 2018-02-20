// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.KubernetesClient.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// NodeAddress contains information for the node's address.
    /// </summary>
    public partial class Iok8skubernetespkgapiv1NodeAddress
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1NodeAddress class.
        /// </summary>
        public Iok8skubernetespkgapiv1NodeAddress()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1NodeAddress class.
        /// </summary>
        /// <param name="address">The node address.</param>
        /// <param name="type">Node address type, one of Hostname, ExternalIP
        /// or InternalIP.</param>
        public Iok8skubernetespkgapiv1NodeAddress(string address, string type)
        {
            Address = address;
            Type = type;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the node address.
        /// </summary>
        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets node address type, one of Hostname, ExternalIP or
        /// InternalIP.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Address == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Address");
            }
            if (Type == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Type");
            }
        }
    }
}
