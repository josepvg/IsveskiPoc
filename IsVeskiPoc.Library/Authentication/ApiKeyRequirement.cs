using Microsoft.AspNetCore.Authorization;

namespace IsVeskiPoc.Library.Authentication;

public class ApiKeyRequirement : IAuthorizationRequirement {
    public string ApiKey { get; private set; }

    public ApiKeyRequirement(string apiKey)
    {
        ApiKey = apiKey;
    }
}
