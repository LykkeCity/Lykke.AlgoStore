using Microsoft.CodeAnalysis;
using System.Linq;

namespace Lykke.AlgoStore.Services.Validation
{
    internal static class RoslynExtensions
    {
        public static SyntaxNode GetSyntaxNode(this ISymbol type)
        {
            return type?.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
        }
    }
}
