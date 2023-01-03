using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.RegularExpressions;

namespace backend.Helpers
{
    internal class CamelCasePathDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var firstUpperCaseLetter = new Regex(@"(?<=[\/{])[A-Z]", RegexOptions.Compiled);// also applied to route parameters
            var toCamelCase = (string path) => firstUpperCaseLetter.Replace(path, match => match.Value.ToLower());

            var paths = swaggerDoc.Paths.ToDictionary(entry => toCamelCase(entry.Key), entry => entry.Value);

            swaggerDoc.Paths.Clear();
            foreach (var path in paths)
                swaggerDoc.Paths.Add(path.Key, path.Value);
        }
    }

    internal class SecurityRequirementOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!context.ApiDescription.CustomAttributes().Any(a => a is AllowAnonymousAttribute))
            {
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            }
        }
    }
}
