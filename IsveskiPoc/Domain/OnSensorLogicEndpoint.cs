using IsVeskiPoc.Library.Generated;

namespace IsveskiPoc.Domain
{
    public class OnSensorLogicEndpoint
    {
        private static Dictionary<string, int> _usages = new Dictionary<string, int>();

        public static async Task OnSensor(RequestParameter requestParameter, IClientDeviceInterfaceClient device)
        {
            try
            {
                var ticket = requestParameter.Tickets?.FirstOrDefault(x => x.TicketDefinitionId == TicketService.TicketDefinitionId);

                if (ticket != null)
                {
                    var payload = System.Text.Json.JsonSerializer.Deserialize<KjarniPayload>(ticket.Data);

                    if( HasAccessInKjarni(payload.id))
                    {
                        await device.ShowMessageAsync(new ShowMessageDto
                        {
                            CommunicationId = requestParameter.CommunicationId,
                            Close = "Ok bæ",
                            Message = "Þetta er skilaboð",
                            Image = "origologo",
                            TimeoutSek = 10,
                            Title = "Þetta er titill"
                        });
                    } else
                    {
                        await device.ShowMessageAsync(new ShowMessageDto
                        {
                            CommunicationId = requestParameter.CommunicationId,
                            Close = "Ok bæ",
                            Message = "Þú mátt ekki borða hérna",
                            Image = "origologo",
                            TimeoutSek = 10,
                            Title = "Hafnað!!"
                        });

                    }
                } else
                {
                    await device.ShowMessageAsync(new ShowMessageDto
                    {
                        CommunicationId = requestParameter.CommunicationId,
                        Close = "Ok bæ",
                        Message = "Þetta eru önnur skilaboð",
                        Image = "origologo",
                        TimeoutSek = 10,
                        Title = "Þetta er titill"
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static bool HasAccessInKjarni(string id)
        {
            return true;
        }
    }
}
