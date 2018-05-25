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
    /// IngressTLS describes the transport layer security associated with an
    /// Ingress.
    /// </summary>
    public partial class Iok8skubernetespkgapisextensionsv1beta1IngressTLS
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisextensionsv1beta1IngressTLS class.
        /// </summary>
        public Iok8skubernetespkgapisextensionsv1beta1IngressTLS()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisextensionsv1beta1IngressTLS class.
        /// </summary>
        /// <param name="hosts">Hosts are a list of hosts included in the TLS
        /// certificate. The values in this list must match the name/s used in
        /// the tlsSecret. Defaults to the wildcard host setting for the
        /// loadbalancer controller fulfilling this Ingress, if left
        /// unspecified.</param>
        /// <param name="secretName">SecretName is the name of the secret used
        /// to terminate SSL traffic on 443. Field is left optional to allow
        /// SSL routing based on SNI hostname alone. If the SNI host in a
        /// listener conflicts with the "Host" header field used by an
        /// IngressRule, the SNI host is used for termination and value of the
        /// Host header is used for routing.</param>
        public Iok8skubernetespkgapisextensionsv1beta1IngressTLS(IList<string> hosts = default(IList<string>), string secretName = default(string))
        {
            Hosts = hosts;
            SecretName = secretName;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets hosts are a list of hosts included in the TLS
        /// certificate. The values in this list must match the name/s used in
        /// the tlsSecret. Defaults to the wildcard host setting for the
        /// loadbalancer controller fulfilling this Ingress, if left
        /// unspecified.
        /// </summary>
        [JsonProperty(PropertyName = "hosts")]
        public IList<string> Hosts { get; set; }

        /// <summary>
        /// Gets or sets secretName is the name of the secret used to terminate
        /// SSL traffic on 443. Field is left optional to allow SSL routing
        /// based on SNI hostname alone. If the SNI host in a listener
        /// conflicts with the "Host" header field used by an IngressRule, the
        /// SNI host is used for termination and value of the Host header is
        /// used for routing.
        /// </summary>
        [JsonProperty(PropertyName = "secretName")]
        public string SecretName { get; set; }

    }
}
