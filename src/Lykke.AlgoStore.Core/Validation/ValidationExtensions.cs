using System.Collections.Generic;
using System.Linq;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.Core.Validation
{
    public static class ValidationExtensions
    {
        private const string ErrorPatternRequired = "{0} is required";

        public static bool ValidateData(this CSharp.AlgoTemplate.Models.Models.BaseValidatableData data, out AlgoStoreAggregateException exception)
        {
            exception = null;

            var validationResult = data.Validate(null).ToList();
            if (validationResult.IsNullOrEmptyCollection())
                return true;

            exception = new AlgoStoreAggregateException(AlgoStoreErrorCodes.ValidationError);
            foreach (var res in validationResult)
            {
                if (!exception.Errors.ContainsKey(res.ErrorMessage))
                    exception.Errors.Add(res.ErrorMessage, new List<string>());

                exception.Errors[res.ErrorMessage].AddRange(res.MemberNames);
            }

            return exception.Errors.Count == 0;
        }

        public static bool ValidateRequiredString(this string data, string memberName, out AlgoStoreAggregateException exception)
        {
            exception = null;

            if (!string.IsNullOrWhiteSpace(data))
                return true;

            exception = new AlgoStoreAggregateException(AlgoStoreErrorCodes.ValidationError);
            exception.Errors.Add(string.Format(ErrorPatternRequired, memberName), new List<string> { memberName });

            return false;
        }
    }
}
