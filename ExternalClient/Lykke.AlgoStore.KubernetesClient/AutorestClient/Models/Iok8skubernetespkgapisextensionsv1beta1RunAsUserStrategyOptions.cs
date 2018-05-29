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
    /// Run A sUser Strategy Options defines the strategy type and any options
    /// used to create the strategy.
    /// </summary>
    public partial class Iok8skubernetespkgapisextensionsv1beta1RunAsUserStrategyOptions
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisextensionsv1beta1RunAsUserStrategyOptions
        /// class.
        /// </summary>
        public Iok8skubernetespkgapisextensionsv1beta1RunAsUserStrategyOptions()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisextensionsv1beta1RunAsUserStrategyOptions
        /// class.
        /// </summary>
        /// <param name="rule">Rule is the strategy that will dictate the
        /// allowable RunAsUser values that may be set.</param>
        /// <param name="ranges">Ranges are the allowed ranges of uids that may
        /// be used.</param>
        public Iok8skubernetespkgapisextensionsv1beta1RunAsUserStrategyOptions(string rule, IList<Iok8skubernetespkgapisextensionsv1beta1IDRange> ranges = default(IList<Iok8skubernetespkgapisextensionsv1beta1IDRange>))
        {
            Ranges = ranges;
            Rule = rule;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets ranges are the allowed ranges of uids that may be
        /// used.
        /// </summary>
        [JsonProperty(PropertyName = "ranges")]
        public IList<Iok8skubernetespkgapisextensionsv1beta1IDRange> Ranges { get; set; }

        /// <summary>
        /// Gets or sets rule is the strategy that will dictate the allowable
        /// RunAsUser values that may be set.
        /// </summary>
        [JsonProperty(PropertyName = "rule")]
        public string Rule { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Rule == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Rule");
            }
            if (Ranges != null)
            {
                foreach (var element in Ranges)
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
