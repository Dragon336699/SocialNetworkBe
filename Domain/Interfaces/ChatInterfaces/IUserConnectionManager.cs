using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.ChatInterfaces
{
    public interface IUserConnectionManager
    {
        Task AddConnectionAsync(string userId, string connectionId);
        Task RemoveConnectionAsync(string userId, string connectionId);
        Task<IEnumerable<string>?> GetConnectionAsync(Guid userId);
    }
}
