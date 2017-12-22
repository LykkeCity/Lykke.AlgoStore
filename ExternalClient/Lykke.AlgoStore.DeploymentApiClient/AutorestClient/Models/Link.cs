// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.DeploymentApiClient.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class Link
    {
        /// <summary>
        /// Initializes a new instance of the Link class.
        /// </summary>
        public Link()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Link class.
        /// </summary>
        public Link(string href = default(string), string rel = default(string), bool? templated = default(bool?))
        {
            Href = href;
            Rel = rel;
            Templated = templated;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "href")]
        public string Href { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "rel")]
        public string Rel { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "templated")]
        public bool? Templated { get; set; }

    }
}