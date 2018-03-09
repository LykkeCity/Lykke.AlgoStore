using System.Collections.Generic;

namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoMetaDataFunctionModel
    {
        /// <summary>
        /// Namescpace of the function, which will be used to load the function
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        ///Unique name of the function. We define it's id - name - by which we will call the function 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Type of parameters class. Namespace of the parameter class, whucj will be used to load the class
        /// </summary>
        public string FunctionParameterType { get; set; }

        /// <summary>
        /// Function Parameters
        /// </summary>
        public IEnumerable<AlgoMetaDataParameterModel> Parameters { get; set; }
    }
}
