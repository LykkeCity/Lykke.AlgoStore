using Lykke.AlgoStore.Core.Domain.Validation;
using Lykke.AlgoStore.Core.Validation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Functions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;

namespace Lykke.AlgoStore.Services.Validation
{
    public class CSharpCodeBuildSession : ICodeBuildSession
    {
        private static readonly List<(string, string)> AllowedFxLibs = new List<(string, string)>
        {
            // Add framework libraries here
            ("System.Runtime", "4.2.0.0")
        };

        private readonly string _code;
        private readonly SourceText _sourceText;

        private ValidationResult _syntaxValidationResult;

        private SyntaxTree _syntaxTree;
        private SemanticModel _semanticModel;
        private CSharpCompilation _compilation;
        private CSharpAlgoValidationWalker _syntaxWalker;

        public CSharpCodeBuildSession(string code)
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

            var root = (CompilationUnitSyntax) await _syntaxTree.GetRootAsync();

            _syntaxWalker = new CSharpAlgoValidationWalker(_sourceText);
            _syntaxWalker.Visit(root);

            validationMessages.AddRange(_syntaxWalker.GetMessages());

            if (ErrorExists(validationMessages))
                return CreateAndSetValidationResult(out _syntaxValidationResult, false, validationMessages);

            // Semantic validation

            var coreLib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var fxLibs = AllowedFxLibs
                .Select(l => MetadataReference.CreateFromFile(
                    Assembly.Load(
                        $"{l.Item1}, Version={l.Item2}, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location))
                .ToArray();

            _compilation = CSharpCompilation.Create("CodeValidation")
                .AddSyntaxTrees(_syntaxTree)
                .AddReferences(coreLib)
                .AddReferences(fxLibs)
                .AddReferences(await NuGetReferenceProvider.GetReferences())
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            _semanticModel = _compilation.GetSemanticModel(_syntaxTree, false);
            var semanticDiagnostics = _semanticModel.GetDiagnostics();

            validationMessages.AddRange(semanticDiagnostics.Select(DiagnosticToValidationMessage));

            return CreateAndSetValidationResult(out _syntaxValidationResult,
                !ErrorExists(validationMessages),
                validationMessages);
        }

        public Task<AlgoMetaDataInformation> ExtractMetadata()
        {
            var algoClassFullName = _syntaxWalker.NamespaceNode == null
                ? $"{_syntaxWalker.ClassNode.Identifier}"
                : $"{_syntaxWalker.NamespaceNode.Name}.{_syntaxWalker.ClassNode.Identifier}";

            var newAssembly = GenerateAssembly();
            var newType = newAssembly.GetType(algoClassFullName);
            var metadata = ExtractMetadata(newType);

            return Task.FromResult(metadata);
        }

        private AlgoMetaDataInformation ExtractMetadata(Type algoType)
        {
            var metadata = new AlgoMetaDataInformation
            {
                Functions = new List<AlgoMetaDataFunction>(),
                Parameters = new List<AlgoMetaDataParameter>()
            };

            //Get public properties that have public setter
            var algoProperties = algoType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetSetMethod() != null);

            foreach (var algoProperty in algoProperties)
            {
                //Base parameters
                if (algoProperty.PropertyType.BaseType != typeof(AbstractFunction)
                    && !typeof(IFunction).IsAssignableFrom(algoProperty.PropertyType))
                {
                    var parameter = ToAlgoMetaDataParameter(algoProperty);

                    if (algoProperty.PropertyType.IsEnum)
                        parameter.PredefinedValues = ToEnumValues(algoProperty);

                    metadata.Parameters.Add(parameter);
                }
                //Functions
                else if (algoProperty.PropertyType.BaseType == typeof(AbstractFunction)
                         || typeof(IFunction).IsAssignableFrom(algoProperty.PropertyType))
                {
                    //Get first public property that is not function base parameters but it inherits it
                    var functionProperty = algoProperty.PropertyType
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly |
                                       BindingFlags.GetProperty |
                                       BindingFlags.SetProperty) //BindingFlags.DeclaredOnly -> ignore inherited members
                        .FirstOrDefault(x => x.GetSetMethod() != null &&
                                             typeof(FunctionParamsBase).IsAssignableFrom(x.PropertyType));

                    //If there is no such property, get property that is function base parameters
                    if (functionProperty == null)
                    {
                        functionProperty = algoProperty.PropertyType
                            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty |
                                           BindingFlags.SetProperty)
                            .FirstOrDefault(x => x.PropertyType == typeof(FunctionParamsBase));
                    }

                    //If we still cannot find function parameters property, just continue
                    if (functionProperty == null)
                        continue;

                    //Check if there is a public constructor which takes FunctionParamsBase
                    if (!algoProperty.PropertyType.GetConstructors().Any(x =>
                        x.GetParameters().Any(y => y.ParameterType == functionProperty.PropertyType)))
                        continue;

                    var function = ToAlgoMetadataFunction(algoProperty);

                    function.FunctionParameterType = functionProperty.PropertyType.FullName;
                    var functionParameters = functionProperty.PropertyType
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public |
                                       BindingFlags.GetProperty | BindingFlags.SetProperty);

                    foreach (var functionParameter in functionParameters)
                    {
                        var parameter = ToAlgoMetaDataParameter(functionParameter);

                        if (functionParameter.PropertyType.IsEnum)
                            parameter.PredefinedValues = ToEnumValues(functionParameter);

                        function.Parameters.Add(parameter);
                    }

                    metadata.Functions.Add(function);
                }
            }

            return metadata;
        }

        private ValidationMessage DiagnosticToValidationMessage(Diagnostic diagnostic)
        {
            var validationMessage = new ValidationMessage();

            var position = diagnostic.Location.GetLineSpan().StartLinePosition;

            validationMessage.Line = (uint) position.Line;
            validationMessage.Column = (uint) position.Character;

            validationMessage.Id = diagnostic.Id;
            validationMessage.Message = diagnostic.ToString();

            switch (diagnostic.Severity)
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

        private Assembly GenerateAssembly()
        {
            using (var ms = new MemoryStream())
            {
                //Emit results into a stream
                var emitResult = _compilation.Emit(ms);

                if (!emitResult.Success)
                {
                    // if not successful, throw an exception
                    var failures = emitResult.Diagnostics
                        .Where(x => x.IsWarningAsError || x.Severity == DiagnosticSeverity.Error);

                    var message = string.Join(Environment.NewLine, failures.Select(x => $"{x.Id}: {x.GetMessage()}"));

                    throw new InvalidOperationException(
                        $"Compilation failures!{Environment.NewLine}{message}{Environment.NewLine}Code:{Environment.NewLine}{_code}");
                }

                ms.Seek(0, SeekOrigin.Begin);
                return Assembly.Load(ms.ToArray());
            }
        }

        private static AlgoMetaDataFunction ToAlgoMetadataFunction(PropertyInfo propertyInfo)
        {
            var result = new AlgoMetaDataFunction
            {
                Parameters = new List<AlgoMetaDataParameter>(),
                Id = propertyInfo.Name,
                Type = propertyInfo.PropertyType.FullName
            };

            return result;
        }

        private static List<EnumValue> ToEnumValues(PropertyInfo propertyInfo)
        {
            var result = new List<EnumValue>();
            var enumValues = Enum.GetValues(propertyInfo.PropertyType);

            foreach (var enumValue in enumValues)
            {
                result.Add(new EnumValue
                {
                    Key = ((int) enumValue).ToString(),
                    Value = enumValue.ToString()
                });
            }

            return result;
        }

        private static AlgoMetaDataParameter ToAlgoMetaDataParameter(PropertyInfo algoProperty)
        {
            var parameter = new AlgoMetaDataParameter
            {
                Key = algoProperty.Name,
                Type = algoProperty.PropertyType.FullName
            };

            return parameter;
        }
    }
}
