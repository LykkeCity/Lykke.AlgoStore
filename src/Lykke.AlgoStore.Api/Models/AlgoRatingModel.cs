using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoRatingModel
    {
        public string AlgoId { get; set; }
        public string ClientId { get; set; }
        public int UsersCount { get; set; }
        public double Rating { get; set; }
    }
}
