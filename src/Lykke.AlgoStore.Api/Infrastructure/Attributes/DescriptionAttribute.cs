using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Api.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class DescriptionAttribute : Attribute
    {
        public string Description { get; }

        public DescriptionAttribute(string description)
        {
            Description = description;
        }       
    }
}
