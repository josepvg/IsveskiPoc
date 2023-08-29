using IsVeskiPoc.Library.Generated;

namespace IsveskiPoc.Domain;

public record IotPayload(string deviceName);

public class SignalHelper
{
    private readonly IClientDeviceInterfaceClient _device;
    private readonly IClientWalletClient _walletClient;
    private readonly HttpClient _signalClient;
    private Guid _communicationId;

    public SignalHelper(IClientDeviceInterfaceClient device, IClientWalletClient walletClient, HttpClient signalClient)
    {
        this._device = device;
        this._walletClient = walletClient;
        _signalClient=signalClient;
    }

    public void Init(Guid communicationId)
    {
        _communicationId = communicationId;
    }

    public async Task ShowMessage(string title, string message, string closebuttontext, TimeSpan duration)
    {
        await _device.ShowMessageAsync(new ShowMessageDto
        {
            CommunicationId = _communicationId,
            Close = closebuttontext,
            Message = message,
            Image = "origologo",
            TimeoutSek = (int)duration.TotalSeconds,
            Title = title
        });
    }

    public async Task AddMealLog(Guid ticketid, string title, string description)
    {
        var template = new LogTemplate1()
        {
            Title = title,
            Description = description,
            Time = DateTime.Now
        };
        var templateText = System.Text.Json.JsonSerializer.Serialize(template);
        await _walletClient.CreateTicketLogAsync(new CreateTicketLogDto
        {
            TicketId = ticketid,
            TicketLogType = TicketLogType.T1,
            LogData = templateText
        });
    }

    public async Task<bool> ShowPrompt(string title, string message, string yesbtn, string nobtn, TimeSpan duration)
    {
        var res = await _device.ShowPromptAsync(new ShowPromptDto
        {
            CommunicationId = _communicationId,
            Yes = yesbtn,
            No = nobtn,
            Message = message,
            Image = "origologo",
            TimeoutSek = (int)duration.TotalSeconds,
            Title = title
        });
        return res.Result;
    }

    public async Task<string> ShowMenu(string title, string message, string[] options, TimeSpan duration)
    {
        var res = await _device.ShowMenuAsync(new ShowMenuDto
        {
            CommunicationId = _communicationId,
            Options = options,
            Message = message,
            Image = "origologo",
            TimeoutSek = (int)duration.TotalSeconds,
            Title = title
        });
        return res.SelectedValue;
    }

    public async Task SignalDevice()
    {
        try
        {
            await _signalClient.PostAsJsonAsync("api/device/signal", new IotPayload("OrigoGate"));
        }catch (Exception ex)
        {
            //todo log
        }
    }
}
