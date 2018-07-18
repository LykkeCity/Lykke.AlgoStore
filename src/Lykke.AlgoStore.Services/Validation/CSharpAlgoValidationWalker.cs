using Lykke.AlgoStore.Core.Domain.Validation;
using Lykke.AlgoStore.Services.Strings;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.Services.Validation
{
    internal class CSharpAlgoValidationWalker : CSharpSyntaxWalker
    {
        private const string BASE_ALGO_NAME = "BaseAlgo";
        private const string CANDLE_RECEIVED_METHOD = "OnCandleReceived";
        private const string QUOTE_RECEIVED_METHOD = "OnQuoteReceived";
        private const string STARTUP_METHOD = "OnStartUp";

        private readonly SourceText _sourceText;
        private readonly string _algoNamespaceValue;

        private bool _foundAlgoClass;
        private bool _foundEventMethod;
        private bool _foundNamespace;

        private List<ValidationMessage> _validationMessages = new List<ValidationMessage>();

        public ClassDeclarationSyntax ClassNode { get; private set; }
        public FastIndicatorInitCandidate[] IndicatorInitializations { get; set; } 
            = new FastIndicatorInitCandidate[0];

        public NamespaceDeclarationSyntax NamespaceNode
        {
            get
            {
                if(ClassNode.Parent is NamespaceDeclarationSyntax)
                    return (NamespaceDeclarationSyntax)ClassNode.Parent;

                return null;
            }
        }

        public CSharpAlgoValidationWalker(SourceText sourceText, string AlgoNamespaceValue)
        {
            _sourceText = sourceText ?? throw new ArgumentNullException(nameof(sourceText));
            _algoNamespaceValue = AlgoNamespaceValue;
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            ValidateNamespaceName(node);            
            _foundNamespace = true;                            

            base.VisitNamespaceDeclaration(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            ValidateTypeName(node);

            // If class has no base types - continue
            if (node.BaseList == null) return;

            // If class doesn't inherit the base algo - continue
            if (!HasBaseType(node, BASE_ALGO_NAME)) return;

            // More than one class inheriting BaseAlgo is not allowed
            if (_foundAlgoClass)
            {
                AddValidationMessage(
                    ValidationErrors.ERROR_BASEALGO_MULTIPLE_INHERITANCE, 
                    Phrases.ERROR_BASEALGO_MULTIPLE_INHERITANCE,
                    position: node.SpanStart);
            }

            // Class inheriting base algo must be sealed
            if (!node.Modifiers.Any(m => m.Text == "sealed"))
            {
                AddValidationMessage(ValidationErrors.ERROR_ALGO_NOT_SEALED, Phrases.ERROR_ALGO_NOT_SEALED,
                    position: node.SpanStart);
            }

            foreach (var method in node.Members.OfType<MethodDeclarationSyntax>())
                ValidateMethod(method);

            _foundAlgoClass = true;
            ClassNode = node;
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            ValidateTypeName(node);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            ValidateTypeName(node);
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            ValidateTypeName(node);
        }

        public IEnumerable<ValidationMessage> GetMessages()
        {
            if (!_foundAlgoClass)
                AddValidationMessage(ValidationErrors.ERROR_BASEALGO_NOT_INHERITED, 
                                     Phrases.ERROR_BASEALGO_NOT_INHERITED);

            if (_foundAlgoClass && !_foundEventMethod)
                AddValidationMessage(ValidationErrors.ERROR_EVENT_NOT_IMPLEMENTED,
                    string.Format(Phrases.ERROR_EVENT_NOT_IMPLEMENTED, CANDLE_RECEIVED_METHOD, QUOTE_RECEIVED_METHOD));

            if (!_foundNamespace)
                AddValidationMessage(ValidationErrors.ERROR_NAMESPACE_NOT_FOUND, Phrases.ERROR_NAMESPACE_NOT_CORRECT);

            return _validationMessages;
        }

        private void ValidateTypeName(BaseTypeDeclarationSyntax typeDecl)
        {
            if (typeDecl.Identifier.Text == BASE_ALGO_NAME)
            {
                AddValidationMessage(ValidationErrors.ERROR_TYPE_NAMED_BASEALGO, Phrases.ERROR_TYPE_NAMED_BASEALGO,
                    position: typeDecl.SpanStart);
            }
        }

        private void ValidateNamespaceName(NamespaceDeclarationSyntax namespaceDecl)
        {
            if (namespaceDecl.Name.ToString() != _algoNamespaceValue)
            {
                AddValidationMessage(ValidationErrors.ERROR_NAMESPACE_NOT_CORRECT, Phrases.ERROR_NAMESPACE_NOT_CORRECT,
                    position: namespaceDecl.SpanStart); 
            }          
        }

        private void ValidateMethod(MethodDeclarationSyntax method)
        {
            if (method.Identifier.Text == STARTUP_METHOD)
                ExtractFastInitializers(method.Body.Statements);
            
            if (method.Identifier.Text != CANDLE_RECEIVED_METHOD &&
                method.Identifier.Text != QUOTE_RECEIVED_METHOD)
                return;

            if (!method.Modifiers.Any(m => m.Text == "override"))
                return;

            _foundEventMethod = true;
        }

        private bool HasBaseType(ClassDeclarationSyntax classDecl, string baseTypeName)
        {
            return classDecl.BaseList != null && classDecl.BaseList.Types
                       .Any(x =>
                           // Example: Base type like "Test"
                               x.Type is IdentifierNameSyntax &&
                               ((IdentifierNameSyntax) x.Type).Identifier.Text == baseTypeName
                               // Example: Qualified base type like "Namespace.Test"
                               || x.Type is QualifiedNameSyntax &&
                               ((QualifiedNameSyntax) x.Type).Right.Identifier.Text == baseTypeName
                       );
        }

        private void ExtractFastInitializers(IEnumerable<StatementSyntax> statements)
        {
            var candidateList = new List<FastIndicatorInitCandidate>();

            foreach(var stmt in statements)
            {
                // Looking only for some type of expression like a = b or func();
                var expressionStmt = stmt as ExpressionStatementSyntax;
                if (expressionStmt == null) continue;

                // Look for only member assignment expression
                var assignmentExpression = expressionStmt.Expression as AssignmentExpressionSyntax;
                if (assignmentExpression == null) continue;

                // Look only for equals assignment
                if (assignmentExpression.OperatorToken.Kind() != SyntaxKind.EqualsToken) continue;

                // Look only for assignments where the left side is some variable like
                // MyProperty = something;
                var targetVariable = assignmentExpression.Left as IdentifierNameSyntax;
                if (targetVariable == null) continue;

                // Look only for assignments where the right side is an invocation like
                // MyProperty = MyFunction(...);
                var methodInvocation = assignmentExpression.Right as InvocationExpressionSyntax;
                if (methodInvocation == null) continue;

                // This will exclude cases where you can call a function like myClass.MyFunction();
                var methodIdentifier = methodInvocation.Expression as IdentifierNameSyntax;
                if (methodIdentifier == null) continue;

                var fastInitCandidate = new FastIndicatorInitCandidate();

                fastInitCandidate.Invocation = methodInvocation;
                fastInitCandidate.MemberIdentifier = targetVariable;

                candidateList.Add(fastInitCandidate);
            }

            IndicatorInitializations = candidateList.ToArray();
        }

        private void AddValidationMessage(
            string id,
            string message,
            ValidationSeverity severity = ValidationSeverity.Error,
            int position = -1)
        {
            var validationMessage = new ValidationMessage
            {
                Id = id,
                Message = message,
                Severity = severity
            };

            if (position != -1)
            {
                var line = _sourceText.Lines.GetLineFromPosition(position);

                validationMessage.Line = (uint) line.LineNumber + 1;
                validationMessage.Column = (uint) (position - line.Start + 1);
            }

            _validationMessages.Add(validationMessage);
        }
    }
}
