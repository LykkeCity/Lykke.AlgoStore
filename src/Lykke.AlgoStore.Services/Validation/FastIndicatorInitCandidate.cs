using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Lykke.AlgoStore.Services.Validation
{
    internal class FastIndicatorInitCandidate
    {
        public IdentifierNameSyntax MemberIdentifier { get; set; }
        public InvocationExpressionSyntax Invocation { get; set; }
    }
}
