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
        private CSharpCompilation _compilation;

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
                    Assembly.Load(
                        $"{l.Item1}, Version={l.Item2}, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location))
                .ToArray();


            _compilation = CSharpCompilation.Create("CodeValidation")
                            .AddSyntaxTrees(_syntaxTree)
                            .AddReferences(coreLib)
                            .AddReferences(fxLibs)
                            .AddReferences(await NuGetReferenceProvider.GetReferences());

            _compilation = _compilation.WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            _semanticModel = _compilation.GetSemanticModel(_syntaxTree, false);
            var semanticDiagnostics = _semanticModel.GetDiagnostics();

            validationMessages.AddRange(semanticDiagnostics.Select(DiagnosticToValidationMessage));

            return CreateAndSetValidationResult(out _syntaxValidationResult, 
                                                !ErrorExists(validationMessages), 
                                                validationMessages);
        }

        public async Task<AlgoMetaDataInformation> ExtractMetadata()
        {
            var root = (CompilationUnitSyntax)await _syntaxTree.GetRootAsync();
            var namespaceDeclaration = (NamespaceDeclarationSyntax)root.Members[0];
            var classDeclaration = namespaceDeclaration.Members
                .OfType<ClassDeclarationSyntax>()
                .First();

            var newAssembly = GenerateAssembly();
            var newType = newAssembly.GetType($"{namespaceDeclaration.Name}.{classDeclaration.Identifier.Text}");
            var metadata = ExtractMetadata(newType);

            return metadata;
        }

        private AlgoMetaDataInformation ExtractMetadata(Type algoType)
        {
            var metadata = new AlgoMetaDataInformation
            {
                Functions = new List<AlgoMetaDataFunction>(),
                Parameters = new List<AlgoMetaDataParameter>()
            };

            var algoProperties = algoType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var algoProperty in algoProperties)
            {
                var parameter = ToAlgoMetaDataParameter(algoProperty);

                if (algoProperty.PropertyType.IsEnum)
                    parameter.PredefinedValues = ToEnumValues(algoProperty);

                metadata.Parameters.Add(parameter);
            }

            var functionProperties = algoType
                .GetRuntimeFields()
                .Where(x => x.FieldType.BaseType == typeof(AbstractFunction));

            foreach (var functionProperty in functionProperties)
            {
                var function = ToAlgoMetadataFunction(functionProperty);

                var functionParamBasePropertyField = functionProperty
                    .FieldType
                    .GetFields()
                    .FirstOrDefault(x => x.FieldType.BaseType == typeof(FunctionParamsBase));

                //There is a public property which base type is of FunctionParamBase type
                if (functionParamBasePropertyField != null)
                {
                    function.FunctionParameterType = functionParamBasePropertyField.FieldType.FullName;
                    var functionParameters = functionParamBasePropertyField.FieldType.GetProperties();

                    foreach (var functionParameter in functionParameters)
                    {
                        var parameter = ToAlgoMetaDataParameter(functionParameter);

                        if (functionParameter.PropertyType.IsEnum)
                            parameter.PredefinedValues = ToEnumValues(functionParameter);

                        function.Parameters.Add(parameter);

                    }
                }
                //There is a NO public property which base type is of FunctionParamBase type,
                //so we should get base algo parameters
                else
                {
                    var functionParameters = functionProperty
                        .FieldType
                        .GetProperties()
                        .Where(x => x.PropertyType == typeof(FunctionParamsBase));

                    foreach (var functionParameter in functionParameters)
                    {
                        function.FunctionParameterType = functionParameter.PropertyType.FullName;

                        var functionParameterProperties = functionParameter
                            .PropertyType
                            .GetProperties();

                        foreach (var functionParameterProperty in functionParameterProperties)
                        {
                            var parameter = ToAlgoMetaDataParameter(functionParameterProperty);

                            if (functionParameterProperty.PropertyType.IsEnum)
                                parameter.PredefinedValues = ToEnumValues(functionParameterProperty);

                            function.Parameters.Add(parameter);
                        }
                    }
                }

                metadata.Functions.Add(function);
            }

            return metadata;
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

        private Assembly GenerateAssembly()
        {
            Assembly newAssembly;

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
                newAssembly = Assembly.Load(ms.ToArray());
            }

            return newAssembly;
        }

        private static AlgoMetaDataFunction ToAlgoMetadataFunction(FieldInfo fieldInfo)
        {
            var result = new AlgoMetaDataFunction
            {
                Parameters = new List<AlgoMetaDataParameter>(),
                Id = fieldInfo.Name,
                Type = fieldInfo.FieldType.FullName
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
                    Key = ((int)enumValue).ToString(),
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
