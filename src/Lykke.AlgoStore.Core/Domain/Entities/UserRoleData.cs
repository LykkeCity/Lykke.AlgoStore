using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class UserRoleData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<UserPermissionData> Permissions { get; set; }
    }
}
