using System;
using System.Collections.Generic;
using System.Text;
using Lykke.AlgoStore.Core.Utils;

namespace Lykke.AlgoStore.Core.Domain.Errors
{
    public class AlgoStoreAggregateException : AlgoStoreException
    {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public AlgoStoreAggregateException(AlgoStoreErrorCodes errorCode) : base(errorCode)
        {
        }

        public Dictionary<string, List<string>> Errors
        {
            get
            {
                return _errors;
            }
        }

        public AlgoStoreAggregateException AddModelError(string key, string message)
        {
            if (!Errors.TryGetValue(key, out List<string> errors))
            {
                errors = new List<string>();

                Errors.Add(key, errors);
            }

            errors.Add(message);

            return this;
        }

        public AlgoStoreException ToBaseException(string displayMessage = "")
        {
            var sb = new StringBuilder();

            if (!_errors.IsNullOrEmptyCollection())
            {
                foreach (var values in _errors)
                {
                    sb.Append(string.Format("Error:{0} Fields:{1}", values.Key, String.Join(",", values.Value)));
                }
            }

            return new AlgoStoreException(ErrorCode, sb.ToString(), displayMessage);
        }
    }
}
