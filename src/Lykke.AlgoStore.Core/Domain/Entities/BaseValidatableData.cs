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
            if (validationContext != null)
                return results;

            Validator.TryValidateObject(
                this,
                new ValidationContext(this, null, null),
                results,
                true);

            return results;

        }
    }
}
