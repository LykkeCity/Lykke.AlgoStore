using Lykke.AlgoStore.Core.Enumerators;
using System;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public interface IAlgo
    {
        string ClientId { get; set; }
        string AlgoId { get; set ;}
        string Name { get; set; }

        string Description { get; set; }
        DateTime DateModified { get; }
        DateTime DateCreated { get; set; }
        AlgoVisibility AlgoVisibility { get; set; }
        string AlgoMetaDataInformationJSON { get; set; }
    }
}
