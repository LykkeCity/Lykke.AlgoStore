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
    /// APIServiceStatus contains derived information about an API server
    /// </summary>
    public partial class Iok8skubeAggregatorpkgapisapiregistrationv1beta1APIServiceStatus
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubeAggregatorpkgapisapiregistrationv1beta1APIServiceStatus
        /// class.
        /// </summary>
        public Iok8skubeAggregatorpkgapisapiregistrationv1beta1APIServiceStatus()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8skubeAggregatorpkgapisapiregistrationv1beta1APIServiceStatus
        /// class.
        /// </summary>
        /// <param name="conditions">Current service state of
        /// apiService.</param>
        public Iok8skubeAggregatorpkgapisapiregistrationv1beta1APIServiceStatus(IList<Iok8skubeAggregatorpkgapisapiregistrationv1beta1APIServiceCondition> conditions = default(IList<Iok8skubeAggregatorpkgapisapiregistrationv1beta1APIServiceCondition>))
        {
            Conditions = conditions;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets current service state of apiService.
        /// </summary>
        [JsonProperty(PropertyName = "conditions")]
        public IList<Iok8skubeAggregatorpkgapisapiregistrationv1beta1APIServiceCondition> Conditions { get; set; }

    }
}
