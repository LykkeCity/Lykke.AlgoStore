using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.Services.Validation;

namespace Lykke.AlgoStore.Services
{
    public class CodeBuildService : ICodeBuildService
    {
        private readonly string AlgoNamespaceValue;

        public CodeBuildService(string algoNamespaceValue)
        {
            AlgoNamespaceValue = algoNamespaceValue;
        }

        public ICodeBuildSession StartSession(string code)
        {
            return new CSharpCodeBuildSession(code, AlgoNamespaceValue);
        }
    }
}
