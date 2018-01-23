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
    /// PodDisruptionBudget is an object to define the max disruption that can
    /// be caused to a collection of pods
    /// </summary>
    public partial class Iok8skubernetespkgapispolicyv1beta1PodDisruptionBudget
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapispolicyv1beta1PodDisruptionBudget class.
        /// </summary>
        public Iok8skubernetespkgapispolicyv1beta1PodDisruptionBudget()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapispolicyv1beta1PodDisruptionBudget class.
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
        /// <param name="spec">Specification of the desired behavior of the
        /// PodDisruptionBudget.</param>
        /// <param name="status">Most recently observed status of the
        /// PodDisruptionBudget.</param>
        public Iok8skubernetespkgapispolicyv1beta1PodDisruptionBudget(string apiVersion = default(string), string kind = default(string), Iok8sapimachinerypkgapismetav1ObjectMeta metadata = default(Iok8sapimachinerypkgapismetav1ObjectMeta), Iok8skubernetespkgapispolicyv1beta1PodDisruptionBudgetSpec spec = default(Iok8skubernetespkgapispolicyv1beta1PodDisruptionBudgetSpec), Iok8skubernetespkgapispolicyv1beta1PodDisruptionBudgetStatus status = default(Iok8skubernetespkgapispolicyv1beta1PodDisruptionBudgetStatus))
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
        /// </summary>
        [JsonProperty(PropertyName = "metadata")]
        public Iok8sapimachinerypkgapismetav1ObjectMeta Metadata { get; set; }

        /// <summary>
        /// Gets or sets specification of the desired behavior of the
        /// PodDisruptionBudget.
        /// </summary>
        [JsonProperty(PropertyName = "spec")]
        public Iok8skubernetespkgapispolicyv1beta1PodDisruptionBudgetSpec Spec { get; set; }

        /// <summary>
        /// Gets or sets most recently observed status of the
        /// PodDisruptionBudget.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public Iok8skubernetespkgapispolicyv1beta1PodDisruptionBudgetStatus Status { get; set; }

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