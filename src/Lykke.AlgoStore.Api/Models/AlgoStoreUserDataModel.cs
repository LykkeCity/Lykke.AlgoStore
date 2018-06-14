using System.Collections.Generic;

namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoStoreUserDataModel
    {
        public string ClientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public List<UserRoleDataModel> Roles { get; set; }
    }
}
