using Lykke.AlgoStore.Core.Domain.Validation;
using Lykke.AlgoStore.Core.Validation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Services.Validation
{
    public class CSharpCodeValidationSession : ICodeValidationSession
    {
        private static readonly List<(string, string)> _allowedFxLibs = new List<(string, string)>
        {
            // Add framework libraries here
            ("System.Runtime", "4.2.0.0")
        };

        private readonly string _code;
        private readonly SourceText _sourceText;

        private ValidationResult _syntaxValidationResult;

        private SyntaxTree _syntaxTree;
        private SemanticModel _semanticModel;

        private bool _isDisposed;

        public CSharpCodeValidationSession(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));

            _code = code;
            _sourceText = SourceText.From(code);
        }

        public async Task<ValidationResult> Validate()
        {
            if (_syntaxValidationResult != null)
                return _syntaxValidationResult;

            _syntaxTree = CSharpSyntaxTree.ParseText(_code);

            var validationMessages = new List<ValidationMessage>(_syntaxTree.GetDiagnostics()
                                                                            .Select(DiagnosticToValidationMessage));

            if (ErrorExists(validationMessages))
                return CreateAndSetValidationResult(out _syntaxValidationResult, false, validationMessages);

            var root = (CompilationUnitSyntax)await _syntaxTree.GetRootAsync();

            var syntaxWalker = new CSharpAlgoValidationWalker(_sourceText);
            syntaxWalker.Visit(root);

            validationMessages.AddRange(syntaxWalker.GetMessages());

            if(ErrorExists(validationMessages))
                return CreateAndSetValidationResult(out _syntaxValidationResult, false, validationMessages);

            // Semantic validation

            var coreLib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var fxLibs = _allowedFxLibs
                .Select(l => MetadataReference.CreateFromFile(
                    System.Reflection.Assembly.Load(
                        $"{l.Item1}, Version={l.Item2}, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location))
                .ToArray();


            var compilation = CSharpCompilation.Create("CodeValidation")
                            .AddSyntaxTrees(_syntaxTree)
                            .AddReferences(coreLib)
                            .AddReferences(fxLibs)
                            .AddReferences(await NuGetReferenceProvider.GetReferences());

            _semanticModel = compilation.GetSemanticModel(_syntaxTree, false);
            var semanticDiagnostics = _semanticModel.GetDiagnostics();

            validationMessages.AddRange(semanticDiagnostics.Select(DiagnosticToValidationMessage));

            return CreateAndSetValidationResult(out _syntaxValidationResult, 
                                                !ErrorExists(validationMessages), 
                                                validationMessages);
        }

        private ValidationMessage DiagnosticToValidationMessage(Diagnostic diagnostic)
        {
            var validationMessage = new ValidationMessage();

            var position = diagnostic.Location.GetLineSpan().StartLinePosition;

            validationMessage.Line = (uint)position.Line;
            validationMessage.Column = (uint)position.Character;

            validationMessage.Id = diagnostic.Id;
            validationMessage.Message = diagnostic.Descriptor.Description.ToString();

            switch(diagnostic.Severity)
            {
                case DiagnosticSeverity.Error:
                    validationMessage.Severity = ValidationSeverity.Error;
                    break;

                case DiagnosticSeverity.Warning:
                    validationMessage.Severity = ValidationSeverity.Warning;
                    break;

                default:
                    validationMessage.Severity = ValidationSeverity.Info;
                    break;
            }

            return validationMessage;
        }

        private bool ErrorExists(IEnumerable<ValidationMessage> messages)
        {
            return messages.Any(v => v.Severity == ValidationSeverity.Error);
        }

        private ValidationResult CreateAndSetValidationResult(
            out ValidationResult validationResult,
            bool isSuccessful,
            IReadOnlyCollection<ValidationMessage> messages)
        {
            validationResult = new ValidationResult(isSuccessful, messages);

            return validationResult;
        }
    }
}
