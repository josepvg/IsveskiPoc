using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace IsVeskiPoc.Library.Authentication;

public static class AuthorizationExtensions
{
    public const string ICEWALLET_KEY_POLICY = "IceWalletKeyPolicy";

    public static IServiceCollection AddIceWalletApiKeyAuthorization(this IServiceCollection services, string apikey, Action<AuthorizationOptions> extraPoliciesBuilder = null )
    {
        services.AddSingleton<IAuthorizationHandler, ApiKeyRequirementHandler>();

        services.AddAuthorization(options => {
            options.AddPolicy(
                ICEWALLET_KEY_POLICY,
                policy => policy.Requirements.Add(new ApiKeyRequirement(apikey))
            );
            extraPoliciesBuilder?.Invoke(options);
        });
        return services;
    }
}
