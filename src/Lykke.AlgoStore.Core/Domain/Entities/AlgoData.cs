using Lykke.AlgoStore.Core.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoData : BaseValidatableData
    {
        [Required]
        public string ClientId { get; set; }

        [Required]
        public string AlgoId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public AlgoVisibility AlgoVisibility { get; set; }
        public string AlgoMetaDataInformationJSON { get; set; }
    }
}
