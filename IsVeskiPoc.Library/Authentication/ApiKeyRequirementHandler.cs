using Microsoft.AspNetCore.Authorization;

namespace IsVeskiPoc.Library.Authentication;

public class ApiKeyRequirementHandler : AuthorizationHandler<ApiKeyRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
    {
        var apiHeader = context.User.FindFirst(c => c.Type == ApiKeyAuthenticationHandler.ApiKeyHeaderName);     
     
        if (apiHeader != null && apiHeader.Value.Equals(requirement.ApiKey))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
