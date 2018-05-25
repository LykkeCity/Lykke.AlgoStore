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
    /// Node affinity is a group of node affinity scheduling rules.
    /// </summary>
    public partial class Iok8skubernetespkgapiv1NodeAffinity
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1NodeAffinity class.
        /// </summary>
        public Iok8skubernetespkgapiv1NodeAffinity()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1NodeAffinity class.
        /// </summary>
        /// <param name="preferredDuringSchedulingIgnoredDuringExecution">The
        /// scheduler will prefer to schedule pods to nodes that satisfy the
        /// affinity expressions specified by this field, but it may choose a
        /// node that violates one or more of the expressions. The node that is
        /// most preferred is the one with the greatest sum of weights, i.e.
        /// for each node that meets all of the scheduling requirements
        /// (resource request, requiredDuringScheduling affinity expressions,
        /// etc.), compute a sum by iterating through the elements of this
        /// field and adding "weight" to the sum if the node matches the
        /// corresponding matchExpressions; the node(s) with the highest sum
        /// are the most preferred.</param>
        /// <param name="requiredDuringSchedulingIgnoredDuringExecution">If the
        /// affinity requirements specified by this field are not met at
        /// scheduling time, the pod will not be scheduled onto the node. If
        /// the affinity requirements specified by this field cease to be met
        /// at some point during pod execution (e.g. due to an update), the
        /// system may or may not try to eventually evict the pod from its
        /// node.</param>
        public Iok8skubernetespkgapiv1NodeAffinity(IList<Iok8skubernetespkgapiv1PreferredSchedulingTerm> preferredDuringSchedulingIgnoredDuringExecution = default(IList<Iok8skubernetespkgapiv1PreferredSchedulingTerm>), Iok8skubernetespkgapiv1NodeSelector requiredDuringSchedulingIgnoredDuringExecution = default(Iok8skubernetespkgapiv1NodeSelector))
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
        /// that satisfy the affinity expressions specified by this field, but
        /// it may choose a node that violates one or more of the expressions.
        /// The node that is most preferred is the one with the greatest sum of
        /// weights, i.e. for each node that meets all of the scheduling
        /// requirements (resource request, requiredDuringScheduling affinity
        /// expressions, etc.), compute a sum by iterating through the elements
        /// of this field and adding "weight" to the sum if the node matches
        /// the corresponding matchExpressions; the node(s) with the highest
        /// sum are the most preferred.
        /// </summary>
        [JsonProperty(PropertyName = "preferredDuringSchedulingIgnoredDuringExecution")]
        public IList<Iok8skubernetespkgapiv1PreferredSchedulingTerm> PreferredDuringSchedulingIgnoredDuringExecution { get; set; }

        /// <summary>
        /// Gets or sets if the affinity requirements specified by this field
        /// are not met at scheduling time, the pod will not be scheduled onto
        /// the node. If the affinity requirements specified by this field
        /// cease to be met at some point during pod execution (e.g. due to an
        /// update), the system may or may not try to eventually evict the pod
        /// from its node.
        /// </summary>
        [JsonProperty(PropertyName = "requiredDuringSchedulingIgnoredDuringExecution")]
        public Iok8skubernetespkgapiv1NodeSelector RequiredDuringSchedulingIgnoredDuringExecution { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (PreferredDuringSchedulingIgnoredDuringExecution != null)
            {
                foreach (var element in PreferredDuringSchedulingIgnoredDuringExecution)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
            if (RequiredDuringSchedulingIgnoredDuringExecution != null)
            {
                RequiredDuringSchedulingIgnoredDuringExecution.Validate();
            }
        }
    }
}
