﻿using System.Collections.Generic;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Utils;

namespace Lykke.AlgoStore.Core.Validation
{
    public static class ValidationExtensions
    {
        public static bool ValidateData(this BaseValidatableData data, out AlgoStoreAggregateException exception)
        {
            exception = null;

            var validationResult = data.Validate(null);
            if (validationResult.IsNullOrEmptyEnumerable())
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
    }
}