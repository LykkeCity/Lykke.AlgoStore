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
    /// Node is a worker node in Kubernetes. Each node will have a unique
    /// identifier in the cache (i.e. in etcd).
    /// </summary>
    public partial class Iok8skubernetespkgapiv1Node
    {
        /// <summary>
        /// Initializes a new instance of the Iok8skubernetespkgapiv1Node
        /// class.
        /// </summary>
        public Iok8skubernetespkgapiv1Node()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Iok8skubernetespkgapiv1Node
        /// class.
        /// </summary>
        /// <param name="apiVersion">APIVersion defines the versioned schema of
        /// this representation of an object. Servers should convert recognized
        /// schemas to the latest internal value, and may reject unrecognized
        /// values. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#resources</param>
        /// <param name="kind">Kind is a string value representing the REST
        /// resource this object represents. Servers may infer this from the
        /// endpoint the client submits requests to. Cannot be updated. In
        /// CamelCase. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#types-kinds</param>
        /// <param name="metadata">Standard object's metadata. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#metadata</param>
        /// <param name="spec">Spec defines the behavior of a node.
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#spec-and-status</param>
        /// <param name="status">Most recently observed status of the node.
        /// Populated by the system. Read-only. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#spec-and-status</param>
        public Iok8skubernetespkgapiv1Node(string apiVersion = default(string), string kind = default(string), Iok8sapimachinerypkgapismetav1ObjectMeta metadata = default(Iok8sapimachinerypkgapismetav1ObjectMeta), Iok8skubernetespkgapiv1NodeSpec spec = default(Iok8skubernetespkgapiv1NodeSpec), Iok8skubernetespkgapiv1NodeStatus status = default(Iok8skubernetespkgapiv1NodeStatus))
        {
            ApiVersion = apiVersion;
            Kind = kind;
            Metadata = metadata;
            Spec = spec;
            Status = status;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets aPIVersion defines the versioned schema of this
        /// representation of an object. Servers should convert recognized
        /// schemas to the latest internal value, and may reject unrecognized
        /// values. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#resources
        /// </summary>
        [JsonProperty(PropertyName = "apiVersion")]
        public string ApiVersion { get; set; }

        /// <summary>
        /// Gets or sets kind is a string value representing the REST resource
        /// this object represents. Servers may infer this from the endpoint
        /// the client submits requests to. Cannot be updated. In CamelCase.
        /// More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#types-kinds
        /// </summary>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets standard object's metadata. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#metadata
        /// </summary>
        [JsonProperty(PropertyName = "metadata")]
        public Iok8sapimachinerypkgapismetav1ObjectMeta Metadata { get; set; }

        /// <summary>
        /// Gets or sets spec defines the behavior of a node.
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#spec-and-status
        /// </summary>
        [JsonProperty(PropertyName = "spec")]
        public Iok8skubernetespkgapiv1NodeSpec Spec { get; set; }

        /// <summary>
        /// Gets or sets most recently observed status of the node. Populated
        /// by the system. Read-only. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#spec-and-status
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public Iok8skubernetespkgapiv1NodeStatus Status { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Metadata != null)
            {
                Metadata.Validate();
            }
            if (Status != null)
            {
                Status.Validate();
            }
        }
    }
}
