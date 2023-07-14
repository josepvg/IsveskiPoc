using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IsVeskiPoc.Library.Authentication;

public static class SwaggerGenExtensions
{
    public static void AddIceWalletApiKeySupport(this SwaggerGenOptions setup)
    {
        setup.AddSecurityDefinition(ApiKeyAuthenticationHandler.SchemeName, new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = ApiKeyAuthenticationHandler.ApiKeyHeaderName,
            Type = SecuritySchemeType.ApiKey
        });
        setup.AddSecurityRequirement(new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = ApiKeyAuthenticationHandler.SchemeName
                    }
                },
                Array.Empty<string>()
            } });
    }
}