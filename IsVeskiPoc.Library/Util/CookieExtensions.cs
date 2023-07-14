using Microsoft.AspNetCore.Http;

namespace IsVeskiPoc.Library.Util;

public static class CookieExtensions
{
    public static string? GetIsVeskiCookie(this HttpContext httpcontext)
    {
        if (httpcontext.Request.Cookies.TryGetValue("isveski", out var cookieValue))
        {
            return cookieValue;
        }
        return null;
    }
}
