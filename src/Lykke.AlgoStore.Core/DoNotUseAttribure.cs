using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Core
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
