using IsVeskiPoc.Library.Generated;
using Microsoft.Extensions.DependencyInjection;

namespace IsVeskiPoc.Library.Client;

public static class ClientExtensions
{
    private const string BaseUrl = "https://isveski.is";

    public static IServiceCollection SetupClients(this IServiceCollection services, string apikey)
    {
        const string HttpClientFactoryName = "isveski";

        services.AddHttpClient(HttpClientFactoryName, client =>
        {
            client.BaseAddress = new Uri(BaseUrl);
            client.DefaultRequestHeaders.Add("x-api-key", apikey);
        });


        services.AddScoped((Func<IServiceProvider, IClientDeviceInterfaceClient>)(sp =>
        {
            var ret = new ClientDeviceInterfaceClient(
                BaseUrl, 
                sp.GetRequiredService<IHttpClientFactory>().CreateClient(HttpClientFactoryName));
            return ret;
        }));

        services.AddScoped((Func<IServiceProvider, IClientWalletClient>)(sp =>
        {
            var ret = new ClientWalletClient(
                BaseUrl,
                sp.GetRequiredService<IHttpClientFactory>().CreateClient(HttpClientFactoryName));
            return ret;
        }));
        
        return services;
    }
}
