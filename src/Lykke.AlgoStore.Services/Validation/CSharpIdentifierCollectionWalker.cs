using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Lykke.AlgoStore.Services.Validation
{
    internal class CSharpIdentifierCollectionWalker : CSharpSyntaxWalker
    {
        public List<UsingDirectiveSyntax> Usings { get; } = new List<UsingDirectiveSyntax>();
        public List<InvocationExpressionSyntax> Invocations { get; } = new List<InvocationExpressionSyntax>();
        public List<VariableDeclaratorSyntax> Declarators { get; } = new List<VariableDeclaratorSyntax>();
        public List<TypeDeclarationSyntax> TypeDeclarations { get; } = new List<TypeDeclarationSyntax>();

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            Invocations.Add(node);
            base.VisitInvocationExpression(node);
        }

        public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            Declarators.Add(node);
            base.VisitVariableDeclarator(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            TypeDeclarations.Add(node);
            base.VisitClassDeclaration(node);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            TypeDeclarations.Add(node);
            base.VisitStructDeclaration(node);
        }

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            Usings.Add(node);
            base.VisitUsingDirective(node);
        }
    }
}
