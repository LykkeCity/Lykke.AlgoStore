using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Newtonsoft.Json;
using System;

namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoDataModel
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(DefaultDateTimeConverter))]
        public DateTime DateCreated { get; set; }
        [JsonConverter(typeof(DefaultDateTimeConverter))]
        public DateTime DateModified { get; set; }
        [JsonConverter(typeof(DefaultDateTimeConverter))]
        public DateTime? DatePublished { get; set; }
        public AlgoVisibility AlgoVisibility { get; set; }
    }
}
