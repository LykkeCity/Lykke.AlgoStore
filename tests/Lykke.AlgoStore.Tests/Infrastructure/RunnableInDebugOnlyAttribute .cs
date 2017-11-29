using System.Diagnostics;
using Xunit;

namespace Lykke.AlgoStore.Tests.Infrastructure
{
    public class RunnableInDebugOnlyAttribute : FactAttribute
    {
        public RunnableInDebugOnlyAttribute(string description)
        {
            if (!Debugger.IsAttached)
            {
                Skip = description;
            }
        }
    }
}
