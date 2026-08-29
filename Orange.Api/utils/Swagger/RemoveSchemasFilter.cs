using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Orange.Api.utils.Swagger;

public class RemoveSchemasFilter : IDocumentFilter
{
    private static readonly string[] SchemasToRemove =
    {
        "ProblemDetails",
    };
    
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var name in SchemasToRemove)
        {
            swaggerDoc.Components?.Schemas?.Remove(name);
        }
    }
}