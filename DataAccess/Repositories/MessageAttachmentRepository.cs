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
    public class MessageAttachmentRepository : GenericRepository<MessageAttachment>, IMessageAttachmentRepository
    {
        public MessageAttachmentRepository(SocialNetworkDbContext context): base(context)
        {
            
        }
    }
}
