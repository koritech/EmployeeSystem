namespace EmployeeSystem
{
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class AddRequiredHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Processing-Mode",
                In = ParameterLocation.Header,
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new Microsoft.OpenApi.Any.OpenApiString("db"),
                    Enum = new List<Microsoft.OpenApi.Any.IOpenApiAny>
                {
                    new Microsoft.OpenApi.Any.OpenApiString("db"),
                    new Microsoft.OpenApi.Any.OpenApiString("kafka")
                }
                }
            });
        }
    }
}

