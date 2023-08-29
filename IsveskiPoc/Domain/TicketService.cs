using IsVeskiPoc.Library.Generated;
using System.Text.Json;

namespace IsveskiPoc.Domain;


//record KjarniPayload( string id);
public record KjarniPayload(string userName,int teriaCafeteriaId );

public class TicketService : ITicketService
{
    public static readonly Guid TicketDefinitionId = new Guid("e2a2e8c3-5005-4566-8b7d-76af98851800");

    private readonly IClientWalletClient _clientService;

    public TicketService(IClientWalletClient clientService)
    {
        this._clientService = clientService;
    }

    public async Task AddTicket(string icewalletusername, string idoncard)
    {
        var userId = await _clientService.SearchClientAsync(icewalletusername);
        var t1 = new Template1()
        {
            Image = "origologo",
            Description = "Bon apetit",
            Expiry = DateTime.Now.AddDays(10),
            Time = DateTime.Now,
            Title = "Mötuneytismiði"
        };

        var td1 = new DetailTemplate1()
        {
            Image = "origologo",
            Description = "Verði þér að góðu",
            Expiry = DateTime.Now.AddDays(11),
            Time = DateTime.Now,
            Title = "Mötuneytismiði",
            Buttons = new TemplateButton[0]
        };

        var data = new KjarniPayload("hif",1184);
        var payload = JsonSerializer.Serialize(data);
        var result = await _clientService.CreateTicketAsync(
            new CreateTicketDto()
            {
                UserId = userId,
                Data = payload,
                DetailPresentationData = JsonSerializer.Serialize(td1),
                PresentationData = JsonSerializer.Serialize(t1),
                DetailPresentationType = "iw/t1",
                PresentationType = "iw/t1",
                Name = "Mötuneyti",
                Note = "Sett á af poc",
                TicketDefinitionId = TicketDefinitionId
            }
        );
        _ = result;
    }
}
