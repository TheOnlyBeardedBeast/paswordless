using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Passwordless.Hubs
{
    public class AuthHub : Hub
    {

        public override Task OnConnectedAsync()
        {
            var id = this.Context.ConnectionId;

            Clients.Client(id).SendAsync("id",id);

            return base.OnConnectedAsync();
        }
    }
}
