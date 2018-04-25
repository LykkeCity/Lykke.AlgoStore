﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Api.Models
{
    public class UserRoleModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool CanBeModified { get; set; }
        public bool CanBeDeleted { get; set; }
        public List<UserPermissionModel> Permissions { get; set; }
    }
}
