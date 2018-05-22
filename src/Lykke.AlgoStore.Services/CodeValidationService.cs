using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.Services.Validation;

namespace Lykke.AlgoStore.Services
{
    public class CodeValidationService : ICodeValidationService
    {
        public ICodeValidationSession StartSession(string code)
        {
            return new CSharpCodeValidationSession(code);
        }
    }
}
