using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Api.Models
{
    public class UserModel
    {
        public string ClientId { get; set; }
        public bool GDPRConsent { get; set; }
        public bool CookieConsent { get; set; }
    }
}
