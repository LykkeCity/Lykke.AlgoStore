﻿// Code generated by Microsoft (R) AutoRest Code Generator 1.2.2.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Lykke.AlgoStore.Client.AutorestClient.Models
{
    using Lykke.AlgoStore.Client;
    using Lykke.AlgoStore.Client.AutorestClient;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class IsAliveResponse
    {
        /// <summary>
        /// Initializes a new instance of the IsAliveResponse class.
        /// </summary>
        public IsAliveResponse()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the IsAliveResponse class.
        /// </summary>
        public IsAliveResponse(string version = default(string), string env = default(string), IList<IssueIndicator> issueIndicators = default(IList<IssueIndicator>))
        {
            Version = version;
            Env = env;
            IssueIndicators = issueIndicators;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Version")]
        public string Version { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Env")]
        public string Env { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "IssueIndicators")]
        public IList<IssueIndicator> IssueIndicators { get; set; }

    }
}
