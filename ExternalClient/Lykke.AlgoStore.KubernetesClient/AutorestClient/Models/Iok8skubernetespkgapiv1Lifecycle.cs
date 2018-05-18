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
    /// Lifecycle describes actions that the management system should take in
    /// response to container lifecycle events. For the PostStart and PreStop
    /// lifecycle handlers, management of the container blocks until the action
    /// is complete, unless the container process fails, in which case the
    /// handler is aborted.
    /// </summary>
    public partial class Iok8skubernetespkgapiv1Lifecycle
    {
        /// <summary>
        /// Initializes a new instance of the Iok8skubernetespkgapiv1Lifecycle
        /// class.
        /// </summary>
        public Iok8skubernetespkgapiv1Lifecycle()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Iok8skubernetespkgapiv1Lifecycle
        /// class.
        /// </summary>
        /// <param name="postStart">PostStart is called immediately after a
        /// container is created. If the handler fails, the container is
        /// terminated and restarted according to its restart policy. Other
        /// management of the container blocks until the hook completes. More
        /// info:
        /// https://kubernetes.io/docs/concepts/containers/container-lifecycle-hooks/#container-hooks</param>
        /// <param name="preStop">PreStop is called immediately before a
        /// container is terminated. The container is terminated after the
        /// handler completes. The reason for termination is passed to the
        /// handler. Regardless of the outcome of the handler, the container is
        /// eventually terminated. Other management of the container blocks
        /// until the hook completes. More info:
        /// https://kubernetes.io/docs/concepts/containers/container-lifecycle-hooks/#container-hooks</param>
        public Iok8skubernetespkgapiv1Lifecycle(Iok8skubernetespkgapiv1Handler postStart = default(Iok8skubernetespkgapiv1Handler), Iok8skubernetespkgapiv1Handler preStop = default(Iok8skubernetespkgapiv1Handler))
        {
            PostStart = postStart;
            PreStop = preStop;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets postStart is called immediately after a container is
        /// created. If the handler fails, the container is terminated and
        /// restarted according to its restart policy. Other management of the
        /// container blocks until the hook completes. More info:
        /// https://kubernetes.io/docs/concepts/containers/container-lifecycle-hooks/#container-hooks
        /// </summary>
        [JsonProperty(PropertyName = "postStart")]
        public Iok8skubernetespkgapiv1Handler PostStart { get; set; }

        /// <summary>
        /// Gets or sets preStop is called immediately before a container is
        /// terminated. The container is terminated after the handler
        /// completes. The reason for termination is passed to the handler.
        /// Regardless of the outcome of the handler, the container is
        /// eventually terminated. Other management of the container blocks
        /// until the hook completes. More info:
        /// https://kubernetes.io/docs/concepts/containers/container-lifecycle-hooks/#container-hooks
        /// </summary>
        [JsonProperty(PropertyName = "preStop")]
        public Iok8skubernetespkgapiv1Handler PreStop { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (PostStart != null)
            {
                PostStart.Validate();
            }
            if (PreStop != null)
            {
                PreStop.Validate();
            }
        }
    }
}
