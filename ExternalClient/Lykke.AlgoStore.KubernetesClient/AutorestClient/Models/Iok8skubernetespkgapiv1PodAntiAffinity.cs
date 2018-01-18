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
    /// Pod anti affinity is a group of inter pod anti affinity scheduling
    /// rules.
    /// </summary>
    public partial class Iok8skubernetespkgapiv1PodAntiAffinity
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1PodAntiAffinity class.
        /// </summary>
        public Iok8skubernetespkgapiv1PodAntiAffinity()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1PodAntiAffinity class.
        /// </summary>
        /// <param name="preferredDuringSchedulingIgnoredDuringExecution">The
        /// scheduler will prefer to schedule pods to nodes that satisfy the
        /// anti-affinity expressions specified by this field, but it may
        /// choose a node that violates one or more of the expressions. The
        /// node that is most preferred is the one with the greatest sum of
        /// weights, i.e. for each node that meets all of the scheduling
        /// requirements (resource request, requiredDuringScheduling
        /// anti-affinity expressions, etc.), compute a sum by iterating
        /// through the elements of this field and adding "weight" to the sum
        /// if the node has pods which matches the corresponding
        /// podAffinityTerm; the node(s) with the highest sum are the most
        /// preferred.</param>
        /// <param name="requiredDuringSchedulingIgnoredDuringExecution">NOT
        /// YET IMPLEMENTED. TODO: Uncomment field once it is implemented. If
        /// the anti-affinity requirements specified by this field are not met
        /// at scheduling time, the pod will not be scheduled onto the node. If
        /// the anti-affinity requirements specified by this field cease to be
        /// met at some point during pod execution (e.g. due to a pod label
        /// update), the system will try to eventually evict the pod from its
        /// node. When there are multiple elements, the lists of nodes
        /// corresponding to each podAffinityTerm are intersected, i.e. all
        /// terms must be satisfied.
        /// RequiredDuringSchedulingRequiredDuringExecution []PodAffinityTerm
        /// `json:"requiredDuringSchedulingRequiredDuringExecution,omitempty"`
        /// If the anti-affinity requirements specified by this field are not
        /// met at scheduling time, the pod will not be scheduled onto the
        /// node. If the anti-affinity requirements specified by this field
        /// cease to be met at some point during pod execution (e.g. due to a
        /// pod label update), the system may or may not try to eventually
        /// evict the pod from its node. When there are multiple elements, the
        /// lists of nodes corresponding to each podAffinityTerm are
        /// intersected, i.e. all terms must be satisfied.</param>
        public Iok8skubernetespkgapiv1PodAntiAffinity(IList<Iok8skubernetespkgapiv1WeightedPodAffinityTerm> preferredDuringSchedulingIgnoredDuringExecution = default(IList<Iok8skubernetespkgapiv1WeightedPodAffinityTerm>), IList<Iok8skubernetespkgapiv1PodAffinityTerm> requiredDuringSchedulingIgnoredDuringExecution = default(IList<Iok8skubernetespkgapiv1PodAffinityTerm>))
        {
            PreferredDuringSchedulingIgnoredDuringExecution = preferredDuringSchedulingIgnoredDuringExecution;
            RequiredDuringSchedulingIgnoredDuringExecution = requiredDuringSchedulingIgnoredDuringExecution;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the scheduler will prefer to schedule pods to nodes
        /// that satisfy the anti-affinity expressions specified by this field,
        /// but it may choose a node that violates one or more of the
        /// expressions. The node that is most preferred is the one with the
        /// greatest sum of weights, i.e. for each node that meets all of the
        /// scheduling requirements (resource request, requiredDuringScheduling
        /// anti-affinity expressions, etc.), compute a sum by iterating
        /// through the elements of this field and adding "weight" to the sum
        /// if the node has pods which matches the corresponding
        /// podAffinityTerm; the node(s) with the highest sum are the most
        /// preferred.
        /// </summary>
        [JsonProperty(PropertyName = "preferredDuringSchedulingIgnoredDuringExecution")]
        public IList<Iok8skubernetespkgapiv1WeightedPodAffinityTerm> PreferredDuringSchedulingIgnoredDuringExecution { get; set; }

        /// <summary>
        /// Gets or sets NOT YET IMPLEMENTED. TODO: Uncomment field once it is
        /// implemented. If the anti-affinity requirements specified by this
        /// field are not met at scheduling time, the pod will not be scheduled
        /// onto the node. If the anti-affinity requirements specified by this
        /// field cease to be met at some point during pod execution (e.g. due
        /// to a pod label update), the system will try to eventually evict the
        /// pod from its node. When there are multiple elements, the lists of
        /// nodes corresponding to each podAffinityTerm are intersected, i.e.
        /// all terms must be satisfied.
        /// RequiredDuringSchedulingRequiredDuringExecution []PodAffinityTerm
        /// `json:"requiredDuringSchedulingRequiredDuringExecution,omitempty"`
        /// If the anti-affinity requirements specified by this field are not
        /// met at scheduling time, the pod will not be scheduled onto the
        /// node. If the anti-affinity requirements specified by this field
        /// cease to be met at some point during pod execution (e.g. due to a
        /// pod label update), the system may or may not try to eventually
        /// evict the pod from its node. When there are multiple elements, the
        /// lists of nodes corresponding to each podAffinityTerm are
        /// intersected, i.e. all terms must be satisfied.
        /// </summary>
        [JsonProperty(PropertyName = "requiredDuringSchedulingIgnoredDuringExecution")]
        public IList<Iok8skubernetespkgapiv1PodAffinityTerm> RequiredDuringSchedulingIgnoredDuringExecution { get; set; }

    }
}
