using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace JobApplication.API.Extensions
{
    public class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
    {
        private const string SchemeName = "Bearer";

        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                In = ParameterLocation.Header,
                BearerFormat = "JWT"
            };

            var securitySchemeReference = new OpenApiSecuritySchemeReference(SchemeName, document, null);

            var protectedOperations = context.DescriptionGroups
                .SelectMany(g => g.Items)
                .Where(d => d.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
                .Select(d => (RelativePath: d.RelativePath?.TrimStart('/'), d.HttpMethod))
                .ToHashSet();

            foreach (var (path, pathItem) in document.Paths)
            {
                if (pathItem?.Operations is null)
                {
                    continue;
                }

                foreach (var (method, operation) in pathItem.Operations)
                {
                    var relativePath = path.TrimStart('/');

                    if (protectedOperations.Contains((relativePath, method.Method)))
                    {
                        operation.Security ??= new List<OpenApiSecurityRequirement>();
                        operation.Security.Add(new OpenApiSecurityRequirement
                        {
                            [securitySchemeReference] = new List<string>()
                        });
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
