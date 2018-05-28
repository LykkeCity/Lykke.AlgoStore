// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.KubernetesClient.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// NetworkPolicySpec provides the specification of a NetworkPolicy
    /// </summary>
    public partial class Iok8skubernetespkgapisnetworkingv1NetworkPolicySpec
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisnetworkingv1NetworkPolicySpec class.
        /// </summary>
        public Iok8skubernetespkgapisnetworkingv1NetworkPolicySpec()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisnetworkingv1NetworkPolicySpec class.
        /// </summary>
        /// <param name="podSelector">Selects the pods to which this
        /// NetworkPolicy object applies. The array of ingress rules is applied
        /// to any pods selected by this field. Multiple network policies can
        /// select the same set of pods. In this case, the ingress rules for
        /// each are combined additively. This field is NOT optional and
        /// follows standard label selector semantics. An empty podSelector
        /// matches all pods in this namespace.</param>
        /// <param name="ingress">List of ingress rules to be applied to the
        /// selected pods. Traffic is allowed to a pod if there are no
        /// NetworkPolicies selecting the pod (and cluster policy otherwise
        /// allows the traffic), OR if the traffic source is the pod's local
        /// node, OR if the traffic matches at least one ingress rule across
        /// all of the NetworkPolicy objects whose podSelector matches the pod.
        /// If this field is empty then this NetworkPolicy does not allow any
        /// traffic (and serves solely to ensure that the pods it selects are
        /// isolated by default)</param>
        public Iok8skubernetespkgapisnetworkingv1NetworkPolicySpec(Iok8sapimachinerypkgapismetav1LabelSelector podSelector, IList<Iok8skubernetespkgapisnetworkingv1NetworkPolicyIngressRule> ingress = default(IList<Iok8skubernetespkgapisnetworkingv1NetworkPolicyIngressRule>))
        {
            Ingress = ingress;
            PodSelector = podSelector;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets list of ingress rules to be applied to the selected
        /// pods. Traffic is allowed to a pod if there are no NetworkPolicies
        /// selecting the pod (and cluster policy otherwise allows the
        /// traffic), OR if the traffic source is the pod's local node, OR if
        /// the traffic matches at least one ingress rule across all of the
        /// NetworkPolicy objects whose podSelector matches the pod. If this
        /// field is empty then this NetworkPolicy does not allow any traffic
        /// (and serves solely to ensure that the pods it selects are isolated
        /// by default)
        /// </summary>
        [JsonProperty(PropertyName = "ingress")]
        public IList<Iok8skubernetespkgapisnetworkingv1NetworkPolicyIngressRule> Ingress { get; set; }

        /// <summary>
        /// Gets or sets selects the pods to which this NetworkPolicy object
        /// applies. The array of ingress rules is applied to any pods selected
        /// by this field. Multiple network policies can select the same set of
        /// pods. In this case, the ingress rules for each are combined
        /// additively. This field is NOT optional and follows standard label
        /// selector semantics. An empty podSelector matches all pods in this
        /// namespace.
        /// </summary>
        [JsonProperty(PropertyName = "podSelector")]
        public Iok8sapimachinerypkgapismetav1LabelSelector PodSelector { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (PodSelector == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "PodSelector");
            }
        }
    }
}
