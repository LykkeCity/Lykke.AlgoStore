using System.Collections.Generic;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class UserRoleData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool CanBeDeleted { get; set; }
        public bool CanBeModified { get; set; }
        public List<UserPermissionData> Permissions { get; set; }
    }
}
