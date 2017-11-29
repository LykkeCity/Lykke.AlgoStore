using System;
using System.Linq;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Validation;
using Xunit;

namespace Lykke.AlgoStore.Tests.Unit
{
    public class ValidationTests
    {
        [Fact]
        public void ErrorData_Validated_ReturnError()
        {
            var data = Given_Error_BaseValidatableData();
            bool result = When_Invoke_ValidateData(data, out AlgoStoreAggregateException exception);
            Then_Result_ShouldBe_False(result);
            And_Errors_ShouldBe_Populated(exception, "Name");
        }
        [Fact]
        public void CorrectData_Validated_ReturnOk()
        {
            var data = Given_Correct_BaseValidatableData();
            bool result = When_Invoke_ValidateData(data, out AlgoStoreAggregateException exception);
            Then_Result_ShouldBe_True(result);
            And_Errors_ShouldBe_Empty(exception);
        }

        private static BaseValidatableData Given_Error_BaseValidatableData()
        {
            var result = new AlgoMetaData();
            result.ClientAlgoId = Guid.NewGuid().ToString();
            result.Name = null;

            return result;
        }
        private static BaseValidatableData Given_Correct_BaseValidatableData()
        {
            var result = new AlgoMetaData();
            result.ClientAlgoId = Guid.NewGuid().ToString();
            result.Name = "Test";

            return result;
        }
        private static bool When_Invoke_ValidateData(BaseValidatableData data, out AlgoStoreAggregateException exception)
        {
            return data.ValidateData(out exception);
        }
        private static void Then_Result_ShouldBe_False(bool result)
        {
            Assert.False(result);
        }
        private static void Then_Result_ShouldBe_True(bool result)
        {
            Assert.True(result);
        }
        private static void And_Errors_ShouldBe_Populated(AlgoStoreAggregateException exception, string propertyName)
        {
            Assert.NotNull(exception);
            Assert.NotEmpty(exception.Errors);
            Assert.Single(exception.Errors);
            var error = exception.Errors.First();
            Assert.Contains(propertyName, error.Key);
            Assert.Contains(propertyName, error.Value[0]);
        }
        private static void And_Errors_ShouldBe_Empty(AlgoStoreAggregateException exception)
        {
            Assert.Null(exception);
        }
    }
}
