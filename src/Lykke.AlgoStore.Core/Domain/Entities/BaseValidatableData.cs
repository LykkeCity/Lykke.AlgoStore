using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class BaseValidatableData : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return ValidateInternal(validationContext);
        }

        protected virtual IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(
                this,
                new ValidationContext(this, null, null),
                results,
                false);

            return results;

        }
    }
}
