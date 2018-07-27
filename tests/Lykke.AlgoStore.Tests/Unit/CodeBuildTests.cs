using System.Collections.Generic;
using Lykke.AlgoStore.Core.Domain.Validation;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.Services;
using NUnit.Framework;
using System.Linq;
using FluentAssertions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;
using System;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class CodeBuildTests
    {
        #region Data Generation
        private const string ALGONAMESPACE = "Lykke.AlgoStore.CSharp.Algo.Implemention.ExecutableClass";

        private static readonly List<EnumValue> CandleTimeIntervalPredefinedValues = new List<EnumValue>
        {
            new EnumValue
            {
                Key = "Unspecified",
                Value = "0"
            },
            new EnumValue
            {
                Key = "Second",
                Value = "1"
            },
            new EnumValue
            {
                Key = "Minute",
                Value = "60"
            },
            new EnumValue
            {
                Key = "5 Minutes",
                Value = "300"
            },
            new EnumValue
            {
                Key = "15 Minutes",
                Value = "900"
            },
            new EnumValue
            {
                Key = "30 Minutes",
                Value = "1800"
            },
            new EnumValue
            {
                Key = "Hour",
                Value = "3600"
            },
            new EnumValue
            {
                Key = "4 Hours",
                Value = "14400"
            },
            new EnumValue
            {
                Key = "6 Hours",
                Value = "21600"
            },
            new EnumValue
            {
                Key = "12 Hours",
                Value = "43200"
            },
            new EnumValue
            {
                Key = "Day",
                Value = "86400"
            },
            new EnumValue
            {
                Key = "Week",
                Value = "604800"
            },
            new EnumValue
            {
                Key = "Month",
                Value = "3000000"
            }
        };

        private static readonly List<EnumValue> CandlePredefinedValues = new List<EnumValue>
        {
            new EnumValue
            {
                Key = "OPEN",
                Value = "0"
            },
            new EnumValue
            {
                Key = "CLOSE",
                Value = "1"
            },
            new EnumValue
            {
                Key = "LOW",
                Value = "2"
            },
            new EnumValue
            {
                Key = "HIGH",
                Value = "3"
            }
        };

        private static IList<AlgoMetaDataParameter> BaseAlgoParameters => new List<AlgoMetaDataParameter>
        {
            new AlgoMetaDataParameter
            {
                Key = "AssetPair",
                Type = "string"
            },
            
            new AlgoMetaDataParameter
            {
                Key = "StartFrom",
                Type = "System.DateTime"
            },
            new AlgoMetaDataParameter
            {
                Key = "EndOn",
                Type = "System.DateTime"
            },
            new AlgoMetaDataParameter
            {
                Key = "Volume",
                Type = "double"
            },
            new AlgoMetaDataParameter
            {
                Key = "TradedAsset",
                Type = "string"
            },
            new AlgoMetaDataParameter
            {
                Key = "CandleInterval",
                Type = "Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval",
                PredefinedValues = CandleTimeIntervalPredefinedValues
            }
        };

        private static AlgoMetaDataInformation BaseAlgoMetadataWithNoFunctions => new AlgoMetaDataInformation
        {
            Functions = new List<AlgoMetaDataFunction>(),
            Parameters = BaseAlgoParameters
        };

        private static AlgoMetaDataFunction SmaFunctionMetadata
        {
            get
            {
                return new AlgoMetaDataFunction
                {
                    Id = "Sma",
                    Parameters = new List<AlgoMetaDataParameter>
                    {
                       new AlgoMetaDataParameter
                        {
                            Key = "period",
                            Type = "int"
                        },
                       new AlgoMetaDataParameter
                        {
                            Key = "startingDate",
                            Type = "System.DateTime"
                        },
                        new AlgoMetaDataParameter
                        {
                            Key = "endingDate",
                            Type = "System.DateTime"
                        },
                        new AlgoMetaDataParameter
                        {
                            Key = "assetPair",
                            Type = "string"
                        },
                        new AlgoMetaDataParameter
                        {
                            Key = "candleOperationMode",
                            Type =
                                "Lykke.AlgoStore.Algo.CandleOperationMode",
                            PredefinedValues = CandlePredefinedValues
                        },
                        new AlgoMetaDataParameter
                        {
                            Key = "candleTimeInterval",
                            Type = "Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval",
                            PredefinedValues = CandleTimeIntervalPredefinedValues
                        }
                    }
                };
            }
        }

        private static AlgoMetaDataInformation BaseAlgoMetadataWithSmaFunction => new AlgoMetaDataInformation
        {
            Functions = new List<AlgoMetaDataFunction> { SmaFunctionMetadata },
            Parameters = BaseAlgoParameters
        };

        private static AlgoMetaDataFunction CustomFunctionMetadata => new AlgoMetaDataFunction()
        {
            Id = "CustomFunc",
            FunctionParameterType = "Lykke.AlgoStore.CSharp.Algo.Implemention.ExecutableClass.AlgoParameters",
            Type = "Lykke.AlgoStore.CSharp.Algo.Implemention.ExecutableClass.CustomFunction",
            Parameters = new List<AlgoMetaDataParameter>
            {
                new AlgoMetaDataParameter
                {
                    Key = "Period",
                    Value = "5",
                    Description = "test",
                    Type = "System.Int32"
                },
                new AlgoMetaDataParameter
                {
                    Key = "AssetPair",
                    Type = "System.String"
                },
                new AlgoMetaDataParameter
                {
                    Key = "CandleOperationMode",
                    Type =
                        "Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Functions.FunctionParamsBase+CandleValue",
                    PredefinedValues = CandlePredefinedValues
                },
                new AlgoMetaDataParameter
                {
                    Key = "FunctionInstanceIdentifier",
                    Type = "System.String"
                },
                new AlgoMetaDataParameter
                {
                    Key = "StartingDate",
                    Type = "System.DateTime"
                },
                new AlgoMetaDataParameter
                {
                    Key = "EndingDate",
                    Type = "System.DateTime"
                },
                new AlgoMetaDataParameter
                {
                    Key = "CandleTimeInterval",
                    Type = "Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval",
                    PredefinedValues = CandleTimeIntervalPredefinedValues
                }
            }
        };

        private static AlgoMetaDataInformation BaseAlgoMetadataWithCustomFunction => new AlgoMetaDataInformation
        {
            Functions = new List<AlgoMetaDataFunction> { CustomFunctionMetadata },
            Parameters = BaseAlgoParameters
        };

        #endregion

        [Test]
        public void SyntaxValidation_Fails_WhenNoClassInheritsBaseAlgo()
        {
            var code = @"namespace Lykke.AlgoStore.CSharp.Algo.Implemention.ExecutableClass{
                            class A {} class B : C {}
                        }";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
            Then_Result_MustContainMessage(result, "AS0001");
        }

        [Test]
        public void SyntaxValidation_Fails_WhenMultipleClassesInheritBaseAlgo()
        {
            var code = @"namespace Lykke.AlgoStore.CSharp.Algo.Implemention.ExecutableClass{
                            class A : BaseAlgo {} class B : BaseAlgo {}
                        }";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
            Then_Result_MustContainMessage(result, "AS0002");
        }

        [Test]
        public void SyntaxValidation_Fails_WhenAlgoNotSealed()
        {
            var code = @"namespace Lykke.AlgoStore.CSharp.Algo.Implemention.ExecutableClass{
                            class A : BaseAlgo {}
                        }";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
            Then_Result_MustContainMessage(result, "AS0003");
        }

        [Test]
        public void SyntaxValidation_Fails_WhenTypeNamedBaseAlgo()
        {
            var code = @"namespace Lykke.AlgoStore.CSharp.Algo.Implemention.ExecutableClass{
                            interface BaseAlgo {} enum BaseAlgo {} class BaseAlgo {} struct BaseAlgo {}
                        }";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
            Then_Result_MustContainMessageNTimes(result, "AS0004", 4);
        }

        [Test]
        public void SyntaxValidation_Fails_WhenEventsNotImplemented()
        {
            var code = @"namespace Lykke.AlgoStore.CSharp.Algo.Implemention.ExecutableClass{
                        class A : BaseAlgo {void A() {} void OnCandleReceived() {}}
                        }";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
            Then_Result_MustContainMessage(result, "AS0005");
        }

        [Test]
        public void SyntaxValidation_Succeeds_WhenAlgoProperlyImplemented()
        {
            var code = @"using Lykke.AlgoStore.Algo; 
                         namespace Lykke.AlgoStore.CSharp.Algo.Implemention.ExecutableClass
                         {
                             sealed class A : BaseAlgo 
                             { 
                                public override void OnCandleReceived(ICandleContext context) {} 
                             }
                         }";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustSucceedAndContainNoMessages(result);
        }

        [Test]
        public void SyntaxValidation_Fails_WhenNamespaceMissing()
        {
            var code = @"using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain; 
                        sealed class A : BaseAlgo 
                            { 
                            public override void OnCandleReceived(ICandleContext context) {} 
                            }";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
            Then_Result_MustContainMessage(result, "AS0010");
        }

        [Test]
        public void SyntaxValidation_Fails_WhenNamespaceNotCorrect()
        {
            var code = @"using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain; 
                         namespace Lykke.AlgoStore.CSharp.Algo.Implemention.IncorrectNamespace{
                            sealed class A : BaseAlgo 
                             { 
                                public override void OnCandleReceived(ICandleContext context) {} 
                             }
                        }";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
            Then_Result_MustContainMessage(result, "AS0009");
        }

        [Test]
        public void
            ExtractMetadata_Succeeds_WhenAlgoProperlyImplementedWithSmaFunctionAndNoAdditionalProperties()
        {
            var code = @"
using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.Algo.Indicators;
namespace Lykke.AlgoStore.CSharp.Algo.Implemention.ExecutableClass
{
    sealed class Algo : BaseAlgo 
    { 
        public SMA Sma {get; set; }

        public override void OnStartUp()
        {
            Sma = SMA(""Sma"");
        }
        public override void OnCandleReceived(ICandleContext context) {} 
    }
}
";

            var session = GetCSharpCodeBuildSession(code);
            var validationResult = session.Validate().Result;

            Assert.AreEqual(true, validationResult.IsSuccessful);

            var metadata = session.ExtractMetadata().Result;

            metadata.Should().BeEquivalentTo(BaseAlgoMetadataWithSmaFunction);
        }

        [Test]
        public void
            ExtractMetadata_Succeeds_WhenAlgoProperlyImplementedAndHasNoAdditionalPropertiesAndFunctions()
        {
            var code = @"
using Lykke.AlgoStore.Algo;
namespace Lykke.AlgoStore.CSharp.Algo.Implemention.ExecutableClass
{
    sealed class Algo : BaseAlgo 
    { 
        public override void OnCandleReceived(ICandleContext context) {} 
    }
}
";

            var session = GetCSharpCodeBuildSession(code);
            var validationResult = session.Validate().Result;

            Assert.AreEqual(true, validationResult.IsSuccessful);

            var metadata = session.ExtractMetadata().Result;

            metadata.Should().BeEquivalentTo(BaseAlgoMetadataWithNoFunctions);
        }
        
        [Test]
        [TestCase("using System.Reflection;", "", "", "AS0012")]
        [TestCase("using System;", "", "Type test = null;", "AS0011")]
        [TestCase("using System;", "class c { Type f; }", "", "AS0011")]
        [TestCase("using System;", "class c { Type p {get;} }", "", "AS0011")]
        [TestCase("using System;", "class c { Type m() {return null;} }", "", "AS0011")]
        [TestCase("", "class c { T m<T>(System.Type t) {return default(T);} }", "", "AS0011")]
        [TestCase("", "class c {object m() {return System.Activator.CreateInstance(typeof(int));}}", "", "AS0011")]
        public void SyntaxValidation_Fails_WhenUsingBlacklistedCode(
            string usingToTest,
            string classToTest,
            string codeToTest,
            string error)
        {
            var code = $@"
using Lykke.AlgoStore.Algo;
{usingToTest}
namespace Lykke.AlgoStore.CSharp.Algo.Implemention.ExecutableClass
{{
    {classToTest}
    sealed class Algo : BaseAlgo 
    {{ 
        public override void OnCandleReceived(ICandleContext context) 
        {{
            {codeToTest}
        }}
    }}
}}";
            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
            Then_Result_MustContainMessage(result, error);
        }

        private ICodeBuildSession GetCSharpCodeBuildSession(string code)
        {
            var codeValidationService = new CodeBuildService(ALGONAMESPACE);
            var session = codeValidationService.StartSession(code);

            return session;
        }

        private ValidationResult When_Code_IsSyntaxValidated(string code)
        {
            var codeValidationService = new CodeBuildService(ALGONAMESPACE);
            var session = codeValidationService.StartSession(code);

            return session.Validate().Result;
        }

        private void Then_Result_MustContainMessage(ValidationResult result, string messageId)
        {
            Assert.IsTrue(result.Messages.Any(m => m.Id == messageId));
        }

        private void Then_Result_MustFailAndContainMessages(ValidationResult result)
        {
            Assert.AreEqual(false, result.IsSuccessful);
            Assert.IsTrue(result.Messages.Count > 0);
        }

        private void Then_Result_MustSucceedAndContainNoMessages(ValidationResult result)
        {
            Assert.AreEqual(true, result.IsSuccessful);
            Assert.IsTrue(result.Messages.Count == 0);
        }

        private void Then_Result_MustContainMessageNTimes(ValidationResult result, string messageId, int count)
        {
            Assert.AreEqual(count, result.Messages.Where(m => m.Id == messageId).Count());
        }
    }
}
