using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Services.Implement
{
    [Authorize]
    public class NotificationHub
    {

        public class ManagerNotificationHub : Hub
        {
            public override async Task OnConnectedAsync()
            {
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
                }
                await base.OnConnectedAsync();
            }

            public override async Task OnDisconnectedAsync(Exception? exception)
            {
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
                }
                await base.OnDisconnectedAsync(exception);
            }
        }

    }
}
