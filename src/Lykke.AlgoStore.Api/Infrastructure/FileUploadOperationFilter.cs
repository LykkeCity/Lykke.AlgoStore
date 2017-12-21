using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lykke.AlgoStore.Api.Models;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.AlgoStore.Api.Infrastructure
{
    //Modified one of provided solutions from
    //https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/193
    public class FileUploadOperationFilter : IOperationFilter
    {
        private const string FormDataMimeType = "multipart/form-data";
        private static readonly List<string> FormFilePropertyNames = new List<string> {  "Data" };

        public FileUploadOperationFilter()
        {
            FormFilePropertyNames.AddRange(typeof(IFormFile).GetTypeInfo().DeclaredProperties.Select(x => x.Name).ToList());
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ParameterDescriptions.Any(x => x.ModelMetadata.ContainerType == typeof(UploadAlgoBinaryModel)))
            {
                var formFileParameters = operation
                    .Parameters
                    .OfType<NonBodyParameter>()
                    .Where(x => FormFilePropertyNames.Contains(x.Name))
                    .ToArray();

                var index = operation.Parameters.IndexOf(formFileParameters.First());

                foreach (var formFileParameter in formFileParameters)
                {
                    operation.Parameters.Remove(formFileParameter);
                }

                var formFileParameterName = typeof(UploadAlgoBinaryModel)
                    .GetProperties()
                    .Where(x => x.PropertyType == typeof(IFormFile))
                    .FirstOrDefault()
                    ?.Name;

                var parameter = new NonBodyParameter()
                {
                    Name = formFileParameterName,
                    In = "formData",
                    Description = "The file to upload.",
                    Required = true,
                    Type = "file"
                };

                operation.Parameters.Insert(index, parameter);

                if (!operation.Consumes.Contains(FormDataMimeType))
                {
                    operation.Consumes.Add(FormDataMimeType);
                }
            }
        }
    }
}
