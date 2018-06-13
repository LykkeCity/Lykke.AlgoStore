using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.Core.Domain.Validation
{
    /// <summary>
    /// Represents a code validation result
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Constructs a validation result given success status and messages
        /// </summary>
        /// <param name="isSuccessful">Whether the validation passed</param>
        /// <param name="messages">The list of messages associated with this validation</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="messages"/> is null</exception>
        public ValidationResult(bool isSuccessful, IReadOnlyCollection<ValidationMessage> messages)
        {
            IsSuccessful = isSuccessful;
            Messages = messages ?? throw new ArgumentNullException(nameof(messages));
        }

        /// <summary>
        /// Whether the validation has found no significant errors
        /// </summary>
        public bool IsSuccessful { get; }

        /// <summary>
        /// Contains all of the informational, warning and error messages for this validation
        /// </summary>
        public IReadOnlyCollection<ValidationMessage> Messages { get; }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Messages.Select(x => x.ToString()));
        }
    }
}
