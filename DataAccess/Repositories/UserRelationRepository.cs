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
    public class UserRelationRepository : GenericRepository<UserRelation>, IUserRelationRepository
    {
        public UserRelationRepository(SocialNetworkDbContext context) : base(context)
        {
        }

    }
}
