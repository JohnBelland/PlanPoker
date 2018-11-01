using PlanPoker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanPoker.Services
{
    public class PresenceTracker : IPresenceTracker
    {
        #region Variables
        private static readonly Dictionary<string, List<PpUser>> _onlineUsersByGroup = new Dictionary<string, List<PpUser>>();
        #endregion

        #region Methods
        public Task AddUserToGroup(PpUser user, string groupName)
        {
            lock (_onlineUsersByGroup)
            {
                if (_onlineUsersByGroup.ContainsKey(groupName))
                {
                    _onlineUsersByGroup.Add(groupName, new List<PpUser> { user });
                }
                else
                {
                    _onlineUsersByGroup[groupName].Add(user);
                }
            }

            return Task.CompletedTask;
        }

        public Task<List<PpUser>> GetUsersByGroup(string groupName)
        {
            List<PpUser> result = new List<PpUser>();

            lock (_onlineUsersByGroup)
            {
                if (_onlineUsersByGroup.ContainsKey(groupName))
                {
                    result = _onlineUsersByGroup[groupName];
                }
            }

            return Task.FromResult(result);
        }

        public Task<PpUser> SetUserSelection(string connectionId, string groupName, int selection)
        {
            PpUser result = null;

            lock (_onlineUsersByGroup)
            {
                result = _onlineUsersByGroup[groupName].First(u => u.ConnectionId == connectionId);
                result.CurrentSelection = selection;
            }

            return Task.FromResult(result);
        }

        public Task ClearAllSelectionsForGroup(string groupName)
        {
            lock (_onlineUsersByGroup)
            {
                foreach (var user in _onlineUsersByGroup[groupName])
                {
                    user.CurrentSelection = null;
                }
            }

            return Task.CompletedTask;
        }

        public Task RemoveUserByConnectionId(string connectionId)
        {
            lock (_onlineUsersByGroup)
            {
                foreach(var userList in _onlineUsersByGroup.Values)
                {
                    foreach(var user in userList)
                    {
                        if (user.ConnectionId == connectionId)
                        {
                            userList.Remove(user);
                            break;
                        }
                    }
                }
                var emptyGroup = _onlineUsersByGroup.Where(item => item.Value.Count == 0);

                if (emptyGroup.Count() > 0)
                {
                    _onlineUsersByGroup.Remove(emptyGroup.First().Key);
                }
            }

            return Task.CompletedTask;
        }
        #endregion
    }
}
