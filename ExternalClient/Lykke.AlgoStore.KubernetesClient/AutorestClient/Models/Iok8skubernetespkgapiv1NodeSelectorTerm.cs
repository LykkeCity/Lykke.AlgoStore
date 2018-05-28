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
    /// A null or empty node selector term matches no objects.
    /// </summary>
    public partial class Iok8skubernetespkgapiv1NodeSelectorTerm
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1NodeSelectorTerm class.
        /// </summary>
        public Iok8skubernetespkgapiv1NodeSelectorTerm()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapiv1NodeSelectorTerm class.
        /// </summary>
        /// <param name="matchExpressions">Required. A list of node selector
        /// requirements. The requirements are ANDed.</param>
        public Iok8skubernetespkgapiv1NodeSelectorTerm(IList<Iok8skubernetespkgapiv1NodeSelectorRequirement> matchExpressions)
        {
            MatchExpressions = matchExpressions;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets required. A list of node selector requirements. The
        /// requirements are ANDed.
        /// </summary>
        [JsonProperty(PropertyName = "matchExpressions")]
        public IList<Iok8skubernetespkgapiv1NodeSelectorRequirement> MatchExpressions { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (MatchExpressions == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "MatchExpressions");
            }
            if (MatchExpressions != null)
            {
                foreach (var element in MatchExpressions)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
        }
    }
}
