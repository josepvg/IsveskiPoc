namespace IsveskiPoc.Domain
{
    public interface ITicketService
    {
        Task AddTicket(string icewalletusername, string idoncard);
    }
}