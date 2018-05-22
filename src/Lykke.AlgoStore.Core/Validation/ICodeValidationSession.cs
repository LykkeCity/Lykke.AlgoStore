using Lykke.AlgoStore.Core.Domain.Validation;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Validation
{
    /// <summary>
    /// Represents a code validation session on a given piece of code
    /// </summary>
    public interface ICodeValidationSession
    {
        /// <summary>
        /// Runs a validation on the code
        /// </summary>
        /// <returns>A <see cref="ValidationResult"/> containing the results of the validation</returns>
        Task<ValidationResult> Validate();
    }
}
