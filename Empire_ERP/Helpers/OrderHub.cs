using Microsoft.AspNetCore.SignalR;

namespace Empire_ERP.Helpers
{
    public class OrderHub : Hub
    {
        public async Task SendOrder(string orderJson)
        {
            await Clients.All.SendAsync("ReceiveOrder", orderJson);
        }
    }
}
