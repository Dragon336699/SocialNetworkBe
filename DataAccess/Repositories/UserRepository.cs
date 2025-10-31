using DataAccess.DbContext;
using Domain.Entities;
using Domain.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(SocialNetworkDbContext context) : base(context)
        {

        }

        public async Task<IEnumerable<User>?> SearchUsers(string keywordNomarlized)
        {
            IEnumerable<User>? users = await _context.Users
                .Where(u => u.Email.ToLower().Contains(keywordNomarlized)
                        || u.UserName.ToLower().Contains(keywordNomarlized)
                        || u.FirstName.ToLower().Contains(keywordNomarlized)
                        || u.LastName.ToLower().Contains(keywordNomarlized))
                .AsNoTracking()
                .Take(10).ToListAsync();
            return users;
        }
    }
}
