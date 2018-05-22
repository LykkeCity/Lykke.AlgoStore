using Lykke.AlgoStore.Core.Domain.Validation;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;

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

        /// <summary>
        /// Extract metadata from code
        /// </summary>
        /// <returns>A <see cref="AlgoMetaDataInformation"/> containing extracted algo metadata</returns>
        Task<AlgoMetaDataInformation> ExtractMetadata();
    }
}
