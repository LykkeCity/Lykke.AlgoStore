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
    /// HTTPIngressRuleValue is a list of http selectors pointing to backends.
    /// In the example: http://&lt;host&gt;/&lt;path&gt;?&lt;searchpart&gt;
    /// -&gt; backend where where parts of the url correspond to RFC 3986, this
    /// resource will be used to match against everything after the last '/'
    /// and before the first '?' or '#'.
    /// </summary>
    public partial class Iok8skubernetespkgapisextensionsv1beta1HTTPIngressRuleValue
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisextensionsv1beta1HTTPIngressRuleValue class.
        /// </summary>
        public Iok8skubernetespkgapisextensionsv1beta1HTTPIngressRuleValue()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubernetespkgapisextensionsv1beta1HTTPIngressRuleValue class.
        /// </summary>
        /// <param name="paths">A collection of paths that map requests to
        /// backends.</param>
        public Iok8skubernetespkgapisextensionsv1beta1HTTPIngressRuleValue(IList<Iok8skubernetespkgapisextensionsv1beta1HTTPIngressPath> paths)
        {
            Paths = paths;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets a collection of paths that map requests to backends.
        /// </summary>
        [JsonProperty(PropertyName = "paths")]
        public IList<Iok8skubernetespkgapisextensionsv1beta1HTTPIngressPath> Paths { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Paths == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Paths");
            }
            if (Paths != null)
            {
                foreach (var element in Paths)
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
