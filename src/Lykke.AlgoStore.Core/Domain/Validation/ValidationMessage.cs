namespace Lykke.AlgoStore.Core.Domain.Validation
{
    /// <summary>
    /// Contains information about the place, severity and text of encountered problems during a validation
    /// </summary>
    public class ValidationMessage
    {
        /// <summary>
        /// The severity of this validation
        /// </summary>
        public ValidationSeverity Severity { get; set; }

        /// <summary>
        /// The line of the code this validation was encountered on
        /// </summary>
        public uint Line { get; set; }

        /// <summary>
        /// The column of the line this validation was encountered on
        /// </summary>
        public uint Column { get; set; }

        /// <summary>
        /// The unique ID representing the type of the validation
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The text content of the message
        /// </summary>
        public string Message { get; set; }

        public override string ToString()
        {
            return $"(Line: {Line}; Column: {Column}) {Id}: {Message}";
        }
    }
}
