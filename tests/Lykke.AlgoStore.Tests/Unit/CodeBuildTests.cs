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

        private static IList<AlgoMetaDataParameter> BaseAlgoParameters => new List<AlgoMetaDataParameter>
        {
            new AlgoMetaDataParameter { Key = "AssetPair", Type = "System.String" },
            new AlgoMetaDataParameter
            {
                Key = "CandleInterval",
                Type = "Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators.CandleTimeInterval",
                PredefinedValues = new List<EnumValue>
                {
                    new EnumValue { Key = "0", Value = "Unspecified"},
                    new EnumValue { Key = "1", Value = "Sec"},
                    new EnumValue { Key = "60", Value = "Minute"},
                    new EnumValue { Key = "300", Value = "Min5"},
                    new EnumValue { Key = "900", Value = "Min15"},
                    new EnumValue { Key = "1800", Value = "Min30"},
                    new EnumValue { Key = "3600", Value = "Hour"},
                    new EnumValue { Key = "14400", Value = "Hour4"},
                    new EnumValue { Key = "21600", Value = "Hour6"},
                    new EnumValue { Key = "43200", Value = "Hour12"},
                    new EnumValue { Key = "86400", Value = "Day"},
                    new EnumValue { Key = "604800", Value = "Week"},
                    new EnumValue { Key = "3000000", Value = "Month"}
                }
            },
            new AlgoMetaDataParameter { Key = "StartFrom", Type = "System.DateTime" },
            new AlgoMetaDataParameter { Key = "EndOn", Type = "System.DateTime" },
            new AlgoMetaDataParameter { Key = "Volume", Type = "System.Double" },
            new AlgoMetaDataParameter { Key = "TradedAsset", Type = "System.String" }
        };

        private static AlgoMetaDataInformation BaseAlgoMetadataWithNoFunctions => new AlgoMetaDataInformation
        {
            Functions = new List<AlgoMetaDataFunction>(),
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
            ExtractMetadata_Succeeds_SyntaxValidation_Succeeds_WhenAlgoProperlyImplementedAndHasNoAdditionalPropertiesAndFunctions()
        {
            var code = @"using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain; 
                         sealed class A : BaseAlgo 
                         { 
                            public override void OnCandleReceived(ICandleContext context) {} 
                         }";

            var session = GetCSharpCodeBuildSession(code);
            var validationResult = session.Validate().Result;

            Assert.AreEqual(true, validationResult.IsSuccessful);

            var metadata = session.ExtractMetadata().Result;

            metadata.Should().BeEquivalentTo(BaseAlgoMetadataWithNoFunctions);
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
