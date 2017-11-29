using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;

namespace Lykke.AlgoStore.Core.Validation
{
    public static class ValidationExtensions
    {
        public static bool ValidateData(this BaseValidatableData data, out AlgoStoreAggregateException exception)
        {
            exception = new AlgoStoreAggregateException(AlgoStoreErrorCodes.ValidationError);

            var validationResult = data.Validate(new ValidationContext(data));

            foreach (var res in validationResult)
            {
                if (!exception.Errors.ContainsKey(res.ErrorMessage))
                    exception.Errors.Add(res.ErrorMessage, new List<string>());

                exception.Errors[res.ErrorMessage].AddRange(res.MemberNames);
            }

            return exception.Errors.Count == 0;
        }
    }
}
