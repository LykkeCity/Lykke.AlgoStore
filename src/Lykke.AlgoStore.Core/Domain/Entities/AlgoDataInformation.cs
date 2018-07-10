using Lykke.AlgoStore.Core.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;
using System;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoDataInformation
    {
        public string AlgoId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime? DatePublished { get; set; }
        public AlgoVisibility AlgoVisibility { get; set; }
        public string Author { get; set; }

        public double Rating { get; set; }
        public int RatedUsersCount { get; set; }
        public int UsersCount { get; set; }

        public AlgoMetaDataInformation AlgoMetaDataInformation { get; set; }
    }
}
