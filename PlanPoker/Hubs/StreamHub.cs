using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PlanPoker.Models;
using PlanPoker.Services;

namespace PlanPoker.Hubs
{
    public class StreamHub : Hub
    {
        #region Variables
        private readonly IPresenceTracker _presenceTracker;
        #endregion

        #region Constructor
        public StreamHub(IPresenceTracker presenceTracker)
        {
            _presenceTracker = presenceTracker;
        }
        #endregion

        #region Methods
        public async Task AddUserToGroup(string groupName, string userName)
        {
            var user = new PpUser { ConnectionId = Context.ConnectionId, UserName = userName };
            await _presenceTracker.AddUserToGroup(user, groupName);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("addUserToGroup", user);
        }

        public async Task GetUsersByGroup(string groupName)
        {
            var users = await _presenceTracker.GetUsersByGroup(groupName);
            await Clients.Caller.SendAsync("getUsersByGroup", users);
        }

        public async Task SetUserSelection(string groupName, int selection)
        {
            var updatedUser = _presenceTracker.SetUserSelection(Context.ConnectionId, groupName, selection);
            await Clients.Group(groupName).SendAsync("setUserSelection", updatedUser);
        }

        public async Task ClearAllSelectionsForGroup(string groupName)
        {
            await _presenceTracker.ClearAllSelectionsForGroup(groupName);
            await Clients.Group(groupName).SendAsync("clearAllSelectionsForGroup", groupName);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _presenceTracker.RemoveUserByConnectionId(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        #endregion
    }
}
