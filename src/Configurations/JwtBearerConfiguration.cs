using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.KeyCloak.Example.Configurations;

public static class JwtBearerConfiguration
{
    public static IServiceCollection AddJwtBearerConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        #if DEBUG
        
        services.AddAuthentication("BasicAuthentication")
            .AddScheme<AuthenticationSchemeOptions, AuthenticatedUser>("BasicAuthentication", null);

        #else
        
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                options.BackchannelHttpHandler = handler;

                options.Authority =
                    $"{configuration["Keycloak:Authority"]}{configuration["Keycloak:AuthorityRealms"]}{configuration["Keycloak:Realms"]}";
                options.Audience = configuration["Keycloak:ClientId"];
                options.RequireHttpsMetadata = true;
                options.IncludeErrorDetails = true;
                options.MetadataAddress =
                    $"{configuration["Keycloak:Authority"]}/realms/{configuration["Keycloak:Realms"]}{configuration["Keycloak:MetadataAddress"]}";

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = false,
                    ValidateIssuer = true,
                    ValidIssuer = $"{configuration["Keycloak:Authority"]}/realms/{configuration["Keycloak:Realms"]}",
                    ValidateLifetime = true,
                    SaveSigninToken = false
                };

                options.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.NoResult();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                            return Task.CompletedTask;
                        }
                        else
                        {
                            context.NoResult();
                            context.Response.StatusCode = context.HttpContext.Response.StatusCode;
                            
                            return Task.CompletedTask;
                        }
                    }
                };
            });
        
        #endif
        
        services.AddAuthorization();

        return services;
    }
}