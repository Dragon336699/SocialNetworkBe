using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.RepositoryInterfaces
{
    public interface IGroupRepository : IGenericRepository<Group>
    {
        Task<List<Group>> GetGroupsWithFullDetailsAsync(
            Expression<Func<Group, bool>> predicate,
            int skip = 0,
            int take = int.MaxValue);

        Task<Group?> GetGroupWithFullDetailsByIdAsync(Guid groupId);
    }

}
