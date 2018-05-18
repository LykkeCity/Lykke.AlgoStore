using Lykke.Service.PersonalData.Contract.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoStoreUserData
    {
        public string ClientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public List<UserRoleData> Roles { get; set; }
    }
}
