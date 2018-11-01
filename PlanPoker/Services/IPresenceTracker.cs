using PlanPoker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanPoker.Services
{
    public interface IPresenceTracker
    {
        Task AddUserToGroup(PpUser user, string groupName);
        Task<List<PpUser>> GetUsersByGroup(string groupName);
        Task<PpUser> SetUserSelection(string connectionId, string groupName, int selection);
        Task ClearAllSelectionsForGroup(string groupName);
        Task RemoveUserByConnectionId(string connectionId);
    }
}
