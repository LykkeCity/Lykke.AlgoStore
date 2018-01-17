using System;

namespace Lykke.AlgoStore.Core.Domain.Attributes
{
    public class DoNotUseAttribure : Attribute
    {
        public string Info { get; set; }
        public DoNotUseAttribure(string info)
        {
            Info = info;
        }
    }
}
