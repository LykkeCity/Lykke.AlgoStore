using Lykke.AlgoStore.Core.Constants;
using Lykke.AlgoStore.Core.Domain.Validation;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;
using Lykke.AlgoStore.Services.Strings;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ValidationResult = Lykke.AlgoStore.Core.Domain.Validation.ValidationResult;

namespace Lykke.AlgoStore.Services.Validation
{
    public class CSharpCodeBuildSession : ICodeBuildSession
    {
        private static readonly List<(string, string)> AllowedFxLibs = new List<(string, string)>
        {
            // Add framework libraries here
            ("System.Runtime", "4.2.0.0"),
            ("System.Collections", "4.1.1.0")
        };

        private static readonly HashSet<string> BlacklistedPropertyNames = new HashSet<string>
        {
            "AssetPair",
            "CandleInterval",
            "StartFrom",
            "EndOn"
        };

        private static readonly HashSet<string> BlacklistedNamespaces = new HashSet<string>
        {
            "System.Reflection",
            "System.Net",
            "System.IO"
        };

        private static readonly HashSet<string> BlacklistedTypes = new HashSet<string>
        {
            "System.Type",
            "System.Activator"
        };

        private const string INDICATORS_ASSEMBLY = "Lykke.AlgoStore.Algo";

        private readonly string _code;
        private readonly string _algoNamespaceValue;
        private readonly SourceText _sourceText;

        private ValidationResult _syntaxValidationResult;

        private SyntaxTree _syntaxTree;
        private SemanticModel _semanticModel;
        private CSharpCompilation _compilation;
        private CSharpAlgoValidationWalker _syntaxWalker;
        private List<FastIndicatorInitCandidate> _filteredCandidates = new List<FastIndicatorInitCandidate>();

        public CSharpCodeBuildSession(string code, string AlgoNamespaceValue)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));

            _code = code;
            _algoNamespaceValue = AlgoNamespaceValue;
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

            _syntaxWalker = new CSharpAlgoValidationWalker(_sourceText, _algoNamespaceValue);
            _syntaxWalker.Visit(root);

            validationMessages.AddRange(_syntaxWalker.GetMessages());

            if (ErrorExists(validationMessages))
                return CreateAndSetValidationResult(out _syntaxValidationResult, false, validationMessages);

            var collectionWalker = new CSharpIdentifierCollectionWalker();
            collectionWalker.Visit(root);

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
                .WithOptions(
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                        .WithMetadataImportOptions(MetadataImportOptions.Public));

            // Extract semantic model once with only public and protected members imported for diagnostics,
            // then extract again with all members imported for extraction of metadata
            _semanticModel = _compilation.GetSemanticModel(_syntaxTree, false);
            var semanticDiagnostics = _semanticModel.GetDiagnostics();

            _compilation = _compilation.WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                                                        .WithMetadataImportOptions(MetadataImportOptions.All));
            _semanticModel = _compilation.GetSemanticModel(_syntaxTree, false);

            ValidateBlacklistedTypes(validationMessages, collectionWalker);

            var usedIndicatorNames = new HashSet<string>();

            foreach (var candidate in _syntaxWalker.IndicatorInitializations)
            {
                var invocationOperation = _semanticModel.GetOperation(candidate.Invocation) as IInvocationOperation;
                if (invocationOperation == null) continue;

                // Cannot invoke methods not defined in BaseAlgo
                if (invocationOperation.TargetMethod.ContainingType.Name != "BaseAlgo") continue;

                var returnType = invocationOperation.TargetMethod.ReturnType;

                if (!returnType.AllInterfaces.Any(i => i.Name == "IIndicator" && i.ContainingAssembly.Name == "Lykke.AlgoStore.Algo")) continue;

                var args = invocationOperation.Arguments;

                // Make sure the first argument (indicator name) is a string literal and has a value
                if (!args[0].Value.ConstantValue.HasValue || string.IsNullOrEmpty(args[0].Value.ConstantValue.Value as string))
                {
                    validationMessages.Add(CreateFromError(
                        ValidationErrors.ERROR_INDICATOR_NAME_NOT_LITERAL,
                        Phrases.ERROR_INDICATOR_NAME_NOT_LITERAL,
                        args[0].Syntax.GetLocation()));
                    continue;
                }

                var indicatorName = args[0].Value.ConstantValue.Value as string;

                // Check if the same argument name was already used
                if(usedIndicatorNames.Contains(indicatorName))
                {
                    validationMessages.Add(CreateFromError(
                        ValidationErrors.ERROR_INDICATOR_DUPLICATE_NAME,
                        Phrases.ERROR_INDICATOR_DUPLICATE_NAME,
                        args[0].Syntax.GetLocation()));
                    continue;
                }

                usedIndicatorNames.Add(args[0].Value.ConstantValue.Value as string);
                _filteredCandidates.Add(candidate);

                // Basic check for argument validity
                foreach(var arg in args)
                {
                    // Constant - we don't care about these
                    if (arg.Value.ConstantValue.HasValue) continue;

                    if (arg.Value is ILocalReferenceOperation)
                    {
                        validationMessages.Add(CreateFromError(
                            ValidationErrors.ERROR_PARAMETER_LOCAL_REFERENCE_USED,
                            Phrases.ERROR_PARAMETER_LOCAL_REFERENCE_USED,
                            arg.Syntax.GetLocation()));
                        continue;
                    }

                    if (arg.Value is IConversionOperation)
                    {
                        if (((IConversionOperation)arg.Value).Operand is ILocalReferenceOperation)
                        {
                            validationMessages.Add(CreateFromError(
                            ValidationErrors.ERROR_PARAMETER_LOCAL_REFERENCE_USED,
                            Phrases.ERROR_PARAMETER_LOCAL_REFERENCE_USED,
                            arg.Syntax.GetLocation()));
                            continue;
                        }
                    }

                    var invocation = arg.Value as IInvocationOperation;

                    // We want to check for Default invocations here
                    if (invocation == null) continue;

                    if (invocation.TargetMethod.Name != "Default"
                        || invocation.TargetMethod.ContainingAssembly.Identity.Name != INDICATORS_ASSEMBLY)
                        continue;

                    // Make sure the argument is not null
                    var defaultArg = invocation.Arguments[0];

                    if (defaultArg.Value is ILocalReferenceOperation)
                    {
                        validationMessages.Add(CreateFromError(
                            ValidationErrors.ERROR_PARAMETER_LOCAL_REFERENCE_USED,
                            Phrases.ERROR_PARAMETER_LOCAL_REFERENCE_USED,
                            defaultArg.Syntax.GetLocation()));
                        continue;
                    }

                    if (defaultArg.Value.ConstantValue.HasValue && defaultArg.Value.ConstantValue.Value == null)
                    {
                        validationMessages.Add(CreateFromError(
                            ValidationErrors.ERROR_DEFAULT_VALUE_NULL,
                            Phrases.ERROR_DEFAULT_VALUE_NULL,
                            defaultArg.Syntax.GetLocation()));
                        continue;
                    }
                }
            }

            validationMessages.AddRange(semanticDiagnostics.Select(DiagnosticToValidationMessage));

            return CreateAndSetValidationResult(out _syntaxValidationResult,
                !ErrorExists(validationMessages),
                validationMessages);
        }

        public Task<AlgoMetaDataInformation> ExtractMetadata()
        {
            var classInfo = _semanticModel.GetDeclaredSymbol(_syntaxWalker.ClassNode);

            var metadata = new AlgoMetaDataInformation
            {
                Functions = new List<AlgoMetaDataFunction>()
            };

            metadata.Parameters = ExtractAlgoParameters(classInfo);
            metadata.Functions = ExtractIndicators();

            return Task.FromResult(metadata);
        }

        private void ValidateBlacklistedTypes(List<ValidationMessage> messages, CSharpIdentifierCollectionWalker walker)
        {
            foreach (var usedNamespace in walker.Usings)
            {
                ValidateNamespace(messages, usedNamespace, usedNamespace.Name.ToString());
            }

            foreach (var invocation in walker.Invocations)
            {
                var operation = _semanticModel.GetOperation(invocation) as IInvocationOperation;

                if (operation == null) continue;

                foreach (var typeParam in operation.TargetMethod.TypeParameters)
                    ValidateType(messages, typeParam.DeclaringSyntaxReferences.First().GetSyntax(), typeParam);

                ValidateType(messages, invocation, operation.TargetMethod.ReturnType);
                ValidateType(messages, invocation, operation.TargetMethod.ContainingType);
            }

            foreach (var declarator in walker.Declarators)
            {
                var declaredSymbol = _semanticModel.GetDeclaredSymbol(declarator) as ILocalSymbol;

                if (declaredSymbol == null) continue;

                ValidateType(messages, declarator, declaredSymbol.Type);
            }

            foreach (var typeDeclaration in walker.TypeDeclarations)
            {
                var declaredSymbol = _semanticModel.GetDeclaredSymbol(typeDeclaration) as INamedTypeSymbol;

                if (declaredSymbol == null) continue;

                var members = declaredSymbol.GetMembers();

                foreach(var member in members)
                {
                    var propertyDecl = member as IPropertySymbol;

                    if(propertyDecl != null)
                    {
                        ValidateType(messages, member.GetSyntaxNode(), propertyDecl.Type);
                        continue;
                    }

                    var fieldDecl = member as IFieldSymbol;

                    if(fieldDecl != null)
                    {
                        ValidateType(messages, member.GetSyntaxNode(), fieldDecl.Type);
                        continue;
                    }

                    var methodDecl = member as IMethodSymbol;

                    if(methodDecl != null)
                    {
                        foreach(var param in methodDecl.Parameters)
                        {
                            if (param.Type is INamedTypeSymbol && ((INamedTypeSymbol)param.Type).IsGenericType) continue;

                            ValidateType(messages, param.GetSyntaxNode(), param.Type);
                        }

                        if (!(methodDecl.ReturnType is INamedTypeSymbol) || !((INamedTypeSymbol)methodDecl.ReturnType).IsGenericType)
                            ValidateType(messages, methodDecl.GetSyntaxNode(), methodDecl.ReturnType);
                    }
                }
            }
        }

        private List<AlgoMetaDataParameter> ExtractAlgoParameters(INamedTypeSymbol classInfo)
        {
            var currentClass = classInfo;
            var parameters = new List<AlgoMetaDataParameter>();
            var existingNames = new HashSet<string>();

            while (currentClass != null)
            {
                var classMembers = currentClass.GetMembers();
                var properties = classMembers.OfType<IPropertySymbol>();

                foreach (var prop in properties)
                {
                    // Not allowed to have blacklisted property names, unless they come from BaseAlgo
                    if ((currentClass.Name != "BaseAlgo" || currentClass.ContainingAssembly.Identity.Name != INDICATORS_ASSEMBLY)
                        && BlacklistedPropertyNames.Contains(prop.Name)) continue;

                    // Must only have a getter in order to be immutable
                    if (!prop.IsReadOnly) continue;

                    // Backing field generated for auto properties
                    if (!classMembers.Any(m => m.Name == $"<{prop.Name}>k__BackingField")) continue;

                    // If we've encountered a property with the same name - continue
                    if (existingNames.Contains(prop.Name)) continue;

                    existingNames.Add(prop.Name);

                    var param = new AlgoMetaDataParameter
                    {
                        Key = prop.Name,
                        Type = prop.Type.ToString(),
                        PredefinedValues = ToEnumValues(prop.Type),
                        Visible = true
                    };

                    CheckAttribute(prop, param);

                    parameters.Add(param);
                }

                currentClass = currentClass.BaseType;
            }

            return parameters;
        }

        private void CheckAttribute(ISymbol prop, AlgoMetaDataParameter param)
        {
            var attributes = prop.GetAttributes();

            foreach (var attr in attributes)
            {
                // Only support attributes which come from our indicators assembly
                if (attr.AttributeClass.ContainingAssembly.Identity.Name != INDICATORS_ASSEMBLY) continue;

                switch (attr.AttributeClass.Name)
                {
                    case "DescriptionAttribute":
                        param.Description = attr.ConstructorArguments[0].Value?.ToString();
                        break;
                    case "DefaultValueAttribute":
                        var argument = attr.ConstructorArguments[0];

                        if (argument.Value == null) break;

                        // Get the conversion type between what's in DefaultValue and the property type
                        var conversion = _compilation.ClassifyConversion(argument.Type, prop.ContainingType);

                        // Either the type is the same or the conversion is implicit
                        if (conversion.IsIdentity || conversion.IsImplicit)
                        {
                            if (argument.Value.GetType().IsEnum)
                                param.Value = Convert.ChangeType(argument.Value, Enum.GetUnderlyingType(argument.Value.GetType())).ToString();
                            else
                                param.Value = argument.Value.ToString();
                        }
                        break;
                }
            }
        }

        private List<AlgoMetaDataFunction> ExtractIndicators()
        {
            var indicators = new List<AlgoMetaDataFunction>();

            foreach(var candidate in _filteredCandidates)
            {
                var invocation = _semanticModel.GetOperation(candidate.Invocation) as IInvocationOperation;

                var indicator = new AlgoMetaDataFunction
                {
                    Id = invocation.Arguments[0].Value.ConstantValue.Value as string,
                    Parameters = new List<AlgoMetaDataParameter>()
                };

                for(var i = 1; i < invocation.Arguments.Length; i++)
                {
                    var arg = invocation.Arguments[i];
                    var param = arg.Parameter;

                    var metaDataParam = new AlgoMetaDataParameter
                    {
                        Key = param.Name,
                        Type = param.Type.ToString().Replace("?", ""),
                        PredefinedValues = ToEnumValues(param.Type),
                        Visible = true
                    };                  

                    CheckAttribute(param, metaDataParam);

                    // There is a constant in the place of the param - include it in the list of parameters
                    // with Visibility set to false
                    if (arg.Value.ConstantValue.HasValue || arg.ArgumentKind == ArgumentKind.DefaultValue)
                    {
                        // If null is written in place of param - add it to list (since we need to fill in the value)
                        //Else add it to list but mark it as not visible
                        if (arg.Value.ConstantValue.Value == null)
                            indicator.Parameters.Add(metaDataParam);
                        else
                        {
                            metaDataParam.Value = arg.Value.ConstantValue.Value.ToString();
                            metaDataParam.Visible = false;
                            indicator.Parameters.Add(metaDataParam);
                        }

                        continue;
                    }

                    // There is an enum constant in the place of the param - include it in the list of parameters
                    // with Visibility set to false
                    if (arg.Value.Children.Any() && arg.Value.Children.First().ConstantValue.HasValue)
                    {
                        // If null is written in place of param - add it to list (since we need to fill in the value)
                        //Else add it to list but mark it as not visible
                        if (arg.Value.Children.First().ConstantValue.Value == null)
                            indicator.Parameters.Add(metaDataParam);
                        else
                        {
                            metaDataParam.Value = arg.Value.Children.First().ConstantValue.Value.ToString();
                            metaDataParam.Visible = false;
                            indicator.Parameters.Add(metaDataParam);
                        }

                        continue;
                    }

                    var dtType = typeof(DateTime);
                    if (metaDataParam.Type == dtType.FullName)
                    {
                        var nameSpaceString = dtType.Namespace;
                        var dtInitializationText = arg.Value.Syntax.GetText().ToString();
                        string objectInstantiationString;

                        if (!dtInitializationText.Contains(dtType.FullName))
                        {
                            var dateTimePosition = dtInitializationText.IndexOf(dtType.Name);

                            objectInstantiationString = dtInitializationText.Substring(0, dateTimePosition)
                                + nameSpaceString + "." + dtInitializationText.Substring(dateTimePosition);
                        }
                        else
                        {
                            objectInstantiationString = dtInitializationText;
                        }                       

                        var date = CSharpScript.EvaluateAsync<DateTime>(objectInstantiationString).Result;
                        if (date != null)
                        {
                            metaDataParam.Value = date.ToUniversalTime().ToString(AlgoStoreConstants.DateTimeFormat, CultureInfo.InvariantCulture);
                            metaDataParam.Visible = false;
                            indicator.Parameters.Add(metaDataParam);
                        }                        
                    }

                    var innerInvocation = arg.Value as IInvocationOperation;

                    // If it is not an invocation, ignore the parameter
                    if (innerInvocation == null) continue;

                    // We only care for Default invocations here
                    if (innerInvocation.TargetMethod.Name != "Default"
                        || innerInvocation.TargetMethod.ContainingAssembly.Identity.Name != INDICATORS_ASSEMBLY)
                        continue;

                    // Set the value to whatever is inside the Default call
                    metaDataParam.Value = innerInvocation.Arguments[0].Value.ConstantValue.Value?.ToString();

                    indicator.Parameters.Add(metaDataParam);
                }

                indicators.Add(indicator);
            }

            return indicators;
        }

        private ValidationMessage DiagnosticToValidationMessage(Diagnostic diagnostic)
        {
            var validationMessage = new ValidationMessage();

            var position = diagnostic.Location.GetLineSpan().StartLinePosition;

            validationMessage.Line = (uint)position.Line;
            validationMessage.Column = (uint)position.Character;

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

        private ValidationMessage CreateFromError(string id, string message, Location location)
        {
            var position = location?.GetLineSpan().StartLinePosition;

            return new ValidationMessage
            {
                Id = id,
                Message = message,
                Line = (uint)(position?.Line ?? 0),
                Column = (uint)(position?.Character ?? 0),
                Severity = ValidationSeverity.Error
            };
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

        private static List<EnumValue> ToEnumValues(ITypeSymbol type)
        {
            if(type.Name == "Nullable" && type.ContainingAssembly.Name == "System.Private.CoreLib")
                type = (type as INamedTypeSymbol).TypeArguments[0];

            if (type.TypeKind != TypeKind.Enum) return null;

            var values = new List<EnumValue>();
            var enumMembers = type.GetMembers().OfType<IFieldSymbol>();

            foreach (var member in enumMembers)
            {
                var attr = member.GetAttributes()
                                 .FirstOrDefault(a => a.AttributeClass.Name == "DisplayAttribute");

                values.Add(new EnumValue
                {
                    Key = attr?.NamedArguments
                              .FirstOrDefault(kvp => kvp.Key == "Name")
                              .Value
                              .Value
                              ?.ToString() 
                              ?? member.Name,
                    Value = member.ConstantValue.ToString()
                });
            }

            return values;
        }

        private void ValidateType(List<ValidationMessage> messages, SyntaxNode syntaxNode, ITypeSymbol typeSymbol)
        {
            while(typeSymbol.Kind == SymbolKind.ArrayType)
            {
                typeSymbol = (typeSymbol as IArrayTypeSymbol).ElementType;
            }

            if (BlacklistedTypes.Contains(typeSymbol.ToString().Replace("?", "")))
            {
                messages.Add(CreateFromError(
                        ValidationErrors.ERROR_BLACKLISTED_TYPE_USED,
                        Phrases.ERROR_BLACKLISTED_TYPE_USED,
                        syntaxNode?.GetLocation()));
            }

            if(typeSymbol.ContainingNamespace != null)
                ValidateNamespace(messages, syntaxNode, typeSymbol.ContainingNamespace.ToString());
        }

        private void ValidateNamespace(List<ValidationMessage> messages, SyntaxNode syntaxNode, string nameSpc)
        {
            if(BlacklistedNamespaces.Any(n => nameSpc.StartsWith(n)))
            {
                messages.Add(CreateFromError(
                       ValidationErrors.ERROR_BLACKLISTED_NAMESPACE_USED,
                       Phrases.ERROR_BLACKLISTED_NAMESPACE_USED,
                       syntaxNode?.GetLocation()));
            }
        }

        private bool IsPropertyValidType(IPropertySymbol property)
        {
            var conversions = new[]
            {
                _compilation.ClassifyConversion(property.Type, _compilation.GetSpecialType(SpecialType.System_UInt64)),
                _compilation.ClassifyConversion(property.Type, _compilation.GetSpecialType(SpecialType.System_Int64)),
                _compilation.ClassifyConversion(property.Type, _compilation.GetSpecialType(SpecialType.System_DateTime)),
                _compilation.ClassifyConversion(property.Type, _compilation.GetSpecialType(SpecialType.System_String))
            };

            return conversions.Any(c => (c.IsImplicit || c.IsIdentity) && !c.IsUserDefined);
        }
    }
}
