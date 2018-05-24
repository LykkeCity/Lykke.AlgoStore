﻿using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.Services.Validation;

namespace Lykke.AlgoStore.Services
{
    public class CodeBuildService : ICodeValidationService
    {
        public ICodeBuildSession StartSession(string code)
        {
            return new CSharpCodeBuildSession(code);
        }
    }
}