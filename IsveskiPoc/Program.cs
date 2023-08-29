using IsveskiPoc;
using IsveskiPoc.Domain;
using IsVeskiPoc.Library.Authentication;
using IsVeskiPoc.Library.Client;
using IsVeskiPoc.Library.Generated;
using IsVeskiPoc.Library.Util;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddIceWalletApiKeySupport();
});

builder.Services.AddHealthChecks();

//Authentication and settings begins
builder.Services.AddAuthentication(ApiKeyAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
        ApiKeyAuthenticationHandler.SchemeName, null);

var pocSettings = new PocSettings();
builder.Configuration.Bind(nameof(PocSettings), pocSettings);

builder.Services.AddIceWalletApiKeyAuthorization(pocSettings.SignalApiKey);
builder.Services.SetupClients(pocSettings.ApiKey);
//Authentication and settings done


builder.Services.AddScoped<ITicketService, TicketService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Endpoint when a signal arrives
app.MapPost("/isveskisignal", async ([FromBody] RequestParameter parameter, 
                                                IClientDeviceInterfaceClient dev, 
                                                IClientWalletClient walletClient,
                                                IHttpClientFactory httpClientFactory) =>
{
    var httpClient = httpClientFactory.CreateClient("isveskiiot");
    await OnSensorLogicEndpoint.OnSensor(parameter, dev, walletClient, httpClient);
    return Results.Ok();
}).RequireAuthorization(AuthorizationExtensions.ICEWALLET_KEY_POLICY);

//Endpoint to add a ticket to the user
app.MapPost("/addtickethc", async (ITicketService service) =>
{
    await service.AddTicket("josepvg", "JosepKjarniId");
    return Results.Ok();
});

//Endpoint that is retrieved in a webview if the user doesn't have a ticket
app.MapGet("/noticket", (HttpContext context) =>
{
    var isveskiCookie = context.GetIsVeskiCookie();
    return new HtmlResult(
        $"<html><body><h1>You don't have a ticket</h1>Here's your cookie {isveskiCookie}, here we could have a login screen to</body></html>");
});

//Endpoint that is displayed in a browser if the user doesn't have the app
app.MapGet("/noapp", (HttpContext context) =>
{
    return new HtmlResult("<html><body><h1>You don't have the app, get it here</h1></body></html>");
});

app.MapHealthChecks("/healthz");

app.Run();
