// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.KubernetesClient.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// EndpointPort is a tuple that describes a single port.
    /// </summary>
    public partial class Iok8skubernetespkgapiv1EndpointPort
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1EndpointPort class.
        /// </summary>
        public Iok8skubernetespkgapiv1EndpointPort()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1EndpointPort class.
        /// </summary>
        /// <param name="port">The port number of the endpoint.</param>
        /// <param name="name">The name of this port (corresponds to
        /// ServicePort.Name). Must be a DNS_LABEL. Optional only if one port
        /// is defined.</param>
        /// <param name="protocol">The IP protocol for this port. Must be UDP
        /// or TCP. Default is TCP.</param>
        public Iok8skubernetespkgapiv1EndpointPort(int port, string name = default(string), string protocol = default(string))
        {
            Name = name;
            Port = port;
            Protocol = protocol;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the name of this port (corresponds to
        /// ServicePort.Name). Must be a DNS_LABEL. Optional only if one port
        /// is defined.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the port number of the endpoint.
        /// </summary>
        [JsonProperty(PropertyName = "port")]
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the IP protocol for this port. Must be UDP or TCP.
        /// Default is TCP.
        /// </summary>
        [JsonProperty(PropertyName = "protocol")]
        public string Protocol { get; set; }

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
