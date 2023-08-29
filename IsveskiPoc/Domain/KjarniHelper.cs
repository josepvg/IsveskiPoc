namespace IsveskiPoc.Domain
{
    public class KjarniHelper
    {
        KjarniPayload _payload;
        private static DateTime? lastUsed;

        public async Task Init(KjarniPayload payload)
        {
            _payload = payload;
        }

        public async Task<bool> TicketIsValid() { 
            return true;
        }

        public async Task<bool> TicketHasRecentlyBeenUsed()
        {
            if(lastUsed.HasValue && lastUsed.Value.AddMinutes(1) > DateTime.Now)
            {
                return true;
            }
            return false;
        }

        public async Task<string> GetFullName()
        {
            return "Jósep Valur Guðlaugsson";
        }

        public async Task<string> FindMealForTheDay()
        {
            return "Ýsa í raspi";
        }

        public async Task RegisterPurchase()
        {
            lastUsed = DateTime.Now;
        }
    }
}
