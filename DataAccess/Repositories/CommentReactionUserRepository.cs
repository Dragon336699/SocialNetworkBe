using DataAccess.DbContext;
using Domain.Entities;
using Domain.Interfaces.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class CommentReactionUserRepository : GenericRepository<CommentReactionUser>, ICommentReactionUserRepository
    {
        public CommentReactionUserRepository(SocialNetworkDbContext context) : base(context)
        {
        }
    }
}
