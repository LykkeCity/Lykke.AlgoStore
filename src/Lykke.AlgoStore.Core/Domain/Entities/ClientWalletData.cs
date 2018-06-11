using Lykke.Service.ClientAccount.Client.Models;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class ClientWalletData
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public static ClientWalletData CreateFromDto(WalletDtoModel dto)
        {
            return new ClientWalletData
            {
                Id = dto.Id,
                Name = dto.Name
            };
        }
    }
}
