using System.Collections.Generic;
using Lykke.AlgoStore.Core.Domain.Validation;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.Services;
using NUnit.Framework;
using System.Linq;
using FluentAssertions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class CodeBuildTests
    {
        #region Data Generation

        private static readonly List<EnumValue> CandleTimeIntervalPredefinedValues = new List<EnumValue>
        {
            new EnumValue
            {
                Key = "Unspecified",
                Value = "0"
            },
            new EnumValue
            {
                Key = "Sec",
                Value = "1"
            },
            new EnumValue
            {
                Key = "Minute",
                Value = "60"
            },
            new EnumValue
            {
                Key = "Min5",
                Value = "300"
            },
            new EnumValue
            {
                Key = "Min15",
                Value = "900"
            },
            new EnumValue
            {
                Key = "Min30",
                Value = "1800"
            },
            new EnumValue
            {
                Key = "Hour",
                Value = "3600"
            },
            new EnumValue
            {
                Key = "Hour4",
                Value = "14400"
            },
            new EnumValue
            {
                Key = "Hour6",
                Value = "21600"
            },
            new EnumValue
            {
                Key = "Hour12",
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
                Type = "System.String"
            },
            new AlgoMetaDataParameter
            {
                Key = "CandleInterval",
                Type = "Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval",
                PredefinedValues = CandleTimeIntervalPredefinedValues
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
                Type = "System.Double"
            },
            new AlgoMetaDataParameter
            {
                Key = "TradedAsset",
                Type = "System.String"
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
                    FunctionParameterType =
                        "Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Functions.SMA.SmaParameters",
                    Id = "Sma",
                    Type = "Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Functions.SMA.SmaFunction",
                    Parameters = new List<AlgoMetaDataParameter>
                    {
                       new AlgoMetaDataParameter
                        {
                            Key = "Capacity",
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
            FunctionParameterType = "AlgoParameters",
            Type = "CustomFunction",
            Parameters = new List<AlgoMetaDataParameter>
            {
                new AlgoMetaDataParameter
                {
                    Key = "Period",
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
            var code = "class A {} class B : C {}";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
            Then_Result_MustContainMessage(result, "AS0001");
        }

        [Test]
        public void SyntaxValidation_Fails_WhenMultipleClassesInheritBaseAlgo()
        {
            var code = "class A : BaseAlgo {} class B : BaseAlgo {}";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
            Then_Result_MustContainMessage(result, "AS0002");
        }

        [Test]
        public void SyntaxValidation_Fails_WhenAlgoNotSealed()
        {
            var code = "class A : BaseAlgo {}";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
            Then_Result_MustContainMessage(result, "AS0003");
        }

        [Test]
        public void SyntaxValidation_Fails_WhenTypeNamedBaseAlgo()
        {
            var code = "interface BaseAlgo {} enum BaseAlgo {} class BaseAlgo {} struct BaseAlgo {}";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
            Then_Result_MustContainMessageNTimes(result, "AS0004", 4);
        }

        [Test]
        public void SyntaxValidation_Fails_WhenEventsNotImplemented()
        {
            var code = "class A : BaseAlgo {void A() {} void OnCandleReceived() {}}";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
            Then_Result_MustContainMessage(result, "AS0005");
        }

        [Test]
        public void SyntaxValidation_Succeeds_WhenAlgoProperlyImplemented()
        {
            var code = @"using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain; 
                         sealed class A : BaseAlgo 
                         { 
                            public override void OnCandleReceived(ICandleContext context) {} 
                         }";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustSucceedAndContainNoMessages(result);
        }

        [Test]
        public void
            ExtractMetadata_Succeeds_WhenAlgoProperlyImplementedWithSmaFunctionAndNoAdditionalProperties()
        {
            var code = @"
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Functions.SMA;
sealed class Algo : BaseAlgo 
{ 
    public SmaFunction Sma {get; set; }
    public override void OnCandleReceived(ICandleContext context) {} 
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
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain; 
sealed class Algo : BaseAlgo 
{ 
    public override void OnCandleReceived(ICandleContext context) {} 
}
";

            var session = GetCSharpCodeBuildSession(code);
            var validationResult = session.Validate().Result;

            Assert.AreEqual(true, validationResult.IsSuccessful);

            var metadata = session.ExtractMetadata().Result;

            metadata.Should().BeEquivalentTo(BaseAlgoMetadataWithNoFunctions);
        }

        [Test]
        public void
            SyntaxValidation_Fails_WhenCustomFunctionIsNotProperlyImplementedFromAbstractFunction()
        {
            var code = @"
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Functions;
sealed class Algo : BaseAlgo 
{ 
    public SmaFunction Sma {get; set; }
    public override void OnCandleReceived(ICandleContext context) {} 
}
public class SmaFunction : AbstractFunction
{
}
";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
        }

        [Test]
        public void
            SyntaxValidation_Fails_WhenCustomFunctionIsNotProperlyImplementedFromFunctionInterface()
        {
            var code = @"
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain;
using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
sealed class Algo : BaseAlgo 
{ 
    public SmaFunction Sma {get; set; }
    public override void OnCandleReceived(ICandleContext context) {} 
}
public class SmaFunction : IFunction
{
}
";

            var result = When_Code_IsSyntaxValidated(code);

            Then_Result_MustFailAndContainMessages(result);
        }

        //REMARK: This test should fail on validation
        [Test]
        public void
            ExtractMetadata_Succeeds_WhenAlgoImplementedWithCustomFunctionThatHasNoConstructor()
        {
            var code = @"
using System.Collections.Generic;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Functions;

sealed class Algo : BaseAlgo
{
    public CustomFunction CustomFunc { get; set; }
    public override void OnStartUp(IFunctionProvider functions) { }
    public override void OnQuoteReceived(IQuoteContext context) { }
    public override void OnCandleReceived(ICandleContext context) { }
}

public class CustomFunction : IFunction
{
    public AlgoParameters AlgoParams { get; set; }
    public FunctionParamsBase FunctionParameters => AlgoParams;
    public double? Value => 43;
    public bool IsReady => true;

    public double? WarmUp(IEnumerable<Candle> values)
    {
        return 43;
    }

    public double? AddNewValue(Candle value)
    {
        return 43;
    }
}

public class AlgoParameters : FunctionParamsBase
{
    public int Period { get; set; }
}
";

            var session = GetCSharpCodeBuildSession(code);
            var validationResult = session.Validate().Result;

            Assert.AreEqual(true, validationResult.IsSuccessful);

            var metadata = session.ExtractMetadata().Result;

            metadata.Should().BeEquivalentTo(BaseAlgoMetadataWithNoFunctions);
        }

        [Test]
        public void
            ExtractMetadata_Succeeds_WhenAlgoImplementedWithCustomFunctionThatHasProperConstructor()
        {
            var code = @"
using System.Collections.Generic;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Functions;

sealed class Algo : BaseAlgo
{
    public CustomFunction CustomFunc { get; set; }

    public override void OnStartUp(IFunctionProvider functions)
    {
    }

    public override void OnQuoteReceived(IQuoteContext context)
    {
    }

    public override void OnCandleReceived(ICandleContext context)
    {
    }
}

public class CustomFunction : IFunction
{
    public AlgoParameters AlgoParams { get; set; }
    public FunctionParamsBase FunctionParameters => AlgoParams;
    public double? Value => 43;
    public bool IsReady => true;

    public double? WarmUp(IEnumerable<Candle> values)
    {
        return 43;
    }

    public double? AddNewValue(Candle value)
    {
        return 43;
    }

    public CustomFunction(AlgoParameters algoParameters)
    {
        AlgoParams = algoParameters;
    }
}

public class AlgoParameters : FunctionParamsBase
{
    public int Period { get; set; }
}
";

            var session = GetCSharpCodeBuildSession(code);
            var validationResult = session.Validate().Result;

            Assert.AreEqual(true, validationResult.IsSuccessful);

            var metadata = session.ExtractMetadata().Result;

            metadata.Should().BeEquivalentTo(BaseAlgoMetadataWithCustomFunction);
        }

        private ICodeBuildSession GetCSharpCodeBuildSession(string code)
        {
            var codeValidationService = new CodeBuildService();
            var session = codeValidationService.StartSession(code);

            return session;
        }

        private ValidationResult When_Code_IsSyntaxValidated(string code)
        {
            var codeValidationService = new CodeBuildService();
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
