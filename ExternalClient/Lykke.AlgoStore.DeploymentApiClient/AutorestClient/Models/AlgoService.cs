// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.DeploymentApiClient.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class AlgoService
    {
        /// <summary>
        /// Initializes a new instance of the AlgoService class.
        /// </summary>
        public AlgoService()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the AlgoService class.
        /// </summary>
        public AlgoService(Algo algo = default(Algo), long? id = default(long?), string name = default(string), string serviceId = default(string), string status = default(string))
        {
            Algo = algo;
            Id = id;
            Name = name;
            ServiceId = serviceId;
            Status = status;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "algo")]
        public Algo Algo { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public long? Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "serviceId")]
        public string ServiceId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

    }
}
