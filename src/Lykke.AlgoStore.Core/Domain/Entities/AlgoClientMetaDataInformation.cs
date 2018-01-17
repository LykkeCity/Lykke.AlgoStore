﻿using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.AlgoMetaDataModels;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoClientMetaDataInformation
    {
        public string AlgoId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }

        public string Author { get; set; }

        public double Rating { get; set; }
        public int UsersCount { get; set; }

        public AlgoMetaDataInformation AlgoMetaDataInformation { get; set; }
    }
}