using System.Collections.Generic;

namespace Lykke.AlgoStore.Api.Models
{
    public class UserRoleDataModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool CanBeDeleted { get; set; }
        public bool CanBeModified { get; set; }
        public List<UserPermissionDataModel> Permissions { get; set; }
    }
}
