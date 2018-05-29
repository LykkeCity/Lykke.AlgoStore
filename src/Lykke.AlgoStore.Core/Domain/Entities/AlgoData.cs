using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Lykke.AlgoStore.Core.Enumerators;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoData : BaseValidatableData
    {
        public string Author { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string AlgoId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        public string Date { get; set; }
        public AlgoVisibility AlgoVisibility { get; set; }
        public string AlgoMetaDataInformationJSON { get; set; }
    }
}
