using DataAccess.DbContext;
using Domain.Entities;
using Domain.Interfaces.RepositoryInterfaces;

namespace DataAccess.Repositories
{
    public class MessageReactionUserRepository : GenericRepository<MessageReactionUser>, IMessageReactionUserRepository
    {
        public MessageReactionUserRepository(SocialNetworkDbContext context) : base(context)
        {

        }
    }
}
