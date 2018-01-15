﻿using Newtonsoft.Json;

namespace Lykke.AlgoStore.TeamCityClient.Models
{
    public class Property : PropertyBase
    {
        public bool Inherited { get; set; }
        [JsonProperty(PropertyName = "type")]
        public PropertyType PropertyType { get; set; }
    }
}
