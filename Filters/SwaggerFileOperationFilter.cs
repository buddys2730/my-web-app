using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class SwaggerFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.RequestBody != null)
        {
            operation.RequestBody.Content["multipart/form-data"] = new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = "object",
                    Properties =
                    {
                        ["file"] = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        }
                    },
                    Required = new HashSet<string> { "file" }
                }
            };
        }
    }
}
