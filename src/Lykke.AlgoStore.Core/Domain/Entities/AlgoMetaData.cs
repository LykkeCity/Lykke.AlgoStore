﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoMetaData : BaseValidatableData, IComparable<AlgoMetaData>
    {
        [Required]
        public string AlgoId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }

        public double Rating { get; set; }
        public int UsersCount { get; set; }
        public string Author { get; set; }

        public int CompareTo(AlgoMetaData other)
        {
            if (string.IsNullOrWhiteSpace(other.Date) && string.IsNullOrWhiteSpace(Date))
                return 0;

            if (string.IsNullOrWhiteSpace(Date))
                return -1;
            if (string.IsNullOrWhiteSpace(other.Date))
                return 1;

            return String.Compare(Date, other.Date, StringComparison.Ordinal);

        }
    }
}
