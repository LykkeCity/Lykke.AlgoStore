// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.AlgoStore.DeploymentApiClient.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class Algo
    {
        /// <summary>
        /// Initializes a new instance of the Algo class.
        /// </summary>
        public Algo()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Algo class.
        /// </summary>
        public Algo(string algoBuildImageId = default(string), AlgoService algoService = default(AlgoService), AlgoTest algoTest = default(AlgoTest), AlgoUser algoUser = default(AlgoUser), long? id = default(long?), string name = default(string), string repo = default(string), long? version = default(long?))
        {
            AlgoBuildImageId = algoBuildImageId;
            AlgoService = algoService;
            AlgoTest = algoTest;
            AlgoUser = algoUser;
            Id = id;
            Name = name;
            Repo = repo;
            Version = version;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "algoBuildImageId")]
        public string AlgoBuildImageId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "algoService")]
        public AlgoService AlgoService { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "algoTest")]
        public AlgoTest AlgoTest { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "algoUser")]
        public AlgoUser AlgoUser { get; set; }

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
        [JsonProperty(PropertyName = "repo")]
        public string Repo { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public long? Version { get; set; }

    }
}