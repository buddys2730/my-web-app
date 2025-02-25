using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

public class AddFileParamTypesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
        {
            operation.Parameters = new System.Collections.Generic.List<OpenApiParameter>();
        }

        foreach (var param in context.ApiDescription.ParameterDescriptions)
        {
            if (param.Type == typeof(Microsoft.AspNetCore.Http.IFormFile))
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content =
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties =
                                {
                                    [param.Name] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Format = "binary"
                                    }
                                }
                            }
                        }
                    }
                };
            }
        }
    }
}
