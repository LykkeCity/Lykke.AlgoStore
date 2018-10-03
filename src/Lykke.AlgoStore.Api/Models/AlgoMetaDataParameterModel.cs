using System.Collections.Generic;

namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoMetaDataParameterModel
    {
        /// <summary>
        /// Unique name of the parameter. This value should be similar to variable defined in code
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Parameter value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Optional parameter description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Type of the parameter - DateTime, String, int, double, etc.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Is the parameter visible in the UI.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// If parameter is enum, it has predefined values which will be used from front-end guys for visualization in dropdowns
        /// </summary>
        public IEnumerable<EnumValue> PredefinedValues { get; set; }
    }
}
