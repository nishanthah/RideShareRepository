using RideShare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideShare.Data
{
    public class UserManager
    {
        IRideShareService rideShareService;

        public UserManager(IRideShareService service)
        {
            rideShareService = service;
        }

        public Task<List<User>> GetTasksAsync()
        {
            return rideShareService.RefreshDataAsync();
        }

        public Task SaveTaskAsync(User item, bool isNewItem = false)
        {
            return rideShareService.SaveTodoItemAsync(item, isNewItem);
        }

        public Task DeleteTaskAsync(User item)
        {
            return rideShareService.DeleteTodoItemAsync(item.ID);
        }
    }
}
