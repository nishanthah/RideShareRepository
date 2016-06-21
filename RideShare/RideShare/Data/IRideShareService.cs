using RideShare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RideShare.Data
{
    public interface IRideShareService
    {
        Task<List<User>> RefreshDataAsync();

        Task SaveTodoItemAsync(User item, bool isNewItem);

        Task DeleteTodoItemAsync(string id);
    }
}
