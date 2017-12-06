using System;
using System.Diagnostics;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Lykke.AlgoStore.Tests.Infrastructure
{
    public class RunnableInDebugOnlyAttribute : Attribute, ITestAction
    {
        private string _message;

        public RunnableInDebugOnlyAttribute(string message)
        {
            _message = message;
        }

        public void BeforeTest(ITest test)
        {
            if (!Debugger.IsAttached)
                Assert.Ignore(_message);
        }

        public void AfterTest(ITest test)
        {

        }

        public ActionTargets Targets { get; }
    }
}
