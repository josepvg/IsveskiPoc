using IsVeskiPoc.Library.Generated;

namespace IsveskiPoc.Domain
{

    public class OnSensorLogicEndpoint
    {
        public static async Task OnSensor(RequestParameter requestParameter, 
            IClientDeviceInterfaceClient device, 
            IClientWalletClient walletClient,
            HttpClient signalClient)
        {
            var signalHelper = new SignalHelper(device, walletClient,signalClient);
            signalHelper.Init(requestParameter.CommunicationId);

            try
            {

                KjarniHelper kjarniHelper = new KjarniHelper();

                var ticket = requestParameter.Tickets?.FirstOrDefault(x => x.TicketDefinitionId == TicketService.TicketDefinitionId);
                if (ticket == null)
                {
                    await signalHelper.ShowMessage("Hafnað", "Finn ekki miða", "Loka", TimeSpan.FromSeconds(10));
                    return;
                }

                var payload = System.Text.Json.JsonSerializer.Deserialize<KjarniPayload>(ticket.Data)!;
                await kjarniHelper.Init(payload);

                var ticketIsValid = await kjarniHelper.TicketIsValid();
                if (!ticketIsValid)
                {
                    await signalHelper.ShowMessage("Hafnað", "Þú mátt ekki borða hérna", "Loka", TimeSpan.FromSeconds(10));
                    return;
                }

                var ticketHasRecentlyBeenUsed = await kjarniHelper.TicketHasRecentlyBeenUsed();
                var employeeName = await kjarniHelper.GetFullName();
                if (!ticketHasRecentlyBeenUsed)
                {
                    await signalHelper.ShowMessage("Velkomin", $"Velkomin í mötuneytið {employeeName}", "Loka", TimeSpan.FromSeconds(10));
                    var currentMeal = await kjarniHelper.FindMealForTheDay();
                    await signalHelper.AddMealLog(ticket.Id, "Þú borðaðir í mötuneytinu", $"Þú fékkst þér {currentMeal}");
                    await signalHelper.SignalDevice();
                    await kjarniHelper.RegisterPurchase();
                    return;
                }

                var payForAnother = await signalHelper.ShowPrompt("Miði nýlega notaður", "Viltu greiða fyrir annan?", "Já", "Nei", TimeSpan.FromSeconds(30));
                if (!payForAnother)
                {
                    await signalHelper.ShowMessage("Hætt við", "Þú valdir að greiða ekki fyrir annan", "Loka", TimeSpan.FromSeconds(10));
                    return;
                }

                var companyOption = "Fyrirtæki";
                var friendOption = "Vin";
                var payForType = await signalHelper.ShowMenu("Greiða fyrir annan", "Fyrir hvern viltu greiða",
                    new string[] { companyOption, friendOption, "Hætta við" }, TimeSpan.FromSeconds(30));
                if (payForType == companyOption || payForType == friendOption)
                {
                    await signalHelper.ShowMessage("Velkomin", $"Velkomin í mötuneytið {employeeName} þú greiddir fyrir annann", "Loka", TimeSpan.FromSeconds(10));
                    await signalHelper.AddMealLog(ticket.Id, "Þú borðaðir í mötuneytinu", $"Þú greiddir fyrir {payForType}");
                    await signalHelper.SignalDevice();
                    await kjarniHelper.RegisterPurchase();
                    return;
                }
                await signalHelper.ShowMessage("Hætt við", "Þú valdir að greiða ekki fyrir annan", "Loka", TimeSpan.FromSeconds(10));
            }
            catch (ApiException<DeviceException> e)
            {

                if (e.Result.StatusDetail == StatusDetail.Timeout)
                {
                    await signalHelper.ShowMessage("Tókst ekki", "Tíminn rann út", "Loka", TimeSpan.FromSeconds(20));
                }
                else if (e.Result.StatusDetail == StatusDetail.Cancelled)
                {
                    await signalHelper.ShowMessage("Tókst", "Notandi hætti við", "Loka", TimeSpan.FromSeconds(20));
                }

            }

        }
    }
}