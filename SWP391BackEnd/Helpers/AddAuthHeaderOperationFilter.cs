using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SWP391BackEnd.Helpers;

public class AddAuthHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var authParameter = operation.Parameters?.FirstOrDefault(p => p.Name == "Authorization");

        if (authParameter != null)
        {
            authParameter.Description = "Enter the access token in this input field";
            authParameter.Required = true;
        }

        if (operation.Security == null)
            operation.Security = new List<OpenApiSecurityRequirement>();

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                new List<string>()
            }
        });
    }
}