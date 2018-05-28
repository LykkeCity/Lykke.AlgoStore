using Lykke.AlgoStore.Core.Enumerators;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public interface IAlgo
    {
        string ClientId { get; set; }
        string AlgoId { get; set ;}
        string Name { get; set; }

        string Description { get; set; }
        string Date { get; }
        string Status { get; set; }
        AlgoVisibility AlgoVisibility { get; set; }
        string AlgoMetaDataInformationJSON { get; set; }
    }
}
