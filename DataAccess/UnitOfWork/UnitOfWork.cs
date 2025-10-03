using DataAccess.DbContext;
using DataAccess.Repositories;
using Domain.Entities;
using Domain.Interfaces.RepositoryInterfaces;
using Domain.Interfaces.UnitOfWorkInterface;

namespace DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SocialNetworkDbContext _context;
        public UnitOfWork(
            SocialNetworkDbContext context
        )
        {
            _context = context;
        }
        public int Complete()
        {
            return _context.SaveChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
